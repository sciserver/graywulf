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
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
    public class ComputeTableStatistics : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        [RequiredArgument]
        public InArgument<ITableSource> TableSource { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var scheduler = activityContext.GetExtension<IScheduler>();
            var query = Query.Get(activityContext);
            var tableSource = TableSource.Get(activityContext);
            DatasetBase statisticsDataset = null;

            using (RegistryContext context = query.CreateContext())
            {
                query.InitializeQueryObject(context, scheduler, true);
                statisticsDataset = query.GetStatisticsDataset(tableSource);
            }

            return delegate(JobContext asyncContext)
            {
                asyncContext.RegisterCancelable(query);
                query.ComputeTableStatistics(tableSource, statisticsDataset);
                asyncContext.UnregisterCancelable(query);
            };
        }
    }
}
