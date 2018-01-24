using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Registry.Jobs.MirrorDatabase
{
    public class RunCheckDb : JobAsyncCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DatabaseInstanceGuid { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            Guid databaseinstanceguid = DatabaseInstanceGuid.Get(activityContext);
            string connectionString;

            EntityGuid.Set(activityContext, databaseinstanceguid);

            using (RegistryContext context = ContextManager.Instance.CreateReadOnlyContext())
            {
                var ef = new EntityFactory(context);
                var di = ef.LoadEntity<DatabaseInstance>(databaseinstanceguid);
                connectionString = di.GetConnectionString().ConnectionString;
            }

            using (var cn = new SqlConnection(connectionString))
            {
                await cn.OpenAsync(cancellationContext.Token);

                using (var cmd = new SqlCommand("DBCC CHECKDB", cn))
                {
                    cmd.CommandTimeout = 0;
                    await cmd.ExecuteNonQueryAsync(cancellationContext.Token);
                }
            }
        }
    }
}
