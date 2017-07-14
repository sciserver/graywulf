using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class ExecuteQuery : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);
            SourceTableQuery source;
            Table destination;

            using (RegistryContext context = querypartition.Query.CreateContext())
            {
                querypartition.PrepareExecuteQuery(context, activityContext.GetExtension<IScheduler>(), out source, out destination);
            }

            return delegate ()
            {
                RegisterCancelable(workflowInstanceId, activityInstanceId, querypartition);
                querypartition.ExecuteQuery(source, destination);
                UnregisterCancelable(workflowInstanceId, activityInstanceId, querypartition);
            };
        }
    }
}
