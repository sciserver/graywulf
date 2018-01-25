using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class GeneratePartitions : JobCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            SqlQuery query = Query.Get(activityContext);

            switch (query.Parameters.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    query.InitializeQueryObject(null, null);
                    query.GeneratePartitions();
                    break;
                case ExecutionMode.Graywulf:
                    using (RegistryContext registryContext = query.CreateContext())
                    {
                        var scheduler = activityContext.GetExtension<IScheduler>();
                        query.InitializeQueryObject(null, registryContext, scheduler, false);
                        query.GeneratePartitions();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
