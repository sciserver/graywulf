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
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO.Jobs.ExportTables
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
            var sf = StreamFactory.Create(parameters.StreamFactoryType);

            // Determine server name from connection string
            // This is required, because bulk copy can go into databases that are only known
            // by their connection string

            // Get server name from the very first data source
            // (requires trimming the sql server instance name)
            // This will be the database server responsible for executing the export
            string host = ((Jhu.Graywulf.Sql.Schema.SqlServer.SqlServerDataset)parameters.Sources[0].Dataset).HostName;

            // Export works in two modes: single file and archive mode.
            // In archive mode, the file is exported using ExportTableArchive task whereas
            // in single file mode a simpler ExportTable task is created

            // If arhival mode is set to automatic, figure out mode from extensions
            var archival = parameters.Archival;
            if (archival == DataFileArchival.Automatic)
            {
                archival = sf.GetArchivalMethod(parameters.Uri);
            }

            if (archival == DataFileArchival.None)
            {
                // Single file mode
                // Use only first item from sources and destinations
                // TODO: this could be extended but that would mean multiple tasks

                var settings = new TableCopySettings()
                {
                    FileFormatFactoryType = parameters.FileFormatFactoryType,
                    StreamFactoryType = parameters.StreamFactoryType,
                    Timeout = parameters.Timeout
                };

                using (var task = RemoteServiceHelper.CreateObject<IExportTable>(cancellationContext, host, true))
                {
                    await task.Value.ExecuteAsync(parameters.Sources[0], parameters.Destinations[0], settings);
                }
            }
            else
            {
                // Archive mode

                var settings = new TableCopySettings()
                {
                    BatchName = Util.UriConverter.GetFileNameWithoutExtension(parameters.Uri),
                    Uri = parameters.Uri,
                    Credentials = parameters.Credentials,
                    FileFormatFactoryType = parameters.FileFormatFactoryType,
                    StreamFactoryType = parameters.StreamFactoryType,
                    Timeout = parameters.Timeout,
                };

                using (var task = RemoteServiceHelper.CreateObject<IExportTableArchive>(cancellationContext, host, true))
                {
                    await task.Value.ExecuteAsync(parameters.Sources, parameters.Destinations, settings);
                }
            }
        }
    }
}
