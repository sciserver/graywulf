using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Threading;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Hosts WorkflowApplications to run multiple workflows in
    /// the same AppDomain.
    /// </summary>
    /// <remarks>
    /// This class is always instanciated inside a different AppDomain
    /// than the main one and marshaled back to provide cross-domain
    /// functionality.
    /// </remarks>
    class WorkflowApplicationHost : MarshalByRefObject
    {
        class WorkflowDetails
        {
            public Job Job;
            public WorkflowApplication WorkflowApplication;
            public Exception LastException;
        }

        #region Private variables

        private bool stopRequested;

        private Guid contextGuid;

        /// <summary>
        /// Reference to the scheduler
        /// </summary>
        private Scheduler scheduler;

        /// <summary>
        /// Holds the workflows hosted in the app domain
        /// </summary>
        private ConcurrentDictionary<Guid, WorkflowDetails> workflows;

        /// <summary>
        /// Logging participant, same for all WorkflowApplications
        /// </summary>
        private Jhu.Graywulf.Activities.GraywulfTrackingParticipant graywulfLogger;

        /// <summary>
        /// Persistence participant, same for all WorkflowApplications
        /// </summary>
        private SqlWorkflowInstanceStore workflowInstanceStore;

        #endregion

        public Guid ContextGuid
        {
            get { return contextGuid; }
            set { contextGuid = value; }
        }

        /// <summary>
        /// Reports workflow events to the main scheduler class
        /// </summary>
        public event EventHandler<WorkflowApplicationHostEventArgs> WorkflowEvent;

        public WorkflowApplicationHost()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.stopRequested = false;
            this.contextGuid = Guid.Empty;
            this.workflows = new ConcurrentDictionary<Guid, WorkflowDetails>();
            this.graywulfLogger = null;
            this.workflowInstanceStore = null;
        }
        
        public override object InitializeLifetimeService()
        {
            // Prevent remoting timeouts
            return null;
        }

        private void EnsureNotStopping()
        {
            if (stopRequested)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Starts a new Workflowhost
        /// </summary>
        public void Start(Scheduler scheduler, bool interactive)
        {
            EnsureNotStopping();

            // Store a reference to the scheduler
            this.scheduler = scheduler;

            // Initialize logging participant
            Logging.Logger.Instance.Writers.Add(new Jhu.Graywulf.Logging.SqlLogWriter());

            if (interactive)
            {
                Logging.Logger.Instance.Writers.Add(new Jhu.Graywulf.Logging.StreamLogWriter(Console.Out));
            }

            graywulfLogger = new GraywulfTrackingParticipant();

            // Initialize persistence participant
            workflowInstanceStore = new SqlWorkflowInstanceStore(Scheduler.Configuration.PersistenceConnectionString);
        }

        /// <summary>
        /// Drain-stops the workflow host by waiting for the
        /// workflows complete
        /// </summary>
        public void Stop(TimeSpan timeout)
        {
            EnsureNotStopping();
            stopRequested = true;

            // Wait until all workflows complete
            while (!workflows.IsEmpty)
            {
                Thread.Sleep(100);  // TODO: use constant
            }

            graywulfLogger = null;
            workflowInstanceStore = null;
            stopRequested = false;
        }

        /// <summary>
        /// Sets up a new workflow and starts the new job
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <returns></returns>
        public Guid PrepareStartJob(Job job)
        {
            EnsureNotStopping();

            // Load job data from the registry
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.ContextGuid = contextGuid;
                context.JobGuid = job.Guid;

                JobInstance ji = LoadJobInstance(context, job);

                // Deserialize parameters
                Dictionary<string, object> pars = new Dictionary<string, object>();
                foreach (Parameter par in ji.Parameters.Values)
                {
                    if ((par.Direction & ParameterDirection.In) != 0)
                    {
                        pars.Add(par.Name, ji.Parameters[par.Name].Value);
                    }
                }

                // Set default parameters
                pars.Add("UserGuid", ji.UserGuidOwner);
                pars.Add("JobGuid", ji.Guid);

                // Start the workflow
                Guid wfguid = PrepareStartWorkflow(job, pars);

                // Update registry
                ji.DateStarted = DateTime.Now;
                ji.WorkflowInstanceId = wfguid;
                ji.JobExecutionStatus = JobExecutionState.Executing;

                ji.Save();

                return wfguid;
            }
        }

        /// <summary>
        /// Resumes a previously persisted workflow
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <returns></returns>
        public Guid PrepareResumeJob(Job job)
        {
            EnsureNotStopping();

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.ContextGuid = contextGuid;
                context.JobGuid = job.Guid;

                JobInstance ji = LoadJobInstance(context, job);

                // Load assembly and create workflow instance
                Type wftype = Type.GetType(ji.WorkflowTypeName);

                // Resume the workflow
                Guid wfguid = PrepareResumeWorkflow(job, ji.WorkflowInstanceId);

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Executing;
                ji.Save();

                return wfguid;
            }
        }

        public void RunJob(Job job)
        {
            EnsureNotStopping();

            WorkflowDetails workflow;

            if (workflows.TryGetValue(job.WorkflowInstanceId, out workflow))
            {
                workflow.WorkflowApplication.Run();
            }
        }

        private void SaveWorkflowParameters(Job job, IDictionary<string, object> outputs)
        {
            // Load job data from the registry
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.ContextGuid = contextGuid;
                context.JobGuid = job.Guid;

                JobInstance ji = LoadJobInstance(context, job);

                foreach (var name in outputs.Keys)
                {
                    var par = outputs[name];

                    // If the parameter support or requires a registry context, set it now
                    // so serialization in the next step can proceed

                    if (par is IContextObject)
                    {
                        ((IContextObject)par).Context = context;
                    }

                    ji.Parameters[name].Value = par;
                }

                ji.Save();
            }
        }

        /// <summary>
        /// Cancels a running workflow.
        /// </summary>
        /// <remarks>
        /// This happened because of an explicit cancel request by a user.
        /// </remarks>
        public Guid CancelJob(Job job)
        {
            EnsureNotStopping();

            JobInstance ji;
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.ContextGuid = contextGuid;
                context.JobGuid = job.Guid;

                ji = LoadJobInstance(context, job);

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Cancelling;
                ji.Save();
            }

            return CancelWorkflow(ji.WorkflowInstanceId);
        }

        /// <summary>
        /// Cancels a running workflow by marking it timed-out
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public Guid TimeOutJob(Job job)
        {
            EnsureNotStopping();

            JobInstance ji;
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.ContextGuid = contextGuid;
                context.JobGuid = job.Guid;

                ji = LoadJobInstance(context, job);

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Cancelling;
                ji.Save();
            }

            return TimeOutWorkflow(ji.WorkflowInstanceId);
        }

        public Guid PersistJob(Job job)
        {
            EnsureNotStopping();

            JobInstance ji;
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                context.ContextGuid = contextGuid;
                context.JobGuid = job.Guid;

                ji = LoadJobInstance(context, job);

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Persisting;
                ji.Save();
            }

            // For some reason, unloading happens synchronously so to avoid deadlock with
            // the previous registry update, this has to be called outside the context
            return PersistWorkflow(ji.WorkflowInstanceId);
        }

        private JobInstance LoadJobInstance(Context context, Job job)
        {
            var ji = new JobInstance(context);
            ji.Guid = job.Guid;
            ji.Load();

            return ji;
        }

        private Type GetWorkflowType(Job job)
        {
            // Implicitely load the assembly.

            // TODO: If this fails, assembly loading failure
            // is the cause. Check ReflectionHelperInternal event handlers.
            Type wftype = Type.GetType(job.WorkflowTypeName);

            if (wftype == null)
            {
                throw new TypeLoadException(ExceptionMessages.ErrorLoadingWorkflowType);
            }

            return wftype;
        }

        /// <summary>
        /// Initializes a WorkflowApplication for the job
        /// </summary>
        /// <param name="wftype"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        private WorkflowApplication CreateWorkflowApplication(Type wftype, Dictionary<string, object> par)
        {
            // Instantiate workflow
            Activity wf = (Activity)Activator.CreateInstance(wftype);

            WorkflowApplication wfapp =
                par == null ? new WorkflowApplication(wf) : new WorkflowApplication(wf, par);

            // Add necessary participants
            wfapp.Extensions.Add(graywulfLogger);
            wfapp.Extensions.Add(scheduler);
            wfapp.InstanceStore = workflowInstanceStore;

            // Wire-up workflow runtime events
            wfapp.OnUnhandledException = wfapp_OnUnhandledException;
            wfapp.Completed = wfapp_WorkflowCompleted;
            wfapp.Unloaded = wfapp_WorkflowUnloaded;
            wfapp.Aborted = wfapp_WorkflowAborted;
            wfapp.Idle = wfapp_WorkflowIdle;
            wfapp.PersistableIdle = fwapp_PersistableIdle;

            return wfapp;
        }

        #region Workflow host actions

        /// <summary>
        /// Starts a job workflow
        /// </summary>
        /// <param name="wftype"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        private Guid PrepareStartWorkflow(Job job, Dictionary<string, object> par)
        {
            var wftype = GetWorkflowType(job);
            var wfapp = CreateWorkflowApplication(wftype, par);

            RegisterWorkflow(job, wfapp);
            return wfapp.Id;
        }

        /// <summary>
        /// Resumes a previously persisted job workflow
        /// </summary>
        /// <param name="wftype"></param>
        /// <param name="par"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        private Guid PrepareResumeWorkflow(Job job, Guid instanceId)
        {
            var wftype = GetWorkflowType(job);
            var wfapp = CreateWorkflowApplication(wftype, null);

            wfapp.Load(instanceId);
            RegisterWorkflow(job, wfapp);

            return wfapp.Id;
        }
        
        /// <summary>
        /// Do bookkeeping required when a workflow starts
        /// </summary>
        /// <param name="wfapp"></param>
        private void RegisterWorkflow(Job job, WorkflowApplication wfapp)
        {
            var workflow = new WorkflowDetails()
            {
                Job = job,
                WorkflowApplication = wfapp,
            };

            workflows.TryAdd(wfapp.Id, workflow);
        }

        private Guid CancelWorkflow(Guid instanceId)
        {
            WorkflowDetails workflow;

            if (workflows.TryGetValue(instanceId, out workflow))
            {
                workflow.Job.Status = JobStatus.Cancelled;
                workflow.WorkflowApplication.Cancel(Scheduler.Configuration.CancelTimeout);
            }
            
            return instanceId;
        }

        private Guid TimeOutWorkflow(Guid instanceId)
        {
            WorkflowDetails workflow;

            if (workflows.TryGetValue(instanceId, out workflow))
            {
                workflow.Job.Status = JobStatus.TimedOut;
                workflow.WorkflowApplication.Cancel(Scheduler.Configuration.CancelTimeout);
            }
            
            return instanceId;
        }

        private Guid PersistWorkflow(Guid instanceId)
        {
            WorkflowDetails workflow;

            if (workflows.TryGetValue(instanceId, out workflow))
            {
                workflow.Job.Status = JobStatus.Persisted;
                workflow.WorkflowApplication.Unload(Scheduler.Configuration.PersistTimeout);
            }
            
            return instanceId;
        }

        /// <summary>
        /// Do bookkeeping required when a workflow finishes
        /// </summary>
        /// <param name="instanceId"></param>
        private void FinishWorkflow(Guid instanceId)
        {
            WorkflowDetails workflow;
            workflows.TryRemove(instanceId, out workflow);
        }

        #endregion
        #region Workflow host events

        private UnhandledExceptionAction wfapp_OnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            // This is important to gracefully unload faulted or canceled/timed-out.
            // It causes the runtime to cancel the workflow instead of abort it instantenously.
            // This will cause the cancel logic and finally activities of a try-catch-finally activity to
            // execute prior to the unloading of the workflow.

            // First we have to store the exception because it won't be available after the
            // workflow gracefully cancels
            WorkflowDetails workflow;

            if (workflows.TryGetValue(e.InstanceId, out workflow))
            {
                workflow.Job.Status = JobStatus.Failed;
                workflow.LastException = e.UnhandledException;
            }

            // Force the workflow to execute the cancel logic
            return UnhandledExceptionAction.Cancel;
        }

        /// <summary>
        /// Executes when a workflow completed either successfully, either failing.
        /// </summary>
        /// <param name="e"></param>
        private void wfapp_WorkflowCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            WorkflowDetails workflow;

            if (workflows.TryGetValue(e.InstanceId, out workflow))
            {
                // This is the point to save output parameters
                SaveWorkflowParameters(workflow.Job, e.Outputs);

                if (WorkflowEvent != null)
                {

                    switch (e.CompletionState)
                    {
                        case ActivityInstanceState.Closed:
                            // Completed successfully
                            WorkflowEvent(this, new WorkflowApplicationHostEventArgs(WorkflowEventType.Completed, e.InstanceId));
                            break;
                        case ActivityInstanceState.Canceled:
                            switch (workflow.Job.Status)
                            {
                                case JobStatus.Cancelled:
                                    WorkflowEvent(this, new WorkflowApplicationHostEventArgs(WorkflowEventType.Cancelled, e.InstanceId));
                                    break;
                                case JobStatus.TimedOut:
                                    WorkflowEvent(this, new WorkflowApplicationHostEventArgs(WorkflowEventType.TimedOut, e.InstanceId));
                                    break;
                                case JobStatus.Failed:

#if BREAKDEBUG
                                System.Diagnostics.Debugger.Break();
#endif

                                    WorkflowEvent(
                                        this,
                                        new WorkflowApplicationHostEventArgs(
                                            WorkflowEventType.Failed,
                                            e.InstanceId,
                                            GetExceptionMessage(workflow.LastException)));
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            break;
                        case ActivityInstanceState.Faulted:
                        // This should not happen, workflows are forced to cancel
                        case ActivityInstanceState.Executing:
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        private void wfapp_WorkflowUnloaded(WorkflowApplicationEventArgs e)
        {
            WorkflowDetails workflow;

            if (workflows.TryGetValue(e.InstanceId, out workflow))
            {
                // Assume that event is wired-up, otherwise it won't work anyway
                switch (workflow.Job.Status)
                {
                    case JobStatus.Persisted:
                        WorkflowEvent(this, new WorkflowApplicationHostEventArgs(WorkflowEventType.Persisted, e.InstanceId));
                        break;
                    default:
                        break;
                }

                FinishWorkflow(e.InstanceId);
            }
        }

        private void wfapp_WorkflowAborted(WorkflowApplicationEventArgs e)
        {
            WorkflowDetails workflow;

            if (workflows.TryGetValue(e.InstanceId, out workflow))
            {

                // Workflows are aborted when an exception is thrown during cancellation
                if (workflow.LastException != null)
                {
                    WorkflowEvent(
                        this,
                        new WorkflowApplicationHostEventArgs(
                            WorkflowEventType.Failed,
                            e.InstanceId,
                            GetExceptionMessage(workflow.LastException)));
                }
                else
                {
                    WorkflowEvent(this, new WorkflowApplicationHostEventArgs(WorkflowEventType.Failed, e.InstanceId));
                }

                FinishWorkflow(e.InstanceId);
            }
        }

        private void wfapp_WorkflowIdle(WorkflowApplicationIdleEventArgs e)
        {
        }

        private PersistableIdleAction fwapp_PersistableIdle(WorkflowApplicationIdleEventArgs e)
        {
            return PersistableIdleAction.Persist;
        }

        #endregion

        private string GetExceptionMessage(Exception exception)
        {
            if (exception is AggregateException)
            {
                return exception.InnerException.Message;
            }
            else
            {
                return exception.Message;
            }
        }
    }
}
