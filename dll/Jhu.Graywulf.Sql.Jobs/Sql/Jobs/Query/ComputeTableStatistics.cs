using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class ComputeTableStatistics : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        [RequiredArgument]
        public InArgument<ITableSource> TableSource { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var scheduler = activityContext.GetExtension<IScheduler>();
            var query = Query.Get(activityContext);
            var tableSource = TableSource.Get(activityContext);
            DatasetBase statisticsDataset = null;

            using (RegistryContext registryContext = query.CreateContext())
            {
                query.InitializeQueryObject(cancellationContext, registryContext, scheduler, true);
                statisticsDataset = query.GetStatisticsDataset(tableSource);
            }

            await query.ComputeTableStatisticsAsync(tableSource, statisticsDataset);
        }
    }
}
