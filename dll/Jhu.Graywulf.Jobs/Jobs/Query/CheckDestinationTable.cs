using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    public class CheckDestinationTable : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<JobContext> JobContext { get; set; }

        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            SqlQuery query = Query.Get(activityContext);

            using (RegistryContext context = query.CreateContext(this, activityContext))
            {
                query.InitializeQueryObject(context);
                query.Destination.CheckTableExistence();
            }
        }
    }
}
