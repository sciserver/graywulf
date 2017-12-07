using System;
using System.Activities;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public class CopyTable : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<CopyTablesParameters> Parameters { get; set; }

        [RequiredArgument]
        public InArgument<CopyTablesItem> Item { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var parameters = Parameters.Get(activityContext);
            var item = Item.Get(activityContext);

            return delegate()
            {
                using (var task = item.GetInitializedCopyTableTask(parameters))
                {
                    RegisterCancelable(workflowInstanceId, activityInstanceId, task.Value);
                    task.Value.Execute();
                    UnregisterCancelable(workflowInstanceId, activityInstanceId, task.Value);
                }
            };
        }
    }
}
