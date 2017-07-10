using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
    public class GeneratePartitions : JobCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            SqlQuery query = Query.Get(activityContext);

            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    query.InitializeQueryObject(null);
                    query.GeneratePartitions();
                    break;
                case ExecutionMode.Graywulf:
                    using (RegistryContext context = query.CreateContext(this, activityContext))
                    {
                        var scheduler = activityContext.GetExtension<IScheduler>();

                        query.InitializeQueryObject(context, scheduler, false);
                        query.GeneratePartitions();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
