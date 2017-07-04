using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Data;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    public class ExecuteScript : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<JobContext> JobContext { get; set; }

        [RequiredArgument]
        public InArgument<SqlScriptParameters> Parameters { get; set; }

        [RequiredArgument]
        public InArgument<DatasetBase> Dataset { get; set; }

        [RequiredArgument]
        public InArgument<string> Script { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var parameters = Parameters.Get(activityContext);
            var dataset = Dataset.Get(activityContext);
            var script = Script.Get(activityContext);
            
            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, parameters, dataset, script), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, SqlScriptParameters parameters, DatasetBase dataset, string script)
        {
            var cn = dataset.OpenConnection();
            var cmd = cn.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = script;
            cmd.CommandTimeout = parameters.Timeout;
            cmd.Transaction = cn.BeginTransaction(parameters.IsolationLevel);

            var ccmd = new CancelableDbCommand(cmd);

            RegisterCancelable(workflowInstanceGuid, activityInstanceId, ccmd);

            ccmd.ExecuteNonQuery();

            if (ccmd.Transaction != null)
            {
                ccmd.Transaction.Commit();
            }

            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, ccmd);

            ccmd.Dispose();
        }
    }
}
