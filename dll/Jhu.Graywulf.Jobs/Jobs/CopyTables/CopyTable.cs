using System;
using System.Activities;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public class CopyTable : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<CopyTablesParameters> Parameters { get; set; }

        [RequiredArgument]
        public InArgument<CopyTablesItem> Item { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var parameters = Parameters.Get(activityContext);
            var item = Item.Get(activityContext);

            return delegate (AsyncJobContext asyncContext)
            {
                var task = item.GetInitializedCopyTableTask(parameters);
                asyncContext.RegisterCancelable(task);
                task.Execute();
                asyncContext.UnregisterCancelable(task);
            };
        }
    }
}
