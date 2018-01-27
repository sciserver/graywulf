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

namespace Jhu.Graywulf.Sql.Jobs.Query
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

            using (RegistryContext context = querypartition.Query.CreateContext())
            {
                querypartition.InitializeQueryObject(cancellationContext, context, activityContext.GetExtension<IScheduler>(), true);
                source = querypartition.GetExecuteSourceQuery();
            }

            await querypartition.ExecuteQueryAsync(source);
        }
    }
}
