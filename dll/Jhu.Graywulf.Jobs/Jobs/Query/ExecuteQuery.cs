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
        public InArgument<JobContext> JobContext { get; set; }

        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);
            SourceTableQuery source;
            Table destination;

            using (Context context = querypartition.Query.CreateContext(this, activityContext))
            {
                querypartition.PrepareExecuteQuery(context, activityContext.GetExtension<IScheduler>(), out source, out destination);
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, querypartition, source, destination), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, SqlQueryPartition querypartition, SourceTableQuery source, Table destination)
        {
            RegisterCancelable(workflowInstanceGuid, activityInstanceId, querypartition);
            querypartition.ExecuteQuery(source, destination);
            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, querypartition);
        }
    }
}
