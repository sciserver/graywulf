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
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class CopyRemoteTable : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }
        [RequiredArgument]
        public InArgument<string> RemoteTable { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            SqlQueryPartition querypartition = (SqlQueryPartition)QueryPartition.Get(activityContext);
            
            TableReference remotetable = null;
            SourceTableQuery source;

            using (Context context = querypartition.Query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {       
                remotetable = querypartition.RemoteTableReferences[RemoteTable.Get(activityContext)];
                querypartition.PrepareCopyRemoteTable(remotetable, out source);
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, querypartition, remotetable, source), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, SqlQueryPartition querypartition, TableReference remotetable, SourceTableQuery source)
        {
            RegisterCancelable(workflowInstanceGuid, activityInstanceId, querypartition);
            querypartition.CopyRemoteTable(remotetable, source);
            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, querypartition);
        }
    }
}
