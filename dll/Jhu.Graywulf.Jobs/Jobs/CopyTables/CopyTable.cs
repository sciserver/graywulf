using System;
using System.Activities;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public class CopyTable : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<JobContext> JobContext { get; set; }

        [RequiredArgument]
        public InArgument<CopyTablesParameters> Parameters { get; set; }

        [RequiredArgument]
        public InArgument<CopyTablesItem> Item { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var parameters = Parameters.Get(activityContext);
            var item = Item.Get(activityContext);

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, parameters, item), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, CopyTablesParameters parameters, CopyTablesItem item)
        {
            var task = item.GetInitializedCopyTableTask(parameters);

            RegisterCancelable(workflowInstanceGuid, activityInstanceId, task);

            task.Execute();

            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, task);
        }
    }
}
