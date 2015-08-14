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
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
    public class ComputeTableStatistics : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        [RequiredArgument]
        public InArgument<ITableSource> TableSource { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var query = Query.Get(activityContext);
            var tableSource = TableSource.Get(activityContext);
            string connectionString;
            SqlCommand cmd;
            int multiplier;

            using (Context context = query.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                query.InitializeQueryObject(context, activityContext.GetExtension<IScheduler>(), true);
                query.PrepareComputeTableStatistics(context, tableSource, out connectionString, out cmd, out multiplier);
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, query, tableSource, connectionString, cmd, multiplier), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, SqlQuery query, ITableSource tableSource, string connectionString, SqlCommand cmd, int multiplier)
        {
            RegisterCancelable(workflowInstanceGuid, activityInstanceId, query);
            query.ComputeTableStatistics(tableSource, connectionString, cmd, multiplier);
            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, query);
        }
    }
}
