using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    public class ExecuteScript : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlScriptParameters> Parameters { get; set; }

        [RequiredArgument]
        public InArgument<DatasetBase> Dataset { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var parameters = Parameters.Get(activityContext);
            var dataset = Dataset.Get(activityContext);
            
            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, parameters, dataset), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, SqlScriptParameters parameters, DatasetBase dataset)
        {
            var cmd = parameters.GetInitializedDbCommand(dataset);

            RegisterCancelable(workflowInstanceGuid, activityInstanceId, cmd);

            cmd.ExecuteNonQuery();
            cmd.Transaction.Commit();
            cmd.Transaction.Dispose();
            cmd.Connection.Close();
            cmd.Connection.Dispose();

            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, cmd);
        }
    }
}
