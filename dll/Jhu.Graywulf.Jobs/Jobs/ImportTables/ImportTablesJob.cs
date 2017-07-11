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

namespace Jhu.Graywulf.Jobs.ImportTables
{
    public class ImportTablesJob : JobAsyncCodeActivity, IJobActivity, IImportTablesJob
    {
        [RequiredArgument]
        public InArgument<ImportTablesParameters> Parameters { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var parameters = Parameters.Get(activityContext);

            // Make sure connection strings are cached
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
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

            return delegate (JobContext asyncContext)
            {
                // Create table importer
                var importer = parameters.GetInitializedTableImportTask();

                asyncContext.RegisterCancelable(importer);

                try
                {
                    importer.Open();
                    importer.Execute();
                }
                finally
                {
                    importer.Close();
                }

                asyncContext.UnregisterCancelable(importer);
            };
        }
    }
}
