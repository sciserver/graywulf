using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class InitializeQuery : JobCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            SqlQuery query = Query.Get(activityContext);

            // Single server mode will run on one partition by definition,
            // Graywulf mode has to look at the registry for available machines
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    query.InitializeQueryObject(null, null);
                    break;
                case ExecutionMode.Graywulf:
                    using (RegistryContext registryContext = ContextManager.Instance.CreateReadOnlyContext())
                    {
                        query.InitializeQueryObject(registryContext);
                        query.Validate();

                        // TODO: add this back
                        query.IdentifyTablesForStatistics();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
