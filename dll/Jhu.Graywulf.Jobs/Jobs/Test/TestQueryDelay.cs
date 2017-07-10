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
    public class TestQueryDelay : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<int> DelayPeriod { get; set; }

        [RequiredArgument]
        public InArgument<int> QueryTimeout { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var delay = DelayPeriod.Get(activityContext);
            var queryTimeout = QueryTimeout.Get(activityContext);

            return delegate (JobContext asyncContext)
            {
                var sql = String.Format("WAITFOR DELAY '{0:mm\\:ss}'", TimeSpan.FromMilliseconds(delay));

                using (var cn = new SqlConnection("Data Source=localhost;Integrated Security=true"))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.CommandTimeout = queryTimeout;

                        var ccmd = new CancelableDbCommand(cmd);
                        asyncContext.RegisterCancelable(ccmd);

                        try
                        {
                            ccmd.ExecuteNonQuery();
                        }
                        finally
                        {
                            asyncContext.UnregisterCancelable(ccmd);
                        }
                    }
                }
            };
        }
    }
}
