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
        #region Private member variables

        private JobInfo jobInfo;
        private IJobActivity activity;
        private Guid workflowInstanceId;
        private string activityInstanceId;
        private CodeActivityContext activityContext;
        private Exception exception;

        #endregion
        #region Properties

        public JobInfo JobInfo
        {
            get { return jobInfo; }
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
            : base(outerContext)
        {
            CopyMembers(activity, activityContext);
        }

        internal protected JobContext(LoggingContext outerContext, JobAsyncCodeActivity activity, AsyncCodeActivityContext activityContext)
            : base(outerContext)
        {
            CopyMembers(activity, activityContext);
        }

        internal protected JobContext(LoggingContext outerContext)
            : base(outerContext)
        {
            InitializeMembers(new StreamingContext());
        }

        internal protected JobContext(JobContext outerContext)
            :base(outerContext)
        {
            CopyMembers(outerContext);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.jobInfo = new JobInfo();
            this.activity = null;
            this.workflowInstanceId = Guid.Empty;
            this.activityInstanceId = null;
            this.activityContext = null;
        }

        private void CopyMembers(IJobActivity activity, CodeActivityContext activityContext)
        {
            this.jobInfo = activityContext.GetValue(activity.JobInfo);
            this.activity = activity;
            this.workflowInstanceId = activityContext.WorkflowInstanceId;
            this.activityInstanceId = activityContext.ActivityInstanceId;
            this.activityContext = activityContext;
        }

        private void CopyMembers(JobContext outerContext)
        {
            this.jobInfo = new JobInfo(outerContext.jobInfo);
            this.activity = outerContext.activity;
            this.workflowInstanceId = outerContext.workflowInstanceId;
            this.activityInstanceId = outerContext.activityInstanceId;
            this.activityContext = outerContext.activityContext;
        }

        #region Log event routing

        public override void UpdateEvent(Event e)
        {
            base.UpdateEvent(e);

            e.UserGuid = jobInfo.UserGuid;
            e.JobGuid = jobInfo.JobGuid;
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
            foreach (var e in AsyncEvents)
            {
                var record = new CustomTrackingRecord("asyncEvent");
                record.Data[Constants.ActivityRecordDataItemEvent] = e;

                activityContext.Track(record);
            }

            AsyncEvents.Clear();
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
