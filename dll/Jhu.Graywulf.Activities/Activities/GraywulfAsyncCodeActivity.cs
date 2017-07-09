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
    public abstract class GraywulfAsyncCodeActivity : AsyncCodeActivity, IGraywulfActivity
    {
        protected delegate void AsyncActivityWorker(AsyncJobContext asyncContext);
        private delegate AsyncJobContext AsyncActivityWorkerTask(AsyncJobContext asyncContext, AsyncActivityWorker worker);

        #region Private member variables

        private ConcurrentDictionary<string, ICancelableTask> cancelableTasks;

        #endregion
        #region Properties

        internal ConcurrentDictionary<string, ICancelableTask> CancelableTasks
        {
            get { return cancelableTasks; }
        }

        public InArgument<JobContext> JobContext { get; set; }

        #endregion
        #region Constructors and initializers

        public GraywulfAsyncCodeActivity()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.cancelableTasks = new ConcurrentDictionary<string, ICancelableTask>();
        }

        #endregion

        protected abstract AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext);

        protected sealed override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var asyncContext = new AsyncJobContext(this, activityContext);
            var action = OnBeginExecute(activityContext);
            var task = new AsyncActivityWorkerTask(Execute);
            activityContext.UserState = task;

            return task.BeginInvoke(asyncContext, action, callback, state);
        }

        private AsyncJobContext Execute(AsyncJobContext context, AsyncActivityWorker action)
        {
            AsyncJobContext.Current = context;

            try
            {
                action(context);
            }
            catch (Exception ex)
            {
                context.Exception = ex;
            }

            AsyncJobContext.Current = null;

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
