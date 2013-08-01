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

namespace Jhu.Graywulf.Jobs.ExportTable
{
    public class ExportTableJob : GraywulfAsyncCodeActivity, IGraywulfActivity, IExportJob
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<ExportTable> Parameters { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var export = Parameters.Get(activityContext);

            // TODO: try to generalize connection string loading logic and move this from here
            // Use connection string of the database instance, if only database instance name is supplied
            if (export.Source.Dataset is GraywulfDataset)
            {
                var gwds = (GraywulfDataset)export.Source.Dataset;

                if (!String.IsNullOrWhiteSpace(gwds.DatabaseInstanceName))
                {
                    // Load database instace
                    using (var context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                    {
                        var ef = new EntityFactory(context);
                        var di = ef.LoadEntity<DatabaseInstance>(gwds.DatabaseInstanceName);
                        gwds.ConnectionString = di.GetConnectionString().ConnectionString;
                    }
                }
            }
            
            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, export), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, ExportTable exportTable)
        {
            // Create table exporter
            var exporter = exportTable.GetInitializedExporter();

            RegisterCancelable(workflowInstanceGuid, activityInstanceId, exporter);
            exporter.Execute();
            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, exporter);
        }

    }
}
