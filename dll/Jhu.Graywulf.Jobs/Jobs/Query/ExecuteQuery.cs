using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class ExecuteQuery : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);
            SourceTableQuery source;
            Table destination;

            using (Context context = querypartition.Query.CreateContext(this, activityContext))
            {
                querypartition.PrepareExecuteQuery(context, activityContext.GetExtension<IScheduler>(), out source, out destination);
            }

            return delegate (AsyncJobContext asyncContext)
            {
                asyncContext.RegisterCancelable(querypartition);
                querypartition.ExecuteQuery(source, destination);
                asyncContext.UnregisterCancelable(querypartition);
            };
        }
    }
}
