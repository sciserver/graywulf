using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class CreateDestinationTablePrimaryKey : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var query = Query.Get(activityContext);
            var queryPartition = query.Partitions[0];
            Table destinationTable;

            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    throw new NotImplementedException();
                case ExecutionMode.Graywulf:
                    using (RegistryContext registryContext = query.CreateContext())
                    {
                        queryPartition.InitializeQueryObject(cancellationContext, registryContext, activityContext.GetExtension<IScheduler>(), true);
                        destinationTable = await queryPartition.PrepareCreateDestinationTablePrimaryKeyAsync();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            await queryPartition.CreateDestinationTablePrimaryKeyAsync(destinationTable);
        }
    }
}
