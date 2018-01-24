using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry.Jobs.MirrorDatabase
{
    [Serializable]
    public class MirrorDatabaseJobFactory : JobFactoryBase
    {
        #region Static members

        public static MirrorDatabaseJobFactory Create(RegistryContext context)
        {
            return new MirrorDatabaseJobFactory(context);
        }

        #endregion
        #region Constructors and initializer

        protected MirrorDatabaseJobFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected MirrorDatabaseJobFactory(RegistryContext context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        #endregion

        public MirrorDatabaseParameters CreateParameters(DatabaseVersion databaseVersion)
        {
            var databaseDefinition = databaseVersion.DatabaseDefinition;
            databaseDefinition.LoadDatabaseInstances(false);
            return CreateParameters(databaseDefinition.DatabaseInstances.Values);
        }

        public MirrorDatabaseParameters CreateParameters(IEnumerable<DatabaseInstance> databaseInstances)
        {
            return CreateParameters(
                databaseInstances.Where(di =>
                    di.ServerInstance.RunningState == RunningState.Running &&
                    di.ServerInstance.Machine.RunningState == RunningState.Running &&
                    di.DeploymentState == DeploymentState.Deployed &&
                    di.RunningState == RunningState.Attached),
                databaseInstances.Where(di =>
                    di.ServerInstance.RunningState == RunningState.Running &&
                    di.ServerInstance.Machine.RunningState == RunningState.Running &&
                    (di.DeploymentState == DeploymentState.New ||
                     di.DeploymentState == DeploymentState.Undeployed)));
        }

        public MirrorDatabaseParameters CreateParameters(IEnumerable<DatabaseInstance> sourceDatabaseInstances, IEnumerable<DatabaseInstance> destinationDatabaseInstances)
        {
            return new MirrorDatabaseParameters()
            {
                SourceDatabaseInstanceGuids = sourceDatabaseInstances.Select(i => i.Guid).ToArray(),
                DestinationDatabaseInstanceGuids = destinationDatabaseInstances.Select(i => i.Guid).ToArray(),
            };
        }

        public JobInstance ScheduleAsJob(MirrorDatabaseParameters parameters, string queueName, TimeSpan timeout, string comments)
        {
            var job = CreateJobInstance(null, GetJobDefinitionName(), queueName, timeout, comments);

            job.Parameters[Constants.JobParameterParameters].Value = parameters;

            return job;
        }

        private string GetJobDefinitionName()
        {
            return EntityFactory.CombineName(EntityType.JobDefinition, RegistryContext.Cluster.Name, Constants.SystemDomainName, Constants.SystemFederationName, typeof(MirrorDatabaseJob).Name);
        }
    }
}
