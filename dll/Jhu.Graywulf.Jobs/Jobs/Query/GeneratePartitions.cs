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
        public InArgument<SqlQuery> Query { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            SqlQuery query = Query.Get(activityContext);

            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    query.InitializeQueryObject(null);
                    query.GeneratePartitions();
                    break;
                case ExecutionMode.Graywulf:
                    using (Context context = query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
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
