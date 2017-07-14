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
    public class RunCheckDb : JobAsyncCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DatabaseInstanceGuid { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            Guid databaseinstanceguid = DatabaseInstanceGuid.Get(activityContext);
            string connectionString;

            EntityGuid.Set(activityContext, databaseinstanceguid);

            using (RegistryContext context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                DatabaseInstance di = new DatabaseInstance(context);
                di.Guid = databaseinstanceguid;
                di.Load();
                connectionString = di.GetConnectionString().ConnectionString;
            }

            return delegate ()
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    cn.Open();

                    using (var cmd = new SqlCommand("DBCC CHECKDB", cn))
                    {
                        cmd.CommandTimeout = 0;

                        var cc = new CancelableDbCommand(cmd);
                        RegisterCancelable(workflowInstanceId, activityInstanceId, cc);
                        cc.ExecuteNonQuery();
                        UnregisterCancelable(workflowInstanceId, activityInstanceId, cc);
                    }
                }
            };
        }
    }
}
