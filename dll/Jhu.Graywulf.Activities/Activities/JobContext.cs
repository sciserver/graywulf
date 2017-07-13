using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Activities;
using System.Activities.Tracking;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Activities
{
    public class JobContext : LoggingContext
    {
        #region Singleton access

        public static new JobContext Current
        {
            get
            {
                return LoggingContext.Current as JobContext;
            }
        }

        #endregion
        #region Private member variables

        private Guid sessionGuid;
        private Guid clusterGuid;
        private Guid domainGuid;
        private Guid federationGuid;
        private Guid jobGuid;
        private Guid userGuid;
        private string userName;
        private string jobID;

        private IJobActivity activity;
        private Guid workflowInstanceId;
        private string activityInstanceId;
        private CodeActivityContext activityContext;
        private Exception exception;

        #endregion
        #region Properties

        public Guid SessionGuid
        {
            get { return sessionGuid; }
            set { sessionGuid = value; }
        }

        public Guid ClusterGuid
        {
            get { return clusterGuid; }
            set { clusterGuid = value; }
        }

        public Guid DomainGuid
        {
            get { return domainGuid; }
            set { domainGuid = value; }
        }

        public Guid FederationGuid
        {
            get { return federationGuid; }
            set { federationGuid = value; }
        }

        public Guid JobGuid
        {
            get { return jobGuid; }
            set { jobGuid = value; }
        }

        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string JobID
        {
            get { return jobID; }
            set { jobID = value; }
        }

        public Guid WorkflowInstanceId
        {
            get { return workflowInstanceId; }
        }

        public string ActivityInstanceId
        {
            get { return ActivityInstanceId; }
        }

        internal CodeActivityContext ActivityContext
        {
            get { return activityContext; }
            set { activityContext = value; }
        }

        internal Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        #endregion

        internal protected JobContext(LoggingContext outerContext, JobCodeActivity activity, CodeActivityContext activityContext)
            : base(outerContext, false)
        {
            CopyMembers(activity, activityContext);
        }

        internal protected JobContext(LoggingContext outerContext, JobAsyncCodeActivity activity, AsyncCodeActivityContext activityContext)
            : base(outerContext, true)
        {
            CopyMembers(activity, activityContext);
        }

        internal protected JobContext(LoggingContext outerContext)
            : base(outerContext)
        {
            InitializeMembers(new StreamingContext());
        }

        internal protected JobContext(JobContext outerContext)
            : base(outerContext)
        {
            CopyMembers(outerContext);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.sessionGuid = Guid.Empty;
            this.clusterGuid = Guid.Empty;
            this.domainGuid = Guid.Empty;
            this.federationGuid = Guid.Empty;
            this.jobGuid = Guid.Empty;
            this.userGuid = Guid.Empty;
            this.userName = null;
            this.jobID = null;

            this.activity = null;
            this.workflowInstanceId = Guid.Empty;
            this.activityInstanceId = null;
            this.activityContext = null;
        }

        private void CopyMembers(IJobActivity activity, CodeActivityContext activityContext)
        {
            var jobInfo = activityContext.GetValue(activity.JobInfo);

            this.sessionGuid = Guid.Empty;
            this.clusterGuid = jobInfo.ClusterGuid;
            this.domainGuid = jobInfo.DomainGuid;
            this.federationGuid = jobInfo.FederationGuid;
            this.jobGuid = jobInfo.JobGuid;
            this.userGuid = jobInfo.UserGuid;
            this.userName = jobInfo.UserName;
            this.jobID = jobInfo.JobID;

            this.activity = activity;
            this.workflowInstanceId = activityContext.WorkflowInstanceId;
            this.activityInstanceId = activityContext.ActivityInstanceId;
            this.activityContext = activityContext;
        }

        private void CopyMembers(JobContext outerContext)
        {
            this.sessionGuid = outerContext.sessionGuid;
            this.clusterGuid = outerContext.clusterGuid;
            this.domainGuid = outerContext.domainGuid;
            this.federationGuid = outerContext.federationGuid;
            this.jobGuid = outerContext.JobGuid;
            this.userGuid = outerContext.userGuid;
            this.userName = outerContext.userName;
            this.jobID = outerContext.jobID;

            this.activity = outerContext.activity;
            this.workflowInstanceId = outerContext.workflowInstanceId;
            this.activityInstanceId = outerContext.activityInstanceId;
            this.activityContext = outerContext.activityContext;
        }

        #region Log event routing

        public override void UpdateEvent(Event e)
        {
            base.UpdateEvent(e);

            e.SessionGuid = this.sessionGuid;
            e.UserGuid = this.userGuid;
            e.JobGuid = this.jobGuid;
        }

        public override void RecordEvent(Event e)
        {
            if (activityContext != null && !IsAsync)
            {
                // This is a synchronous event called from a simple CodeActivity
                // Route event through the workflow tracking infrastructure
                var ctr = new CustomTrackingRecord("Graywulf log event");
                ctr.Data.Add("Event", e);
                activityContext.Track(ctr);
            }
            else
            {
                // This is an event occuring outside a workflow or within
                // the async thread of an AsyncCodeActiviry
                base.RecordEvent(e);
            }
        }

        public override void FlushEvents()
        {
            if (IsAsync && AsyncEvents != null)
            {
                foreach (var e in AsyncEvents)
                {
                    var record = new CustomTrackingRecord("asyncEvent");
                    record.Data[Constants.ActivityRecordDataItemEvent] = e;

                    activityContext.Track(record);
                }

                AsyncEvents.Clear();
            }
        }

        #endregion
        #region Async operation cancellation logic

        /// <summary>
        /// Registers a task that have to be canceled when the activity is canceled.
        /// </summary>
        /// <param name="cancelableTask"></param>
        public virtual void RegisterCancelable(ICancelableTask cancelableTask)
        {
            if (activity == null)
            {
                throw new InvalidOperationException();
            }

            var ac = (JobAsyncCodeActivity)activity;

            if (!ac.CancelableTasks.TryAdd(ac.GetUniqueActivityID(workflowInstanceId, activityInstanceId), cancelableTask))
            {
                throw new InvalidOperationException(ExceptionMessages.OnlyOneCancelable);
            }
        }

        /// <summary>
        /// Unregisteres a task that no longer needs to be cancelable.
        /// </summary>
        /// <param name="cancelableTask"></param>
        public virtual void UnregisterCancelable(ICancelableTask cancelableTask)
        {
            if (activity == null)
            {
                throw new InvalidOperationException();
            }

            // TODO: apparently, this gets called more than once sometimes
            var ac = (JobAsyncCodeActivity)activity;

            ICancelableTask finishedtask;
            if (ac.CancelableTasks.TryRemove(ac.GetUniqueActivityID(workflowInstanceId, activityInstanceId), out finishedtask))
            {
                if (finishedtask != cancelableTask)
                {
                    throw new InvalidOperationException(ExceptionMessages.ObjectsDoNotMatch);
                }
            }
        }

        #endregion
    }
}
