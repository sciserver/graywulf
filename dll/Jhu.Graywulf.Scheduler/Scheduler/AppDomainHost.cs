using System;
using System.Collections.Generic;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Keeps track of an AppDomain that can be unloaded when idle.
    /// </summary>
    /// <remarks>
    /// Because the UnhandledEvent exception is thrown from the wrapped
    /// AppDomain, this type needs to be serializable to allow handling
    /// the event outside that AppDomain.
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
        private System.AppDomain appDomain;

        /// <summary>
        /// App domain id
        /// </summary>
        private int id;

        /// <summary>
        /// Proxy to the workflow host created inside the AppDomain
        /// </summary>
        private SchedulerWorkflowApplicationHost workflowHost;

        private Dictionary<Guid, Job> runningJobsByGuid;

        /// <summary>
        /// Event handler to forward workflow events to the main program.
        /// </summary>
        public event EventHandler<WorkflowApplicationHostEventArgs> WorkflowEvent;

        /// <summary>
        /// Event handler for forward app domain events to the main program.
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        /// <summary>
        /// Reference to the workflow event handler
        /// </summary>
        /// <remarks>
        /// This explicit delegate declaration is necessary to keep the event handler alive
        /// when events are marshaled accross AppDomain boundaries.
        /// </remarks>
        private EventHandler<WorkflowApplicationHostEventArgs> workflowEventHandler;

        /// <summary>
        /// Gets the unique ID of the AppDomain.
        /// </summary>
        public int ID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the time when the AppDomain appeared active the last time.
        /// </summary>
        public DateTime LastTimeActive
        {
            get { return lastTimeActive; }
        }

        public Dictionary<Guid, Job> RunningJobs
        {
            get { return runningJobsByGuid; }
        }

        #region Constructors and initializers

        public AppDomainHost(System.AppDomain ad)
        {
            InitializeMembers();

            this.id = ad.Id;
            this.appDomain = ad;
        }

        private void InitializeMembers()
        {
            this.lastTimeActive = DateTime.MinValue;
            this.id = -1;
            this.appDomain = null;

            this.workflowHost = null;
            this.WorkflowEvent = null;
            this.workflowEventHandler = null;
        }

        public override object InitializeLifetimeService()
        {
            // This is necessary
            return null;
        }

        #endregion
        #region Host operations

        private void Touch()
        {
            lastTimeActive = DateTime.Now;
        }

        /// <summary>
        /// Starts a new AppDomain to accept workflow execution requests.
        /// </summary>
        public void Start(Guid guid, Scheduler scheduler, bool interactive)
        {
            Touch();

            QueueManager.Instance.LogDebug("Staring new host in AppDomain: {0}", ID);

            appDomain.UnhandledException += AppDomain_UnhandledException;

            runningJobsByGuid = new Dictionary<Guid, Job>();

            // Create the new WorkflowHost inside the new AppDomain and unwrap the proxy
            workflowHost = (SchedulerWorkflowApplicationHost)appDomain.CreateInstanceAndUnwrap(
                typeof(Jhu.Graywulf.Scheduler.SchedulerWorkflowApplicationHost).Assembly.FullName,
                typeof(Jhu.Graywulf.Scheduler.SchedulerWorkflowApplicationHost).FullName);

            // Now create a reference to the event handler and cache it in
            // workflowEventHandler. This is necessary to keep the delegate alive because
            // workflowHost is a proxy class from the prospective of the remote app domain
            // and events are marshaled accross AppDomain boundaries.

            // This event handler will be fired only if the exception is thrown from a thread
            // which is started inside the domain.

            workflowEventHandler = new EventHandler<WorkflowApplicationHostEventArgs>(WorkflowHost_WorkflowEvent);
            workflowHost.WorkflowEvent += workflowEventHandler;

            // Start the new workflow host inside the new AppDomain
            workflowHost.Start(guid, Logging.LoggingContext.Current.GetLogger(), scheduler, interactive);
        }

        /// <summary>
        /// Drain-stops the workflows hosted inside the app domain
        /// and unloads the AppDomain itself.
        /// </summary>
        public bool TryStop()
        {
            if (runningJobsByGuid.Count == 0 && workflowHost.TryStop())
            {
                Logging.LoggingContext.Current.LogDebug(
                    Logging.EventSource.Scheduler,
                    String.Format("Stopping AppDomain: {0}", ID));

                workflowHost.WorkflowEvent -= workflowEventHandler;
                workflowEventHandler = null;
                workflowHost = null;
                appDomain.UnhandledException -= AppDomain_UnhandledException;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Abort()
        {
            Logging.LoggingContext.Current.LogDebug(
                    Logging.EventSource.Scheduler,
                    String.Format("Aborting AppDomain: {0}", ID));

            workflowHost.Abort();
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
            Touch();
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
            Touch();
            return workflowHost.PrepareResumeJob(job);
        }

        public void RunJob(Job job)
        {
            Touch();
            workflowHost.RunJob(job);
        }

        /// <summary>
        /// Instructs the workflow host to cancel a
        /// running job.
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        public void CancelJob(Job job)
        {
            Touch();
            workflowHost.CancelJob(job);
        }

        /// <summary>
        /// Instructs the workflow host to cancel the job and mark it as timed-out.
        /// </summary>
        /// <param name="job"></param>
        public void TimeOutJob(Job job)
        {
            Touch();
            workflowHost.TimeOutJob(job);
        }

        /// <summary>
        /// Instructs the workflow host to persist and suspend a job.
        /// </summary>
        /// <param name="jobGuid"></param>
        public void PersistJob(Job job)
        {
            Touch();
            workflowHost.PersistJob(job);
        }

        #endregion
        #region Workflow host event forwarders

        /// <summary>
        /// Forwards the workflow events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WorkflowHost_WorkflowEvent(object sender, WorkflowApplicationHostEventArgs e)
        {
            Touch();
            WorkflowEvent(this, e);
        }

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException?.Invoke(this, e);
        }

        #endregion
    }
}
