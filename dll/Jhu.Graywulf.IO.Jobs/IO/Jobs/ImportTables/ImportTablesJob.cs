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

namespace Jhu.Graywulf.IO.Jobs.ImportTables
{
    public class ImportTablesJob : JobAsyncCodeActivity, IJobActivity, IImportTablesJob
    {
        [RequiredArgument]
        public InArgument<ImportTablesParameters> Parameters { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            // TODO: implement multi-file import

            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var parameters = Parameters.Get(activityContext);

            // Make sure connection strings are cached
            using (var context = ContextManager.Instance.CreateReadOnlyContext())
            {
                for (int i = 0; i < parameters.Destinations.Length; i++)
                {
                    if (parameters.Destinations[i].Dataset is GraywulfDataset)
                    {
                        var gwds = (GraywulfDataset)parameters.Destinations[i].Dataset;
                        gwds.RegistryContext = context;
                        gwds.CacheSchemaConnectionString();
                    }
                }
            }

            // Create table importer task
            var sf = StreamFactory.Create(parameters.StreamFactoryType);

            // Get server name from the very first destination
            // (requires trimming the sql server instance name)
            // This will be the database server responsible for executing the import
            var dataset = (Jhu.Graywulf.Sql.Schema.SqlServer.SqlServerDataset)parameters.Destinations[0].Dataset;
            var host = dataset.HostName;

            // Import works in two modes: single file and archive mode.
            // In archive mode, the file is imported using ImportTableArchive task whereas
            // in single file mode a simpler ImportTable task is created

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
                    Timeout = parameters.Timeout,
                };

                using (var task = RemoteServiceHelper.CreateObject<IImportTable>(cancellationContext, host, true))
                {
                    await task.Value.ExecuteAsyncEx(parameters.Sources[0], parameters.Destinations[0], settings);
                }
            }
            else
            {
                // Archive mode

                var settings = new TableCopySettings()
                {
                    BatchName = Util.UriConverter.GetFileNameWithoutExtension(parameters.Uri),
                    FileFormatFactoryType = parameters.FileFormatFactoryType,
                    StreamFactoryType = parameters.StreamFactoryType,
                    Timeout = parameters.Timeout,
                };

                var archiveSettings = new TableArchiveSettings()
                {

                    Uri = parameters.Uri,
                    Credentials = parameters.Credentials,
                };

                using (var task = RemoteServiceHelper.CreateObject<IImportTableArchive>(cancellationContext, host, true))
                {
                    await task.Value.ExecuteAsyncEx(parameters.Sources, parameters.Destinations, settings, archiveSettings);
                }
            }
        }
    }
}
