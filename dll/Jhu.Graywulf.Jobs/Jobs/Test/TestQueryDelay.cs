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
    public class TestQueryDelay : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<int> DelayPeriod { get; set; }

        [RequiredArgument]
        public InArgument<int> QueryTimeout { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var delay = DelayPeriod.Get(activityContext);
            var queryTimeout = QueryTimeout.Get(activityContext);

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, delay, queryTimeout), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, int period, int queryTimeout)
        {
            var sql = String.Format("WAITFOR DELAY '{0:mm\\:ss}'", TimeSpan.FromMilliseconds(period));

            using (var cn = new SqlConnection("Data Source=localhost;Integrated Security=true"))
            {
                cn.Open();
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandTimeout = queryTimeout;

                    var ccmd = new CancelableDbCommand(cmd);
                    RegisterCancelable(workflowInstanceGuid, activityInstanceId, ccmd);

                    try
                    {
                        ccmd.ExecuteNonQuery();
                    }
                    finally
                    {
                        UnregisterCancelable(workflowInstanceGuid, activityInstanceId, ccmd);
                    }
                }
            }
        }

    }
}
