using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.Activities.Tracking;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Scheduler.Jobs.Test
{
    public class TestAsyncTrackingRecord : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<string> Message { get; set; }

        protected override Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            string message = activityContext.GetValue(Message);

            Logging.LoggingContext.Current.LogStatus(Logging.EventSource.Test, "Test event from async activitity.");

            return Task.CompletedTask;
        }
    }
}
