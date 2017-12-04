using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Tasks;


namespace Jhu.Graywulf.Activities
{
    public abstract class JobAsyncCodeActivity : AsyncCodeActivity, IJobActivity
    {
        #region Properties

        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        #endregion

        protected sealed override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var activityState = new JobAsyncCodeActivityState();
            activityContext.UserState = activityState;

            var task = ExecuteAsync(activityContext);
            var tcs = new TaskCompletionSource<object>(state);

            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(null);
                }

                callback?.Invoke(tcs.Task);
            });

            return tcs.Task;
        }

        protected sealed override void EndExecute(AsyncCodeActivityContext activityContext, IAsyncResult result)
        {
            var state = (JobAsyncCodeActivityState)activityContext.UserState;
            var task = (Task)result;

            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                var helper = new Util.CancellationHelper(ex);

                if (activityContext.IsCancellationRequested && helper.IsCancelled)
                {
                    activityContext.MarkCanceled();
                }
                else
                {
                    throw helper.DispatchException();
                }
            }
            finally
            {
                lock (state.SyncRoot)
                {
                    state.Dispose();
                }
            }
        }

        protected override void Cancel(AsyncCodeActivityContext activityContext)
        {
            // Any exceptions thrown from this method are fatal to the workflow instance. 
            // This call can happen on a thread concurrent to OnExecuteAsync
            var state = (JobAsyncCodeActivityState)activityContext.UserState;
            var jobContext = new JobContext(this, activityContext);

            using (new LoggingContext(true))
            {
                // Context becomes invalid once the activity has completed but cancel
                // can be called after ExecuteAsync
                if (state.CancellationContext.IsValid && !state.CancellationContext.IsRequested)
                {
                    lock (state.SyncRoot)
                    {
                        state.CancellationContext.Cancel();
                    }
                }
            }
        }

        private async Task ExecuteAsync(AsyncCodeActivityContext activityContext)
        {
            var state = (JobAsyncCodeActivityState)activityContext.UserState;
            var jobContext = new JobContext(this, activityContext);

            using (new LoggingContext(true))
            {
                // Save cancellation context to be called when cancel request arrives
                // from another thread context

                jobContext.UpdateLoggingContext(LoggingContext.Current);
                jobContext.Push();

                await OnExecuteAsync(activityContext, state.CancellationContext);
            }
        }

        protected abstract Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext);


    }
}
