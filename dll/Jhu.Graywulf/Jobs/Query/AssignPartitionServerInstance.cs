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
        public InArgument<QueryPartitionBase> QueryPartition { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            QueryPartitionBase queryPartition = QueryPartition.Get(activityContext);

            switch (queryPartition.Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    queryPartition.InitializeQueryObject(null, true);
                    break;
                case ExecutionMode.Graywulf:
                    using (var context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                    {
                        var scheduler = activityContext.GetExtension<IScheduler>();

                        queryPartition.DatabaseVersionName = queryPartition.Query.SourceDatabaseVersionName;
                        queryPartition.InitializeQueryObject(context, true);

                        var dss = queryPartition.FindRequiredDatasets();

                        // Check if there are any Graywulf datasets referenced in the query
                        var assignmydb = (dss.Values.FirstOrDefault(ds => !ds.IsSpecificInstanceRequired) == null);

                        // *** TODO: replace this whole thing to use JOIN graphs
                        // If no graywulf datasets are used, use the server containing myDB,
                        // otherwise ask the scheduler for an appropriate server
                        if (dss.Count == 0 || assignmydb)
                        {
                            // use MyDB's server
                            var ef = new EntityFactory(context);
                            var federation = queryPartition.FederationReference.Value;
                            var user = ef.LoadEntity<User>(context.UserGuid);
                            var di = user.GetMyDB(federation);

                            queryPartition.AssignedServerInstanceReference.Value = di.ServerInstance;
                        }
                        else
                        {
                            // Assign new server instance
                            // TODO: delete next line if works correctly
                            //queryPartition.AssignedServerInstance = new EntityReference<ServerInstance>();
                            queryPartition.AssignedServerInstanceReference.Guid =
                                scheduler.GetServerInstance(dss.Values.Where(x => !x.IsSpecificInstanceRequired).Select(x => x.DatabaseDefinitionName).ToArray(), queryPartition.Query.SourceDatabaseVersionName);
                        }

                        queryPartition.InitializeQueryObject(context, true);

                        EntityGuid.Set(activityContext, queryPartition.AssignedServerInstanceReference.Guid);
                    }
                    break;
            }
        }
    }
}
