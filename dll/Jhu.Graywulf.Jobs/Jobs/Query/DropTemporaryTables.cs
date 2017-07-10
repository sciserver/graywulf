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
        public InArgument<JobInfo> JobInfo { get; set; }

        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        [RequiredArgument]
        public InArgument<bool> SuppressErrors { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);

            using (RegistryContext context = querypartition.Query.CreateContext(this, activityContext))
            {
                querypartition.InitializeQueryObject(context);
            }

            var suppressErrors = SuppressErrors.Get(activityContext);

            querypartition.CleanUp(suppressErrors);
        }
    }
}
