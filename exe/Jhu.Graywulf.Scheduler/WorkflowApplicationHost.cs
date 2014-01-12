using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Runtime.DurableInstancing;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
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

        private object syncRoot;
        private bool stopRequested;

        private Guid contextGuid;

        /// <summary>
        /// Reference to the scheduler
        /// </summary>
        private Scheduler scheduler;

        /// <summary>
        /// Holds the workflows hosted in the app domain
        /// </summary>
        private Dictionary<Guid, WorkflowDetails> workflows;

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
        public event EventHandler<HostEventArgs> WorkflowEvent;

        public WorkflowApplicationHost()
        {
            InitializeMembers();
            InitializeAppDomain();
        }

        private void InitializeMembers()
        {
            this.syncRoot = new object();
            this.stopRequested = false;
            this.contextGuid = Guid.Empty;
            this.workflows = new Dictionary<Guid, WorkflowDetails>();
            this.graywulfLogger = null;
            this.workflowInstanceStore = null;
        }

        public override object InitializeLifetimeService()
        {
            // Prevent remoting timeouts
            return null;
        }

        /// <summary>
        /// Initializes the AppDomain.
        /// </summary>
        private void InitializeAppDomain()
        {
            AppDomain ad = AppDomain.CurrentDomain;

            // All other events will be handled by the ReflectionHelperInternal class when initialized
            ad.UnhandledException += new UnhandledExceptionEventHandler(ad_UnhandledException);
        }

        #region AppDomain event handlers

        //*** TODO: move this to an appdomainhelper class
        void ad_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // **** TODO: handle app domain level exception here
            // to prevent failing of the entire scheduler
            throw (Exception)e.ExceptionObject;
        }

        #endregion

        private void EnsureNotStopping()
        {
            lock (syncRoot)
            {
                if (stopRequested)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Starts a new Workflowhost
        /// </summary>
        public void Start(Scheduler scheduler, bool interactive)
        {
            EnsureNotStopping();

            // Store a reference to the proxy to the scheduler
            this.scheduler = scheduler;

            // Initialize logging participant
            // Have to add console logger because it's a new AppDomain
            if (interactive)
            {
                Jhu.Graywulf.Logging.Logger.Instance.Writers.Add(new Jhu.Graywulf.Logging.StreamLogWriter(Console.Out));
            }

            graywulfLogger = new Jhu.Graywulf.Activities.GraywulfTrackingParticipant();

            // Initialize persistence participant
            workflowInstanceStore = new SqlWorkflowInstanceStore(AppSettings.PersistenceConnectionString);
        }

        /// <summary>
        /// Drain-stops the workflow host by waiting for the
        /// workflows complete
        /// </summary>
        public void Stop(TimeSpan timeout)
        {
            lock (syncRoot)
            {
                if (stopRequested)
                {
                    throw new InvalidOperationException();
                }

                stopRequested = true;
            }

            // Wait until all workflows complete

            while (true)
            {
                lock (syncRoot)
                {
                    if (workflows.Count == 0)
                    {
                        break;
                    }
                }

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
                foreach (JobInstanceParameter par in ji.Parameters.Values)
                {
                    if ((par.Direction & JobParameterDirection.In) != 0)
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

            lock (syncRoot)
            {
                workflows[job.WorkflowInstanceId].WorkflowApplication.Run();
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
                    ji.Parameters[name].Value = outputs[name];
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
            lock (syncRoot)
            {
                workflows.Add(wfapp.Id, new WorkflowDetails()
                {
                    Job = job,
                    WorkflowApplication = wfapp,
                });
            }
        }

        private Guid CancelWorkflow(Guid instanceId)
        {
            // *** TODO: handle timeout exception here
            workflows[instanceId].Job.Status = JobStatus.Cancelled;
            workflows[instanceId].WorkflowApplication.Cancel(AppSettings.CancelTimeout);
            //workflows[instanceId].WorkflowApplication.Cancel();

            return instanceId;
        }

        private Guid TimeOutWorkflow(Guid instanceId)
        {
            // *** TODO: handle timeout exception here
            workflows[instanceId].Job.Status = JobStatus.TimedOut;
            workflows[instanceId].WorkflowApplication.Cancel(AppSettings.CancelTimeout);

            return instanceId;
        }

        private Guid PersistWorkflow(Guid instanceId)
        {
            // *** TODO: this might sometime throw a KeyNotFoundException (after persist and cancel?)
            // *** TODO: handle timeout exception here
            workflows[instanceId].Job.Status = JobStatus.Persisted;
            workflows[instanceId].WorkflowApplication.Unload(AppSettings.PersistTimeout);

            return instanceId;
        }

        /// <summary>
        /// Do bookkeeping required when a workflow finishes
        /// </summary>
        /// <param name="instanceId"></param>
        private void FinishWorkflow(Guid instanceId)
        {
            lock (syncRoot)
            {
                workflows.Remove(instanceId);
            }
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
            lock (syncRoot)
            {
                var workflow = workflows[e.InstanceId];
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
            lock (syncRoot)
            {
                workflow = workflows[e.InstanceId];
            }

            // This is the point to save output parameters
            SaveWorkflowParameters(workflow.Job, e.Outputs);

            if (WorkflowEvent != null)
            {

                switch (e.CompletionState)
                {
                    case ActivityInstanceState.Closed:
                        // Completed successfully
                        WorkflowEvent(this, new HostEventArgs(WorkflowEventType.Completed, e.InstanceId));
                        break;
                    case ActivityInstanceState.Canceled:
                        switch (workflow.Job.Status)
                        {
                            case JobStatus.Cancelled:
                                WorkflowEvent(this, new HostEventArgs(WorkflowEventType.Cancelled, e.InstanceId));
                                break;
                            case JobStatus.TimedOut:
                                WorkflowEvent(this, new HostEventArgs(WorkflowEventType.TimedOut, e.InstanceId));
                                break;
                            case JobStatus.Failed:
                                WorkflowEvent(this, new HostEventArgs(WorkflowEventType.Failed, e.InstanceId, workflow.LastException.Message));
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

        private void wfapp_WorkflowUnloaded(WorkflowApplicationEventArgs e)
        {
            WorkflowDetails workflow;
            lock (syncRoot)
            {
                workflow = workflows[e.InstanceId];
            }

            // Assume that event is wired-up, otherwise it won't work anyway
            switch (workflow.Job.Status)
            {
                case JobStatus.Persisted:
                    WorkflowEvent(this, new HostEventArgs(WorkflowEventType.Persisted, e.InstanceId));
                    break;
                default:
                    break;
            }

            FinishWorkflow(e.InstanceId);
        }

        private void wfapp_WorkflowAborted(WorkflowApplicationEventArgs e)
        {
            WorkflowDetails workflow;
            lock (syncRoot)
            {
                workflow = workflows[e.InstanceId];
            }

            // TODO: this might be needed here for Kill to work correctly, needs testing
            //WorkflowEvent(this, new HostEventArgs(WorkflowEventType.Failed, e.InstanceId, workflow.LastException.Message));

            FinishWorkflow(e.InstanceId);
        }

        #endregion

    }
}
