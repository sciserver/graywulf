using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs;
using Jhu.Graywulf.Jobs.MirrorDatabase;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    [Serializable]
    public class MirrorDatabaseJobFactory : JobFactoryBase
    {
        #region Static members

        public static MirrorDatabaseJobFactory Create(Federation federation)
        {
            return new MirrorDatabaseJobFactory(federation.Context);
        }

        #endregion
        #region Constructors and initializer

        protected MirrorDatabaseJobFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected MirrorDatabaseJobFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        #endregion

        public JobInstance ScheduleAsJob(DatabaseVersion databaseVersion, bool cascaded)
        {
            var databaseDefinition = databaseVersion.DatabaseDefinition;
            databaseDefinition.LoadDatabaseInstances(false);
            return ScheduleAsJob(databaseDefinition.DatabaseInstances.Values, cascaded);
        }

        public JobInstance ScheduleAsJob(IEnumerable<DatabaseInstance> databaseInstances, bool cascaded)
        {
            return ScheduleAsJob(
                databaseInstances.Where(di =>
                    di.ServerInstance.RunningState == RunningState.Running &&
                    di.ServerInstance.Machine.RunningState == RunningState.Running &&
                    di.DeploymentState == DeploymentState.Deployed &&
                    di.RunningState == RunningState.Attached),
                databaseInstances.Where(di =>
                    di.ServerInstance.RunningState == RunningState.Running &&
                    di.ServerInstance.Machine.RunningState == RunningState.Running &&
                    (di.DeploymentState == DeploymentState.New ||
                     di.DeploymentState == DeploymentState.Undeployed)),
                cascaded);
        }

        public JobInstance ScheduleAsJob(IEnumerable<DatabaseInstance> sourceDatabaseInstances, IEnumerable<DatabaseInstance> destinationDatabaseInstances, bool cascadedMirror)
        {
            var job = CreateJobInstance(null, GetJobDefinitionName(), GetQueueName(), "");

            job.Parameters[Constants.JobParameterCascadedMirror].Value = cascadedMirror;
            job.Parameters[Constants.JobParameterSourceDatabaseInstanceGuids].Value = sourceDatabaseInstances.ToArray();
            job.Parameters[Constants.JobParameterDestinationDatabaseInstanceGuids].Value = destinationDatabaseInstances.ToArray();

            return job;
        }

        private string GetJobDefinitionName()
        {
            return EntityFactory.CombineName(EntityType.JobDefinition, Registry.ContextManager.Configuration.ClusterName, Registry.Constants.SystemDomainName, Registry.Constants.SystemFederationName, typeof(MirrorDatabaseJob).Name);
        }

        private string GetQueueName()
        {
            return EntityFactory.CombineName(EntityType.QueueInstance, Context.Federation.ControllerMachine.GetFullyQualifiedName(), Registry.Constants.MaintenanceQueueName);
        }
    }
}
