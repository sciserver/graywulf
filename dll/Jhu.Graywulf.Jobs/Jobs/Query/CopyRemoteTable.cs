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
    public class CopyRemoteTable : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        [RequiredArgument]
        public InArgument<string> RemoteTable { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            SqlQueryPartition querypartition = (SqlQueryPartition)QueryPartition.Get(activityContext);
            TableReference remotetable = null;
            SourceTableQuery source;

            using (RegistryContext context = querypartition.Query.CreateContext())
            {
                remotetable = querypartition.RemoteTableReferences[RemoteTable.Get(activityContext)];
                querypartition.PrepareCopyRemoteTable(remotetable, out source);
            }

            return delegate (JobContext asyncContext)
            {
                asyncContext.RegisterCancelable(querypartition);
                querypartition.CopyRemoteTable(remotetable, source);
                asyncContext.UnregisterCancelable(querypartition);
            };
        }
    }
}
