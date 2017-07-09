using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    public class CopyResultset : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);

            using (var context = querypartition.Query.CreateContext(this, activityContext))
            {
                querypartition.PrepareCopyResultset(context);
            }

            return delegate(AsyncJobContext asyncContext)
            {
                asyncContext.RegisterCancelable(querypartition);
                querypartition.CopyResultset();
                asyncContext.UnregisterCancelable(querypartition);
            };
        }
    }
}
