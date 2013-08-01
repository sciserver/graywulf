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
    public class ExecuteQuery : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryPartitionBase> QueryPartition { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            QueryPartitionBase querypartition = QueryPartition.Get(activityContext);

            using (Context context = querypartition.Query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                querypartition.PrepareExecuteQuery(context, activityContext.GetExtension<IScheduler>());
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, querypartition), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, QueryPartitionBase querypartition)
        {
            RegisterCancelable(workflowInstanceGuid, activityInstanceId, querypartition);
            querypartition.ExecuteQuery();
            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, querypartition);
        }
    }
}
