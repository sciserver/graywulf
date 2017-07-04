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
    public class ImportTablesJob : GraywulfAsyncCodeActivity, IGraywulfActivity, IImportTablesJob
    {
        [RequiredArgument]
        public InArgument<JobContext> JobContext { get; set; }

        [RequiredArgument]
        public InArgument<ImportTablesParameters> Parameters { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var parameters = Parameters.Get(activityContext);

            // Make sure connection strings are cached
            using (var context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                for (int i = 0; i < parameters.Destinations.Length; i++)
                {
                    if (parameters.Destinations[i].Dataset is GraywulfDataset)
                    {
                        var gwds = (GraywulfDataset)parameters.Destinations[i].Dataset;
                        gwds.Context = context;
                        gwds.CacheSchemaConnectionString();
                    }
                }
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, parameters), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, ImportTablesParameters parameters)
        {
            // Create table exporter
            var importer = parameters.GetInitializedTableImportTask();

            RegisterCancelable(workflowInstanceGuid, activityInstanceId, importer);

            try
            {
                importer.Open();
                importer.Execute();
            }
            finally
            {
                importer.Close();
            }

            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, importer);
        }

    }
}
