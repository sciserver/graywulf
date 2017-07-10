using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Activities
{
    public abstract class JobAsyncCodeActivity : AsyncCodeActivity, IJobActivity
    {
        protected delegate void AsyncActivityWorker(JobContext asyncContext);
        private delegate JobContext AsyncActivityWorkerTask(JobContext asyncContext, AsyncActivityWorker worker);

        #region Private member variables

        private ConcurrentDictionary<string, ICancelableTask> cancelableTasks;

        #endregion
        #region Properties

        internal ConcurrentDictionary<string, ICancelableTask> CancelableTasks
        {
            get { return cancelableTasks; }
        }

        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        #endregion
        #region Constructors and initializers

        public JobAsyncCodeActivity()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.cancelableTasks = new ConcurrentDictionary<string, ICancelableTask>();
        }

        #endregion

        protected sealed override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var asyncContext = new JobContext(this, activityContext);
            var action = OnBeginExecute(activityContext);
            var task = new AsyncActivityWorkerTask(Execute);
            activityContext.UserState = task;

            return task.BeginInvoke(asyncContext, action, callback, state);
        }

        protected abstract AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext);

        private JobContext Execute(JobContext context, AsyncActivityWorker action)
        {
            JobContext.Current = context;

            try
            {
                action(context);
            }
            catch (Exception ex)
            {
                context.Exception = ex;
            }

            JobContext.Current = null;

            return context;
        }

        protected override void EndExecute(AsyncCodeActivityContext activityContext, IAsyncResult result)
        {
            var task = (AsyncActivityWorkerTask)activityContext.UserState;
            var gwcx = task.EndInvoke(result);
            var ex = gwcx.Exception;

            // Process tracking records from async call
            foreach (var r in gwcx.TrackingRecords)
            {
                activityContext.Track(r);
            }

            gwcx.Dispose();

            if (ex != null)
            {
                if (ex is AggregateException &&
                   ((AggregateException)gwcx.Exception).InnerException is OperationCanceledException)
                {
                    // This is a normal way of operation
                }
                else
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Cancels the execution of the activity.
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>
        /// This method cancels all registered running cancelable tasks before marking the
        /// activity canceled.
        /// </remarks>
        protected override void Cancel(AsyncCodeActivityContext context)
        {
            ICancelableTask canceledtask;

            if (cancelableTasks.TryRemove(GetUniqueActivityID(context.WorkflowInstanceId, context.ActivityInstanceId), out canceledtask))
            {
                canceledtask.Cancel();
            }

            // This is absolutely important here:
            if (context.IsCancellationRequested)
            {
                context.MarkCanceled();
            }

            base.Cancel(context);
        }

        internal string GetUniqueActivityID(Guid workflowInstanceGuid, string activityInstanceId)
        {
            return String.Format("{0}_{1}", workflowInstanceGuid.ToString(), activityInstanceId);
        }
    }
}
