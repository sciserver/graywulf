using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class PrepareDestinationTable : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        [RequiredArgument]
        public InArgument<string> RemoteTable { get; set; }

        /// <summary>
        /// Creates or truncates destination table in the output database (usually MYDB)
        /// </summary>
        /// <remarks>
        /// This has to be in the QueryPartition class because the Query class does not
        /// have information about the database server the partition is executing on and
        /// the temporary tables are required to generate the destination table schema.
        /// 
        /// The destination table is created by the very first partition that gets to
        /// the point of copying results. This is when the name of the target table is
        /// determined in case only a table name pattern is specified and automatic
        /// unique naming is turned on.
        /// </remarks>
        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            SqlQueryPartition queryPartition = QueryPartition.Get(activityContext);
            SqlQuery query = queryPartition.Query;

            switch (query.Parameters.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // Output is already written to the target table
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    using (RegistryContext registryContext = queryPartition.CreateContext())
                    {
                        queryPartition.InitializeQueryObject(cancellationContext, registryContext, activityContext.GetExtension<IScheduler>(), true);
                        await queryPartition.PrepareDestinationTableAsync();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
