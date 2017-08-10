using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Runtime.DurableInstancing;
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
    class SchedulerWorkflowApplicationHost : WorkflowApplicationHostBase
    {
        public new class WorkflowApplicationDetails : WorkflowApplicationHostBase.WorkflowApplicationDetails
        {
            public Job Job;
        }

        #region Private variables

        private Guid guid;

        /// <summary>
        /// Reference to the scheduler
        /// </summary>
        private Scheduler scheduler;

        /// <summary>
        /// Persistence participant, same for all WorkflowApplications
        /// </summary>
        private SqlWorkflowInstanceStore workflowInstanceStore;
        private InstanceHandle workflowInstanceHandle;

        #endregion
        #region Properties

        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        #endregion

        public SchedulerWorkflowApplicationHost()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.workflowInstanceStore = null;
        }

        private RegistryContext CreateRegistryContext()
        {
            var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.ManualCommit);
            context.LockOwner = this.guid;
            return context;
        }

        /// <summary>
        /// Starts a new Workflowhost
        /// </summary>
        public void Start(Guid guid, Scheduler scheduler, bool interactive)
        {
            Start(interactive);

            this.guid = guid;
            this.scheduler = scheduler;

            // Initialize persistence participant
            workflowInstanceStore = new SqlWorkflowInstanceStore(Scheduler.Configuration.PersistenceConnectionString);
            workflowInstanceStore.InstanceCompletionAction = InstanceCompletionAction.DeleteAll;
            // Create the instance store owner
            workflowInstanceHandle = workflowInstanceStore.CreateInstanceHandle();
            var view = workflowInstanceStore.Execute(workflowInstanceHandle, new CreateWorkflowOwnerCommand(), TimeSpan.FromSeconds(30));
            workflowInstanceStore.DefaultInstanceOwner = view.InstanceOwner;

        }

        /// <summary>
        /// Drain-stops the workflow host by waiting for the
        /// workflows complete
        /// </summary>
        public override bool TryStop()
        {
            if (base.TryStop())
            {
                //var deleteOwnerCmd = new DeleteWorkflowOwnerCommand();
                //workflowInstanceStore.Execute(workflowInstanceHandle, deleteOwnerCmd, TimeSpan.FromSeconds(30));
                workflowInstanceStore.DefaultInstanceOwner = null;
                workflowInstanceHandle.Free();
                workflowInstanceStore = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets up a new workflow and starts the new job
        /// </summary>
        public Guid PrepareStartJob(Job job)
        {
            new JobContext(job).Push();

            Guid wfguid;

            using (var context = CreateRegistryContext())
            {
                var ji = LoadJobInstance(context, job);

                // Deserialize parameters
                Dictionary<string, object> pars = new Dictionary<string, object>();
                foreach (Parameter par in ji.Parameters.Values)
                {
                    if ((par.Direction & ParameterDirection.In) != 0)
                    {
                        pars.Add(par.Name, ji.Parameters[par.Name].Value);
                    }
                }

                // Set default parameters to be passed to the job
                // These will be passed around from activity to activity and
                // can be used by the job tracker (logger)
                var jobinfo = new JobInfo((JobInfo)job);
                pars.Add(Jhu.Graywulf.Activities.Constants.ActivityParameterJobInfo, jobinfo);

                // Start the workflow
                wfguid = PrepareStartWorkflow(job, pars);

                // Update registry
                ji.DateStarted = DateTime.Now;
                ji.WorkflowInstanceId = wfguid;
                ji.JobExecutionStatus = JobExecutionState.Executing;
                ji.Save();

                context.CommitTransaction();
            }

            JobContext.Current.Pop();

            return wfguid;
        }

        /// <summary>
        /// Resumes a previously persisted workflow
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <returns></returns>
        public Guid PrepareResumeJob(Job job)
        {
            new JobContext(job).Push();

            Guid wfguid;

            using (var context = CreateRegistryContext())
            {
                JobInstance ji = LoadJobInstance(context, job);

                // Load assembly and create workflow instance
                Type wftype = Type.GetType(ji.WorkflowTypeName);

                // Resume the workflow
                wfguid = PrepareResumeWorkflow(job, ji.WorkflowInstanceId);

                // Update registry
                ji.WorkflowInstanceId = wfguid;
                ji.JobExecutionStatus = JobExecutionState.Executing;
                ji.Save();

                context.CommitTransaction();
            }

            JobContext.Current.Pop();

            return wfguid;
        }

        public void RunJob(Job job)
        {
            RunWorkflow(job.WorkflowInstanceId);
        }

        private void SaveJobParameters(Job job, IDictionary<string, object> outputs)
        {
            new JobContext(job).Push();

            // Load job data from the registry
            using (var context = CreateRegistryContext())
            {
                JobInstance ji = LoadJobInstance(context, job);

                foreach (var name in outputs.Keys)
                {
                    var par = outputs[name];

                    // If the parameter support or requires a registry context, set it now
                    // so serialization in the next step can proceed

                    if (par is IRegistryContextObject)
                    {
                        ((IRegistryContextObject)par).RegistryContext = context;
                    }

                    ji.Parameters[name].Value = par;
                }

                ji.Save();

                context.CommitTransaction();
            }

            JobContext.Current.Pop();
        }

        /// <summary>
        /// Cancels a running workflow.
        /// </summary>
        /// <remarks>
        /// This happened because of an explicit cancel request by a user.
        /// </remarks>
        public Guid CancelJob(Job job)
        {
            new JobContext(job).Push();

            Guid wfguid;

            using (var context = CreateRegistryContext())
            {
                var ji = LoadJobInstance(context, job);

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Cancelling;
                ji.Save();

                wfguid = ji.WorkflowInstanceId;

                context.CommitTransaction();
            }

            CancelWorkflow(wfguid, Scheduler.Configuration.CancelTimeout);

            JobContext.Current.Pop();

            return wfguid;
        }

        /// <summary>
        /// Cancels a running workflow by marking it timed-out
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public Guid TimeOutJob(Job job)
        {
            new JobContext(job).Push();

            Guid wfguid;

            using (var context = CreateRegistryContext())
            {
                var ji = LoadJobInstance(context, job);

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Cancelling;
                ji.Save();

                wfguid = ji.WorkflowInstanceId;

                context.CommitTransaction();
            }

            TimeOutWorkflow(wfguid, Scheduler.Configuration.CancelTimeout);

            JobContext.Current.Pop();

            return wfguid;
        }

        public Guid PersistJob(Job job)
        {
            new JobContext(job).Push();

            Guid wfguid;

            using (var context = CreateRegistryContext())
            {
                var ji = LoadJobInstance(context, job);

                // Update registry
                ji.JobExecutionStatus = JobExecutionState.Persisting;
                ji.Save();

                wfguid = ji.WorkflowInstanceId;

                context.CommitTransaction();
            }

            // For some reason, unloading happens synchronously so to avoid deadlock with
            // the previous registry update, this has to be called outside the context
            PersistWorkflow(wfguid);

            JobContext.Current.Pop();

            return wfguid;
        }

        private JobInstance LoadJobInstance(RegistryContext context, Job job)
        {
            var ef = new EntityFactory(context);
            var ji = ef.LoadEntity<JobInstance>(job.Guid);
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

        protected override WorkflowApplication CreateWorkflowApplication(Type wftype, Dictionary<string, object> par)
        {
            var wfapp = base.CreateWorkflowApplication(wftype, par);

            // Add necessary participants
            wfapp.Extensions.Add(scheduler);
            wfapp.InstanceStore = workflowInstanceStore;

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

            BookkeepWorkflow(job, wfapp);
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
            BookkeepWorkflow(job, wfapp);

            return wfapp.Id;
        }

        /// <summary>
        /// Do bookkeeping required when a workflow starts
        /// </summary>
        /// <param name="wfapp"></param>
        private void BookkeepWorkflow(Job job, WorkflowApplication wfapp)
        {
            var workflow = new WorkflowApplicationDetails()
            {
                Job = job,
                WorkflowApplication = wfapp,
            };

            BookkeepWorkflow(wfapp, workflow);
        }

        protected override void OnWorkflowCancelling(WorkflowApplicationHostBase.WorkflowApplicationDetails w)
        {
            base.OnWorkflowCancelling(w);

            var workflow = (WorkflowApplicationDetails)w;

            workflow.Job.Status = JobStatus.Cancelled;
        }

        protected override void OnWorkflowTimingOut(WorkflowApplicationHostBase.WorkflowApplicationDetails w)
        {
            base.OnWorkflowTimingOut(w);

            var workflow = (WorkflowApplicationDetails)w;

            workflow.Job.Status = JobStatus.TimedOut;
        }

        protected override void OnWorkflowPersisting(WorkflowApplicationHostBase.WorkflowApplicationDetails w)
        {
            base.OnWorkflowPersisting(w);

            var workflow = (WorkflowApplicationDetails)w;
            workflow.Job.Status = JobStatus.Persisting;

            /* TODO: this often throws the following:
             * An exception of type 'System.Runtime.DurableInstancing.InstanceOwnerException' 
             * occurred in System.ServiceModel.Internals.dll but was not handled in user code
               Additional information: The execution of an InstancePersistenceCommand was interrupted 
               because the instance owner registration for owner ID 'd4fd92a7-1350-44a5-9dee-87c47f8c40b4' 
               has become invalid. This error indicates that the in-memory copy of all instances locked by 
               this owner have become stale and should be discarded, along with the InstanceHandles. 
               Typically, this error is best handled by restarting the host.
            */

            workflow.WorkflowApplication.Unload(Scheduler.Configuration.PersistTimeout);
        }

        #endregion
        #region Workflow host events

        protected override void OnWorkflowUnHandledException(WorkflowApplicationUnhandledExceptionEventArgs e, WorkflowApplicationHostBase.WorkflowApplicationDetails w)
        {
            base.OnWorkflowUnHandledException(e, w);

            var workflow = (WorkflowApplicationDetails)w;

            workflow.Job.Status = JobStatus.Failed;
        }

        protected override void OnWorkflowCompleted(WorkflowApplicationCompletedEventArgs e, WorkflowApplicationHostBase.WorkflowApplicationDetails w)
        {
            var workflow = (WorkflowApplicationDetails)w;

            // This is the point to save output parameters
            SaveJobParameters(workflow.Job, e.Outputs);

            switch (e.CompletionState)
            {
                case ActivityInstanceState.Closed:
                    // Completed successfully
                    OnWorkflowEvent(
                        new WorkflowApplicationHostEventArgs(
                            WorkflowEventType.Completed,
                            e.InstanceId));
                    break;
                case ActivityInstanceState.Canceled:
                    switch (workflow.Job.Status)
                    {
                        case JobStatus.Cancelled:
                            OnWorkflowEvent(
                                new WorkflowApplicationHostEventArgs(
                                    WorkflowEventType.Cancelled,
                                    e.InstanceId));
                            break;
                        case JobStatus.TimedOut:
                            OnWorkflowEvent(
                                new WorkflowApplicationHostEventArgs(
                                    WorkflowEventType.TimedOut,
                                    e.InstanceId));
                            break;
                        case JobStatus.Failed:

#if BREAKDEBUG
                            if (System.Diagnostics.Debugger.IsAttached)
                            {
                                System.Diagnostics.Debugger.Break();
                            }
#endif

                            OnWorkflowEvent(
                                new WorkflowApplicationHostEventArgs(
                                    WorkflowEventType.Failed,
                                    e.InstanceId,
                                    workflow.LastException));
                            break;
                        default:
                            // TODO: what about persisting and persisted stages?
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

        protected override void OnWorkflowUnloaded(WorkflowApplicationEventArgs e, WorkflowApplicationHostBase.WorkflowApplicationDetails w)
        {
            base.OnWorkflowUnloaded(e, w);

            var workflow = (WorkflowApplicationDetails)w;

            switch (workflow.Job.Status)
            {
                case JobStatus.Persisting:
                    workflow.Job.Status = JobStatus.Persisted;
                    OnWorkflowEvent(new WorkflowApplicationHostEventArgs(WorkflowEventType.Persisted, e.InstanceId));
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
