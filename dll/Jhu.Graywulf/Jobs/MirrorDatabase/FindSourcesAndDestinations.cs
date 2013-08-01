using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class FindSourcesAndDestinations : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<string> DatabaseVersionName { get; set; }

        [RequiredArgument]
        public OutArgument<Queue<Guid>> SourceDatabaseInstanceGuids { get; set; }
        [RequiredArgument]
        public OutArgument<Queue<Guid>> DestinationDatabaseInstanceGuids { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            // get/init workflow arguments
            string databaseVersionName = DatabaseVersionName.Get(activityContext);
            Queue<Guid> sourceDatabaseInstanceGuids = new Queue<Guid>();
            Queue<Guid> destinationDatabaseInstanceGuids = new Queue<Guid>();

            using (Context context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                EntityFactory ef = new EntityFactory(context);
                DatabaseVersion dv = ef.LoadEntity<DatabaseVersion>(databaseVersionName);

                EntityGuid.Set(activityContext, dv.Guid);

                dv.DatabaseDefinition.LoadDatabaseInstances(false);

                foreach (DatabaseInstance di in dv.DatabaseDefinition.DatabaseInstances.Values)
                {
                    if (Entity.StringComparer.Compare(di.DatabaseVersion.Name, dv.Name) == 0)
                    {
                        if (di.DeploymentState == DeploymentState.Deployed && di.RunningState == RunningState.Attached)
                        {
                            sourceDatabaseInstanceGuids.Enqueue(di.Guid);
                        }
                        else if (di.DeploymentState == DeploymentState.New || di.DeploymentState == DeploymentState.Undeployed)
                        {
                            destinationDatabaseInstanceGuids.Enqueue(di.Guid);
                        }
                    }
                }
            }

            SourceDatabaseInstanceGuids.Set(activityContext, sourceDatabaseInstanceGuids);
            DestinationDatabaseInstanceGuids.Set(activityContext, destinationDatabaseInstanceGuids);

            if (sourceDatabaseInstanceGuids.Count == 0 ||
                destinationDatabaseInstanceGuids.Count == 0)
            {
                throw new InvalidOperationException(ExceptionMessages.NoDatabasesToCopy);
            }
        }
    }
}
