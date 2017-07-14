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
    public class CopyResultset : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);

            using (var context = querypartition.Query.CreateContext())
            {
                querypartition.PrepareCopyResultset(context);
            }

            return delegate()
            {
                RegisterCancelable(workflowInstanceId, activityInstanceId, querypartition);
                querypartition.CopyResultset();
                UnregisterCancelable(workflowInstanceId, activityInstanceId, querypartition);
            };
        }
    }
}
