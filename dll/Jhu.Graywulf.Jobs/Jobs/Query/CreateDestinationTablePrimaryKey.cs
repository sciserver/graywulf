using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class CreateDestinationTablePrimaryKey : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var query = Query.Get(activityContext);
            var queryPartition = query.Partitions[0];
            Table destinationTable;

            using (Context context = query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                queryPartition.InitializeQueryObject(context, null, true);
                queryPartition.PrepareCreateDestinationTablePrimaryKey(context, activityContext.GetExtension<IScheduler>(), out destinationTable);
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, queryPartition, destinationTable), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, SqlQueryPartition queryPartition, Table destinationTable)
        {
            RegisterCancelable(workflowInstanceGuid, activityInstanceId, queryPartition);
            queryPartition.CreateDestinationTablePrimaryKey(destinationTable);
            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, queryPartition);
        }
    }
}
