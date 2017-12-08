using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public class CopyTable : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<CopyTablesParameters> Parameters { get; set; }

        [RequiredArgument]
        public InArgument<CopyTablesItem> Item { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var parameters = Parameters.Get(activityContext);
            var item = Item.Get(activityContext);

            using (var task = item.GetInitializedCopyTableTask(cancellationContext, parameters))
            {
                await task.Value.ExecuteAsync();
            }
        }
    }
}
