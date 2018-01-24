using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class CopyResultset : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            SqlQueryPartition querypartition = QueryPartition.Get(activityContext);

            using (var registryContext = querypartition.Query.CreateContext())
            {
                querypartition.InitializeQueryObject(cancellationContext, registryContext);
            }
            
            await querypartition.CopyResultsetAsync();
        }
    }
}
