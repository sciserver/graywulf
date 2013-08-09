using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
    public class GeneratePartitions : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryBase> Query { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            QueryBase query = Query.Get(activityContext);

            int pcount = 1;

            // Single server mode will run on one partition by definition,
            // Graywulf mode has to look at the registry for available machines
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    query.InitializeQueryObject(null);
                    break;
                case ExecutionMode.Graywulf:
                    using (Context context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                    {
                        var scheduler = activityContext.GetExtension<IScheduler>();
                        query.InitializeQueryObject(context, scheduler);

                        // TODO: move this code inside the QueryBase class
                        // If query is partitioned, statistics must be gathered
                        if (query.IsPartitioned)
                        {
                            // Assign a server that will run the statistics queries
                            // Try to find a server that contains all required datasets. This is true right now for
                            // SkyQuery where all databases are mirrored but will have to be updated later

                            // Collect all datasets that are required to answer the query
                            var dss = query.FindRequiredDatasets();

                            // Datasets that are mirrored and can be on any server
                            var reqds = (from ds in dss.Values
                                         where !ds.IsSpecificInstanceRequired
                                         select ds.DatabaseDefinition.Guid).ToArray();

                            // Datasets that are only available at a specific server instance
                            /*var spds = (from ds in dss.Values
                                        where ds.IsSpecificInstanceRequired && !ds.DatabaseDefinition.IsEmpty
                                        select ds.DatabaseDefinition.Guid).ToArray();*/
                            var spds = (from ds in dss.Values
                                        where ds.IsSpecificInstanceRequired && !ds.DatabaseInstance.IsEmpty
                                        select ds.DatabaseInstance.Guid).ToArray();

                            
                            var si = new ServerInstance(context);
                            si.Guid = scheduler.GetNextServerInstance(reqds, query.StatDatabaseVersionName, spds);
                            si.Load();

                            query.AssignedServerInstance = si;

                            query.DatabaseVersionName = query.StatDatabaseVersionName;

                            // *** TODO: find optimal number of partitions
                            // TODO: replace "2" with a value from settings
                            pcount = 2 * scheduler.GetServerInstances(reqds, query.SourceDatabaseVersionName, spds).Length;

                            // Now have to reinitialize to load the assigned server instances
                            query.InitializeQueryObject(context, scheduler, true);
                            EntityGuid.Set(activityContext, query.AssignedServerInstance.Guid);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            query.GeneratePartitions(pcount);
        }
    }
}
