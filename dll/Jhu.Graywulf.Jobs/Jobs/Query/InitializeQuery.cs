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
    public class InitializeQuery : CodeActivity, IGraywulfActivity
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

            // Single server mode will run on one partition by definition,
            // Graywulf mode has to look at the registry for available machines
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    query.InitializeQueryObject(null);
                    break;
                case ExecutionMode.Graywulf:
                    using (Context context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                    {
                        query.InitializeQueryObject(context);
                        query.CollectTablesForStatistics();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
