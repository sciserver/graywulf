using System;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Wraps an AppDomain that can be unloaded when idle.
    /// </summary>
    /// <remarks>
    /// Every WorkflowApplication is started in a separate AppDomain if
    /// the workflow is in an assembly that has not been loaded yet.
    /// AppDomains are reused if the same assembly is required.
    /// </remarks>
    class AppDomainHost : MarshalByRefObject
    {
        /// <summary>
        /// Last time the AppDomain was touched. Used to measure idle time.
        /// </summary>
        private DateTime lastTimeActive;

        /// <summary>
        /// Reference to the AppDomain
        /// </summary>
        private AppDomain appDomain;

        /// <summary>
        /// Proxy to the workflow host created inside the AppDomain
        /// </summary>
        private SchedulerWorkflowApplicationHost workflowHost;

        /// <summary>
        /// Event handler to forward workflow events to the
        /// main program.
        /// </summary>
        public event EventHandler<WorkflowApplicationHostEventArgs> WorkflowEvent;

        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        /// <summary>
        /// Reference to the workflow event handler
        /// </summary>
        /// <remarks>
        /// This on is necessary to keep the event handler alive
        /// when events are marshaled accross AppDomain boundaries.
        /// </remarks>
        private EventHandler<WorkflowApplicationHostEventArgs> workflowEventHandler;

        /// <summary>
        /// Gets the unique ID of the AppDomain.
        /// </summary>
        public int ID
        {
            get { return appDomain.Id; }
        }

        /// <summary>
        /// Gets the time when the AppDomain appeared active the last time.
        /// </summary>
        public DateTime LastTimeActive
        {
            get { return lastTimeActive; }
        }

        #region Constructors and initializers

        public AppDomainHost(AppDomain ad)
        {
            InitializeMembers();

            this.appDomain = ad;
        }

        private void InitializeMembers()
        {
            this.lastTimeActive = DateTime.MinValue;
            this.appDomain = null;

            this.workflowHost = null;
            this.WorkflowEvent = null;
            this.workflowEventHandler = null;
        }

        /// <summary>
        /// This is to prevent remoting time-outs on event handler call-backs
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion
        #region Host operations

        /// <summary>
        /// Starts a new AppDomain to accept workflow execution requests.
        /// </summary>
        public void Start(Scheduler scheduler, bool interactive)
        {
            Logging.Logger.Instance.LogDebug(
                Logging.EventSource.Scheduler,
                String.Format("Staring new host in AppDomain: {0}", ID));

            lastTimeActive = DateTime.Now;

            // Create the new WorkflowHost inside the new AppDomain and unwrap the proxy
            workflowHost = (SchedulerWorkflowApplicationHost)appDomain.CreateInstanceAndUnwrap(
                typeof(Jhu.Graywulf.Scheduler.SchedulerWorkflowApplicationHost).Assembly.FullName,
                typeof(Jhu.Graywulf.Scheduler.SchedulerWorkflowApplicationHost).FullName);

            // TODO: delete
            // workflowHost.ContextGuid = contextGuid;

            // Now create a reference to the event handler and cache it in
            // workflowEventHandler. This is necessary to keep the delegate alive because
            // workflowHost is a proxy class and events are marshaled accross AppDomain 
            // boundaries.
            workflowEventHandler = new EventHandler<WorkflowApplicationHostEventArgs>(workflowHost_WorkflowEvent);
            workflowHost.WorkflowEvent += workflowEventHandler;

            // Start the new workflow host inside the new AppDomain
            workflowHost.Start(scheduler, interactive);
        }
        
        /// <summary>
        /// Drain-stops the workflows hosted inside the app domain
        /// and unloads the AppDomain itself.
        /// </summary>
        public void Stop(TimeSpan timeout, bool interactive)
        {
            Logging.Logger.Instance.LogDebug(
                Logging.EventSource.Scheduler,
                String.Format("Stopping AppDomain: {0}", ID));

            workflowHost.Stop(timeout);
            workflowHost = null;
        }

        #endregion
        #region Job operations

        /// <summary>
        /// Instructes the workflow host to start a job.
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <returns></returns>
        public Guid PrepareStartJob(Job job)
        {
            lastTimeActive = DateTime.Now;
            return workflowHost.PrepareStartJob(job);
        }

        /// <summary>
        /// Instructes the workflow host to resume a
        /// job that has previously been persisted.
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <returns></returns>
        public Guid PrepareResumeJob(Job job)
        {
            lastTimeActive = DateTime.Now;
            return workflowHost.PrepareResumeJob(job);
        }

        public void RunJob(Job job)
        {
            lastTimeActive = DateTime.Now;
            workflowHost.RunJob(job);

        }

        /// <summary>
        /// Instructs the workflow host to cancel a
        /// running job.
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        public void CancelJob(Job job)
        {
            lastTimeActive = DateTime.Now;
            workflowHost.CancelJob(job);
        }

        /// <summary>
        /// Instructs the workflow host to cancel the job and mark it as timed-out.
        /// </summary>
        /// <param name="job"></param>
        public void TimeOutJob(Job job)
        {
            lastTimeActive = DateTime.Now;
            workflowHost.TimeOutJob(job);
        }

        /// <summary>
        /// Instructs the workflow host to persist and suspend a job.
        /// </summary>
        /// <param name="jobGuid"></param>
        public void PersistJob(Job job)
        {
            lastTimeActive = DateTime.Now;
            workflowHost.PersistJob(job);
        }

        #endregion
        #region Workflow host event forwarder

        /// <summary>
        /// Forwards the workflow events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void workflowHost_WorkflowEvent(object sender, WorkflowApplicationHostEventArgs e)
        {
            lastTimeActive = DateTime.Now;
            WorkflowEvent(this, e);
        }

        private void workflowHost_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            lastTimeActive = DateTime.Now;
            UnhandledException(sender, e);
        }

        #endregion

        private string GetFriendlyName()
        {
            return String.Format("Jhu.Graywulf.Scheduler_{0:yyyyMMdd_HHmmss}", DateTime.Now);
        }
    }
}
