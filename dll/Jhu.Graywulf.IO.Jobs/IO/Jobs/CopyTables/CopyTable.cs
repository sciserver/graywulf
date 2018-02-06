using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO.Jobs.CopyTables
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

            // Get server name from the data source
            // This will be the database server responsible for executing the table copy
            var host = ((Jhu.Graywulf.Sql.Schema.SqlServer.SqlServerDataset)item.Source.Dataset).HostName;

            using (var task = RemoteServiceHelper.CreateObject<ICopyTable>(cancellationContext, host, true))
            {
                var settings = new TableCopySettings()
                {
                    Timeout = parameters.Timeout
                };

                await task.Value.ExecuteAsync(item.Source, item.Destination, settings);
            }
        }
    }
}
