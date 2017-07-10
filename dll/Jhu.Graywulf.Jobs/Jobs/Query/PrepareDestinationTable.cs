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
        public InArgument<JobContext> JobContext { get; set; }

        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            SqlQueryPartition queryPartition = QueryPartition.Get(activityContext);
            SqlQuery query = queryPartition.Query;

            using (RegistryContext context = queryPartition.CreateContext(this, activityContext))
            {
                query.InitializeQueryObject(context, null, true);
                queryPartition.PrepareDestinationTable(context, activityContext.GetExtension<IScheduler>());
            }
        }
    }
}
