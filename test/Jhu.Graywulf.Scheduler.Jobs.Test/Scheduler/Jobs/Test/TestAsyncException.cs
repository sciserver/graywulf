using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Scheduler.Jobs.Test
{
    public class TestAsyncException : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<string> Message { get; set; }

        protected override Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            string message = activityContext.GetValue(Message);
            throw new Exception(message);
        }
    }
}
