using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
    public class GeneratePartitions : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryBase> Query { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            QueryBase query = Query.Get(activityContext);

            using (Context context = query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                int partitionCount;
                Guid assignedServerInstanceGuid;
                
                query.DeterminePartitionCount(context, activityContext.GetExtension<IScheduler>(), out partitionCount, out assignedServerInstanceGuid);
                EntityGuid.Set(activityContext, assignedServerInstanceGuid);

                query.GeneratePartitions(partitionCount);
            }
        }
    }
}
