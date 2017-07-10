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

namespace Jhu.Graywulf.Jobs.Test
{
    public class TestAsyncException : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<string> Message { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            string message = activityContext.GetValue(Message);

            return delegate (JobContext asyncContext)
            {
                throw new Exception(message);
            };
        }
    }
}
