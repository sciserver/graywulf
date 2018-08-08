using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class IdentifyRemoteSourceTables : JobCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);

            using (RegistryContext registryContext = querypartition.Query.CreateContext())
            {
                querypartition.InitializeQueryObject(registryContext);
                querypartition.IdentifyRemoteSourceTables();
                querypartition.GenerateRemoteSourceTableQueries(ColumnContext.AllReferenced, 0);
            }
        }
    }
}
