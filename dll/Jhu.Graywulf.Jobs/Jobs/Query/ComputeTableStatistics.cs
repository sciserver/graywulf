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
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
    public class ComputeTableStatistics : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        [RequiredArgument]
        public InArgument<ITableSource> TableSource { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var query = Query.Get(activityContext);
            var tableSource = TableSource.Get(activityContext);
            DatasetBase statisticsDataset = null;

            using (Context context = query.CreateContext(this, activityContext))
            {
                query.InitializeQueryObject(context, activityContext.GetExtension<IScheduler>(), true);
                statisticsDataset = query.GetStatisticsDataset(tableSource);
            }

            return delegate(AsyncJobContext asyncContext)
            {
                asyncContext.RegisterCancelable(query);
                query.ComputeTableStatistics(tableSource, statisticsDataset);
                asyncContext.UnregisterCancelable(query);
            };
        }
    }
}
