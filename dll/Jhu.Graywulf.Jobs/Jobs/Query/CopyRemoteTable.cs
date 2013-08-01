using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Jobs.Query
{
    public class CopyRemoteTable : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryPartitionBase> QueryPartition { get; set; }
        [RequiredArgument]
        public InArgument<string> RemoteTable { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            //QueryPartitionBase querypartition = (QueryPartitionBase)QueryPartition.Get(activityContext).Clone();
            QueryPartitionBase querypartition = (QueryPartitionBase)QueryPartition.Get(activityContext);
            
            TableReference remotetable = null;
            SourceQueryParameters source;

            using (Context context = querypartition.Query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                //querypartition.InitializeQueryObject(context);
                
                remotetable = querypartition.RemoteTableReferences[RemoteTable.Get(activityContext)];
                source = querypartition.PrepareCopyRemoteTable(remotetable);
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, querypartition, remotetable, source), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, QueryPartitionBase querypartition, TableReference remotetable, SourceQueryParameters source)
        {
            RegisterCancelable(workflowInstanceGuid, activityInstanceId, querypartition);
            querypartition.CopyRemoteTable(remotetable, source);
            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, querypartition);
        }
    }
}
