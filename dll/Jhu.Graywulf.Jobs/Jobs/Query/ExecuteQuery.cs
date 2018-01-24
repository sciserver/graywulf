using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class ExecuteQuery : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);
            SourceTableQuery source;
            Table destination;

            using (RegistryContext context = querypartition.Query.CreateContext())
            {
                querypartition.InitializeQueryObject(cancellationContext, context, activityContext.GetExtension<IScheduler>(), true);

                // Destination table
                switch (querypartition.Query.ExecutionMode)
                {
                    case ExecutionMode.SingleServer:
                        // In single-server mode results are directly written into destination table
                        destination = querypartition.Query.Destination.GetTable();
                        break;
                    case ExecutionMode.Graywulf:
                        // In graywulf mode results are written into a temporary table first
                        destination = querypartition.GetOutputTable();
                        querypartition.TemporaryTables.TryAdd(destination.TableName, destination);

                        // Drop destination table, in case it already exists for some reason
                        destination.Drop();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                source = querypartition.GetExecuteSourceQuery();
            }

            await querypartition.ExecuteQueryAsync(source, destination);
        }
    }
}
