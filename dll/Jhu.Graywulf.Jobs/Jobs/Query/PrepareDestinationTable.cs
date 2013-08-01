using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    public class PrepareDestinationTable : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryPartitionBase> QueryPartition { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            QueryPartitionBase queryPartition = QueryPartition.Get(activityContext);
            QueryBase query = queryPartition.Query;

            using (Context context = queryPartition.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                query.InitializeQueryObject(context);
                queryPartition.InitializeDestinationTable(context, activityContext.GetExtension<IScheduler>());
            }
        }
    }
}
