using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Tasks;


namespace Jhu.Graywulf.Activities
{
    public abstract class JobAsyncCodeActivity : AsyncCodeActivity, IJobActivity
    {
        protected delegate void AsyncActivityWorker();
        private delegate object[] AsyncActivityWorkerTask(JobContext jobContext, LoggingContext loggingContext,  AsyncActivityWorker worker);

        #region Private member variables

        private ConcurrentDictionary<string, ICancelableTask> cancelableTasks;

        #endregion
        #region Properties

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
            var jobContext = new JobContext(this, activityContext);
            var loggingContext = new LoggingContext(LoggingContext.Current, true);

            jobContext.UpdateLoggingContext(loggingContext);
            jobContext.Push();
            loggingContext.Push();

            var action = OnBeginExecute(activityContext);

            loggingContext.Pop();
            jobContext.Pop();

            var task = new AsyncActivityWorkerTask(Execute);
            activityContext.UserState = task;

            return task.BeginInvoke(jobContext, loggingContext, action, callback, state);
        }

        protected abstract AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext);

        private object[] Execute(JobContext jobContext, LoggingContext loggingContext, AsyncActivityWorker action)
        {
            Exception exception = null;

            jobContext.Push();
            loggingContext.Push();

            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            loggingContext.Pop();
            jobContext.Pop();

            return new object[] { jobContext, loggingContext, exception };
        }

        protected override void EndExecute(AsyncCodeActivityContext activityContext, IAsyncResult result)
        {
            var task = (AsyncActivityWorkerTask)activityContext.UserState;
            var retpar = task.EndInvoke(result);
            var jobContext = (JobContext)retpar[0];
            var loggingContext = (LoggingContext)retpar[1];
            var ex = (Exception)retpar[2];

            loggingContext.Push();

            // Process tracking records from async call
            loggingContext.ActivityContext = activityContext;
            loggingContext.FlushEvents();

            if (ex != null)
            {
                if (ex is OperationCanceledException ||
                    ex is AggregateException && ((AggregateException)ex).InnerException is OperationCanceledException)
                {
                    // This is a normal way of operation
                }
                else
                {
                    throw ex;
                }
            }

            loggingContext.Pop();
        }

        /// <summary>
        /// Cancels the execution of the activity.
        /// </summary>
        /// <param name="activityContext"></param>
        /// <remarks>
        /// This method cancels all registered running cancelable tasks before marking the
        /// activity canceled.
        /// </remarks>
        protected override void Cancel(AsyncCodeActivityContext activityContext)
        {
            ICancelableTask canceledtask;

            if (cancelableTasks.TryRemove(GetUniqueActivityID(activityContext.WorkflowInstanceId, activityContext.ActivityInstanceId), out canceledtask))
            {
                canceledtask.Cancel();
            }

            // This is absolutely important here:
            if (activityContext.IsCancellationRequested)
            {
                activityContext.MarkCanceled();
            }

            base.Cancel(activityContext);
        }

        internal string GetUniqueActivityID(Guid workflowInstanceGuid, string activityInstanceId)
        {
            return String.Format("{0}_{1}", workflowInstanceGuid.ToString(), activityInstanceId);
        }

        #region Async operation cancellation logic

        /// <summary>
        /// Registers a task that have to be canceled when the activity is canceled.
        /// </summary>
        /// <param name="cancelableTask"></param>
        protected virtual void RegisterCancelable(Guid workflowInstanceId, string activityInstanceId, ICancelableTask cancelableTask)
        {
            if (!cancelableTasks.TryAdd(GetUniqueActivityID(workflowInstanceId, activityInstanceId), cancelableTask))
            {
                throw new InvalidOperationException(ExceptionMessages.OnlyOneCancelable);
            }
        }

        /// <summary>
        /// Unregisteres a task that no longer needs to be cancelable.
        /// </summary>
        /// <param name="cancelableTask"></param>
        protected virtual void UnregisterCancelable(Guid workflowInstanceId, string activityInstanceId, ICancelableTask cancelableTask)
        {
            ICancelableTask finishedtask;
            if (cancelableTasks.TryRemove(GetUniqueActivityID(workflowInstanceId, activityInstanceId), out finishedtask))
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
