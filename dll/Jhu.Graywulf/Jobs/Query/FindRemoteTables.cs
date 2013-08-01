using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    public class FindRemoteTables : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryPartitionBase> QueryPartition { get; set; }
        [RequiredArgument]
        public OutArgument<List<string>> RemoteTableReferences { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            QueryPartitionBase querypartition = QueryPartition.Get(activityContext);
            List<string> remotetablereferences;

            using (Context context = querypartition.Query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                querypartition.InitializeQueryObject(context);
                remotetablereferences = new List<string>(querypartition.FindRemoteTableReferences().Select(x => x.FullyQualifiedName));
                RemoteTableReferences.Set(activityContext, remotetablereferences);
            }
        }
    }
}
