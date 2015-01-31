using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Activities
{
    public abstract class GraywulfAsyncCodeActivity : AsyncCodeActivity
    {
        // Holds a list of tasks that can be cancelled
        private Dictionary<string, ICancelableTask> cancelableTasks;

        #region Constructors and initializers

        public GraywulfAsyncCodeActivity()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();
        }

        #endregion

        protected Task EnqueueAsync(Action<object> action, AsyncCallback callback, object state)
        {
            Task task = Task.Factory.StartNew(action, state);
            task.ContinueWith(res => callback(task));
            return task;
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            try
            {
                ((Task)result).Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is OperationCanceledException)
                {
                    // This is a normal way of operation
                }
                else
                {
                    throw ex.InnerException;
                }
            }
        }

        /// <summary>
        /// Registers a task that have to be canceled when the activity is canceled.
        /// </summary>
        /// <param name="workflowInstanceGuid"></param>
        /// <param name="activityInstanceId"></param>
        /// <param name="cancelableTask"></param>
        protected void RegisterCancelable(Guid workflowInstanceGuid, string activityInstanceId, ICancelableTask cancelableTask)
        {
            if (cancelableTasks.ContainsKey(activityInstanceId))
            {
                throw new InvalidOperationException(ExceptionMessages.OnlyOneCancelable);
            }

            cancelableTasks.Add(String.Format("{0}_{1}", workflowInstanceGuid.ToString(), activityInstanceId), cancelableTask);
        }

        /// <summary>
        /// Unregisteres a task that no longer needs to be cancelable.
        /// </summary>
        /// <param name="workflowInstanceGuid"></param>
        /// <param name="activityInstanceId"></param>
        /// <param name="cancelableTask"></param>
        protected void UnregisterCancelable(Guid workflowInstanceGuid, string activityInstanceId, ICancelableTask cancelableTask)
        {
            string id = String.Format("{0}_{1}", workflowInstanceGuid.ToString(), activityInstanceId);

            if (cancelableTasks[id] != cancelableTask)
            {
                throw new ArgumentException(ExceptionMessages.ObjectsDoNotMatch, "cancelableObject");
            }

            cancelableTasks.Remove(id);
        }

        /// <summary>
        /// Cancels the execution of the activity.
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>
        /// This method cancels all registered runnin cancelable tasks before marking the
        /// activity canceled.
        /// </remarks>
        protected override void Cancel(AsyncCodeActivityContext context)
        {
            string id = String.Format("{0}_{1}", context.WorkflowInstanceId.ToString(), context.ActivityInstanceId);

            if (cancelableTasks.ContainsKey(id))
            {
                cancelableTasks[id].Cancel();
            }

            // This is absolutely important here:
            if (context.IsCancellationRequested)
            {
                context.MarkCanceled();
            }

            base.Cancel(context);
        }


    }
}
