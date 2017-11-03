using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    public class ExecuteScript : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlScriptParameters> Parameters { get; set; }

        [RequiredArgument]
        public InArgument<DatasetBase> Dataset { get; set; }

        [RequiredArgument]
        public InArgument<string> Script { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var parameters = Parameters.Get(activityContext);
            var dataset = Dataset.Get(activityContext);
            var script = Script.Get(activityContext);

            using (var cn = await dataset.OpenConnectionAsync(cancellationContext.Token))
            {
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = script;
                    cmd.CommandTimeout = parameters.Timeout;
                    cmd.Transaction = cn.BeginTransaction(parameters.IsolationLevel);

                    await cmd.ExecuteNonQueryAsync(cancellationContext.Token);

                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction.Commit();
                    }
                }
            }
        }
    }
}
