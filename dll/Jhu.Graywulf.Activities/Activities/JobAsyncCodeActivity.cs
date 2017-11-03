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
            var task = (Task)result;

            try
            {
                task.Wait();
            }
            catch (OperationCanceledException)
            {
                if (activityContext.IsCancellationRequested)
                {
                    activityContext.MarkCanceled();
                }
                else
                {
                    throw;
                }
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        protected override void Cancel(AsyncCodeActivityContext activityContext)
        {
            var cancellationContext = (CancellationContext)activityContext.UserState;
            cancellationContext.Cancel();
        }

        private async Task ExecuteAsync(AsyncCodeActivityContext activityContext)
        {
            var jobContext = new JobContext(this, activityContext);
            var loggingContext = new LoggingContext(LoggingContext.Current, true);
            
            // Save cancellation context to be called when cancel request arrives
            // from another thread context

            jobContext.UpdateLoggingContext(loggingContext);
            jobContext.Push();
            loggingContext.Push();

            using (var cancellationContext = new CancellationContext())
            {
                activityContext.UserState = cancellationContext;
                await OnExecuteAsync(activityContext, cancellationContext);
            }

            loggingContext.Pop();
            jobContext.Pop();
        }

        protected abstract Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext);
    }
}
