using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    public class InitializeQuery : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryBase> Query { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
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
                        query.InitializeQueryObject(context);

                        // If query is partitioned, statistics must be gathered
                        if (query.IsPartitioned)
                        {

                            // Assign a server that will run the statistics queries
                            IScheduler scheduler = activityContext.GetExtension<IScheduler>();
                            // TODO: delete next line if works correctly
                            //query.AssignedServerInstance = new EntityReference<ServerInstance>();
                            query.AssignedServerInstanceReference.Guid = 
                                scheduler.GetServerInstance(
                                    query.FindRequiredDatasets().Values.Select(x => x.DatabaseDefinitionName).ToArray(),
                                    query.StatDatabaseVersionName);

                            query.DatabaseVersionName = query.StatDatabaseVersionName;

                            // *** TODO: find optimal number of partitions
                            // TODO: replace "2" with a value from settings
                            pcount = 2 * scheduler.GetServerInstances(
                                query.FindRequiredDatasets().Values.Select(x => x.DatabaseDefinitionName).ToArray(),
                                query.SourceDatabaseVersionName).Length;
                        }

                        // Now have to reinitialize to load the assigned server instances
                        query.InitializeQueryObject(context, true);

                        EntityGuid.Set(activityContext, query.AssignedServerInstanceReference.Guid);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, query, pcount), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, QueryBase query, int partitionCount)
        {
            RegisterCancelable(workflowInstanceGuid, activityInstanceId, query);
            query.GeneratePartitions(partitionCount);
            UnregisterCancelable(workflowInstanceGuid, activityInstanceId, query);
        }
    }
}
