using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class RunCheckDb : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DatabaseInstanceGuid { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            Guid databaseinstanceguid = DatabaseInstanceGuid.Get(activityContext);
            string connectionString;

            EntityGuid.Set(activityContext, databaseinstanceguid);

            using (Context context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                DatabaseInstance di = new DatabaseInstance(context);
                di.Guid = databaseinstanceguid;
                di.Load();
                connectionString = di.GetConnectionString().ConnectionString;
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, connectionString), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, string connectionString)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand("DBCC CHECKDB", cn))
                {
                    var cc = new CancelableDbCommand(cmd);
                    RegisterCancelable(workflowInstanceGuid, activityInstanceId, cc);
                    cc.ExecuteNonQuery();
                    UnregisterCancelable(workflowInstanceGuid, activityInstanceId, cc);
                }
            }
        }
    }
}
