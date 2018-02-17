using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class DropTemporaryTables : JobCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        [RequiredArgument]
        public InArgument<bool> SuppressErrors { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);

            var suppressErrors = SuppressErrors.Get(activityContext);
            querypartition.CleanUp(suppressErrors);
        }
    }
}
