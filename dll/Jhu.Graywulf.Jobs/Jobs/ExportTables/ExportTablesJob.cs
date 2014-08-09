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

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public class ExportTablesJob : GraywulfAsyncCodeActivity, IGraywulfActivity, IExportTablesJob
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<ExportTablesParameters> Parameters { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var parameters = Parameters.Get(activityContext);

            // Make sure connection strings are cached
            using (var context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                for (int i = 0; i < parameters.Sources.Length; i++)
                {
                    if (parameters.Sources[i].Dataset is GraywulfDataset)
                    {
                        // Load database instace and get connection string to make it cached

                        var gwds = (GraywulfDataset)parameters.Sources[i].Dataset;
                        gwds.Context = context;
                        gwds.CacheSchemaConnectionString();
                    }
                }
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, parameters), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, ExportTablesParameters exportTable)
        {
            // Create table exporter
            var exporter = exportTable.GetInitializedTableExportTask();

            RegisterCancelable(workflowInstanceGuid, activityInstanceId, exporter);

            try
            {
                exporter.Open();
                exporter.Execute();
            }
            finally
            {
                exporter.Close();
            }

            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, exporter);
        }

    }
}
