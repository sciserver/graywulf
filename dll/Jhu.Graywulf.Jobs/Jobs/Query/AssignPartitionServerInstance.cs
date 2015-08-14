using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.Query
{
    public class AssignPartitionServerInstance : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<SqlQueryPartition> QueryPartition { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            SqlQueryPartition queryPartition = QueryPartition.Get(activityContext);

            switch (queryPartition.Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    queryPartition.InitializeQueryObject(null, null, true);
                    break;
                case ExecutionMode.Graywulf:
                    using (var context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                    {
                        var scheduler = activityContext.GetExtension<IScheduler>();

                        //queryPartition.DatabaseVersionName = queryPartition.Query.SourceDatabaseVersionName; TODO: delete
                        queryPartition.InitializeQueryObject(context, scheduler, false);

                        var dss = queryPartition.FindRequiredDatasets();

                        // Check if there are any Graywulf datasets referenced in the query
                        var assignDefault = (dss.Values.FirstOrDefault(ds => !ds.IsSpecificInstanceRequired) == null);

                        // *** TODO: replace this whole thing to use JOIN graphs
                        // If no graywulf datasets are used, use the server containing myDB,
                        // otherwise ask the scheduler for an appropriate server
                        if (!queryPartition.Query.AssignedServerInstanceReference.IsEmpty && (dss.Count == 0 || assignDefault))
                        {
                            queryPartition.AssignServer(queryPartition.Query.AssignedServerInstance);
                        }
                        else
                        {
                            // Assign new server instance
                            var si = new ServerInstance(context);
                            si.Guid = scheduler.GetNextServerInstance(
                                dss.Values.Where(x => !x.IsSpecificInstanceRequired).Select(x => x.DatabaseDefinitionReference.Guid).ToArray(),
                                queryPartition.Query.SourceDatabaseVersionName,
                                null);
                            si.Load();

                            queryPartition.AssignServer(si);
                        }

                        queryPartition.InitializeQueryObject(context, scheduler, true);
                        EntityGuid.Set(activityContext, queryPartition.AssignedServerInstance.Guid);
                    }
                    break;
            }
        }
    }
}
