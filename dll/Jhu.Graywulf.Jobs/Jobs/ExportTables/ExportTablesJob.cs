using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public class ExportTablesJob : JobAsyncCodeActivity, IJobActivity, IExportTablesJob
    {
        [RequiredArgument]
        public InArgument<ExportTablesParameters> Parameters { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var parameters = Parameters.Get(activityContext);

            // Make sure connection strings are cached
            using (var context = ContextManager.Instance.CreateReadOnlyContext())
            {
                for (int i = 0; i < parameters.Sources.Length; i++)
                {
                    if (parameters.Sources[i].Dataset is GraywulfDataset)
                    {
                        // Load database instace and get connection string to make it cached

                        var gwds = (GraywulfDataset)parameters.Sources[i].Dataset;
                        gwds.RegistryContext = context;
                        gwds.CacheSchemaConnectionString();
                    }
                }
            }

            // Create table exporter
            var exporter = parameters.GetInitializedTableExportTask(cancellationContext);
            
            try
            {
                await exporter.OpenAsync();
                await exporter.ExecuteAsync();
            }
            finally
            {
                exporter.Close();
            }
        }
    }
}
