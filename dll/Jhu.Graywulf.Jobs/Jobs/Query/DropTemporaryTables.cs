using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    public class DropTemporaryTables : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        [RequiredArgument]
        public InArgument<bool> SuppressErrors { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);

            using (Context context = querypartition.Query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                querypartition.InitializeQueryObject(context);
            }

            var suppressErrors = SuppressErrors.Get(activityContext);

            querypartition.CleanUp(suppressErrors);
        }
    }
}
