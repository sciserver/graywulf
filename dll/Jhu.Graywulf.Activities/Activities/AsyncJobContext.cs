using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.Activities.Tracking;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Activities
{
    public class AsyncJobContext : IDisposable
    {
        #region Singleton

        [ThreadStatic]
        private static AsyncJobContext context;

        public static AsyncJobContext Current
        {
            get
            {
                return context;
            }
            internal set
            {
                context = value;
            }
        }

        #endregion
        #region Private member variables

        private GraywulfAsyncCodeActivity activity;
        private string activityInstanceId;
        private Guid workflowInstanceId;
        private JobContext jobContext;
        private Exception exception;
        private List<CustomTrackingRecord> trackingRecords;

        #endregion
        #region Properties

        internal GraywulfAsyncCodeActivity Activity
        {
            get { return activity; }
        }

        public string ActivityInstanceId
        {
            get { return ActivityInstanceId; }
        }

        public Guid WorkflowInstanceId
        {
            get { return workflowInstanceId; }
        }

        public JobContext JobContext
        {
            get { return jobContext; }
        }

        internal Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        internal List<CustomTrackingRecord> TrackingRecords
        {
            get { return trackingRecords; }
        }

        #endregion

        internal AsyncJobContext(GraywulfAsyncCodeActivity activity, AsyncCodeActivityContext activityContext)
        {
            this.activity = activity;
            this.activityInstanceId = activityContext.ActivityInstanceId;
            this.workflowInstanceId = activityContext.WorkflowInstanceId;

            if (jobContext != null)
            {
                this.jobContext = activityContext.GetValue(activity.JobContext);
            }

            this.trackingRecords = new List<CustomTrackingRecord>();
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Registers a task that have to be canceled when the activity is canceled.
        /// </summary>
        /// <param name="cancelableTask"></param>
        public void RegisterCancelable(ICancelableTask cancelableTask)
        {
            if (!activity.CancelableTasks.TryAdd(activity.GetUniqueActivityID(workflowInstanceId, activityInstanceId), cancelableTask))
            {
                throw new InvalidOperationException(ExceptionMessages.OnlyOneCancelable);
            }
        }

        /// <summary>
        /// Unregisteres a task that no longer needs to be cancelable.
        /// </summary>
        /// <param name="cancelableTask"></param>
        public void UnregisterCancelable(ICancelableTask cancelableTask)
        {
            // TODO: apparently, this gets called more than once sometimes

            ICancelableTask finishedtask;
            if (activity.CancelableTasks.TryRemove(activity.GetUniqueActivityID(workflowInstanceId, activityInstanceId), out finishedtask))
            {
                if (finishedtask != cancelableTask)
                {
                    throw new InvalidOperationException(ExceptionMessages.ObjectsDoNotMatch);
                }
            }
        }

        public void Track(CustomTrackingRecord record)
        {
            trackingRecords.Add(record);
        }
    }
}
