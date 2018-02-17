using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class CopySourceTable : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        [RequiredArgument]
        public InArgument<string> RemoteTable { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            SqlQueryPartition querypartition = (SqlQueryPartition)QueryPartition.Get(activityContext);
            string remoteTable = RemoteTable.Get(activityContext);
            SourceQuery source;

            using (RegistryContext context = querypartition.Query.CreateContext())
            {
                querypartition.PrepareCopySourceTable(remoteTable, out source);
            }

            await querypartition.CopySourceTableAsync(remoteTable, source);
        }
    }
}
