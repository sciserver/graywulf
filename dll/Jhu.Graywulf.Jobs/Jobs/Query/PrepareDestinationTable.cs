using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;

namespace Jhu.Graywulf.Jobs.Query
{
    public class PrepareDestinationTable : JobCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            SqlQueryPartition queryPartition = QueryPartition.Get(activityContext);
            SqlQuery query = queryPartition.Query;

            using (RegistryContext context = queryPartition.CreateContext())
            {
                query.InitializeQueryObject(context, null, true);
                queryPartition.PrepareDestinationTable(context, activityContext.GetExtension<IScheduler>());
            }
        }
    }
}
