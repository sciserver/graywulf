using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class CopyRemoteTable : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        [RequiredArgument]
        public InArgument<string> RemoteTable { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            SqlQueryPartition querypartition = (SqlQueryPartition)QueryPartition.Get(activityContext);
            TableReference remotetable = null;
            SourceTableQuery source;

            using (RegistryContext context = querypartition.Query.CreateContext())
            {
                remotetable = querypartition.RemoteTableReferences[RemoteTable.Get(activityContext)];
                querypartition.PrepareCopyRemoteTable(remotetable, out source);
            }

            return delegate ()
            {
                RegisterCancelable(workflowInstanceId, activityInstanceId, querypartition);
                querypartition.CopyRemoteTable(remotetable, source);
                UnregisterCancelable(workflowInstanceId, activityInstanceId, querypartition);
            };
        }
    }
}
