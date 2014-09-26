using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class ClusterInstaller : InstallerBase
    {
        Cluster cluster;

        public ClusterInstaller(Context context)
            : base(context)
        {
        }

        public ClusterInstaller(Cluster cluster)
            : base(cluster.Context)
        {
            this.cluster = cluster;
        }

        public Cluster Install()
        {
            return Install(
                true,
                Constants.ClusterName,
                Constants.ClusterAdminName,
                Constants.ClusterAdminEmail,
                Constants.ClusterAdminPassword);
        }

        public Cluster Install(bool system, string clusterName, string username, string email, string password)
        {
            cluster = new Cluster(Context)
            {
                Name = clusterName,
                System = system,
            };
            cluster.Save();

            // Create machine roles and machines

            //      -- controller role
            var mrcont = new MachineRole(cluster)
            {
                Name = Constants.ControllerMachineRoleName,
                System = system,
                MachineRoleType = MachineRoleType.StandAlone,
            };
            mrcont.Save();

            var sv = new ServerVersion(mrcont)
            {
                Name = Constants.ServerVersionName,
                System = system,
            };
            sv.Save();

            var mcont = new Machine(mrcont)
            {
                Name = Constants.ControllerMachineName,
            };
            mcont.Save();

            var sicont = new ServerInstance(mcont)
            {
                Name = Constants.ServerInstanceName,
                ServerVersion = sv,
            };
            sicont.Save();


            //      -- node role
            var mrnode = new MachineRole(cluster)
            {
                Name = Constants.NodeMachineRoleName,
                MachineRoleType = MachineRoleType.MirroredSet,
            };
            mrnode.Save();

            var nodesv = new ServerVersion(mrnode)
            {
                Name = Constants.ServerVersionName,
            };
            nodesv.Save();

            //      -- Create a node
            var mnode = new Machine(mrnode)
            {
                Name = Constants.NodeMachineRoleName + "00"
            };
            mnode.Save();

            var sinode = new ServerInstance(mnode)
            {
                Name = Constants.ServerInstanceName,
                ServerVersion = nodesv,
            };
            sinode.Save();

            var dnode = new DiskVolume(mnode)
            {
                Name = Constants.DiskVolumeName + "0",
                DiskVolumeType = DiskVolumeType.Data | DiskVolumeType.Log | DiskVolumeType.Temporary,
            };
            dnode.Save();

            // Create the shared domain for cluster level databases and users
            var domain = new Domain(cluster)
            {
                Name = Constants.SystemDomainName,
                Email = email,
                System = system,
            };
            domain.Save();

            // Create administrator group and user
            GenerateAdminGroup(system);
            GenerateAdmin(system, username, email, password);

            // Create the shared feredation
            var federation = new Federation(domain)
            {
                Name = Constants.SystemFederationName,
                Email = email,
                System = system,
                ControllerMachine = mcont,
                SchemaSourceServerInstance = sicont,
            };
            federation.Save();

            // Temp database definition
            var tempdd = new DatabaseDefinition(federation)
            {
                Name = Constants.TempDbName,
                System = system,
                LayoutType = DatabaseLayoutType.Monolithic,
                DatabaseInstanceNamePattern = Constants.TempDbInstanceNamePattern,
                DatabaseNamePattern = Constants.TempDbNamePattern,
                SliceCount = 1,
                PartitionCount = 1,
            };
            tempdd.Save();

            var tempddi = new DatabaseDefinitionInstaller(tempdd);
            tempddi.GenerateDefaultChildren(nodesv, Constants.TempDbName);

            // Create cluster level jobs and queues

            //      -- admin queue definition
            QueueDefinition qd = new QueueDefinition(cluster)
            {
                Name = Constants.MaintenanceQueueDefinitionName,
                System = system,
            };
            qd.Save();

            QueueInstance qi = new QueueInstance(mcont)
            {
                Name = Constants.MaintenanceQueueName,
                System = true,
                Hidden = true,
                RunningState = Registry.RunningState.Running,
            };
            qi.QueueDefinitionReference.Value = qd;
            qi.Save();

            //      -- long queue definition
            qd = new QueueDefinition(cluster)
            {
                Name = Constants.LongQueueDefinitionName
            };
            qd.Save();

            qi = new QueueInstance(mcont)
            {
                Name = Constants.LongQueueName,
                RunningState = Registry.RunningState.Running,
            };
            qi.QueueDefinitionReference.Value = qd;
            qi.Save();

            //      -- quick queue definition
            qd = new QueueDefinition(cluster)
            {
                Name = Constants.QuickQueueDefinitionName,
            };
            qd.Save();

            qi = new QueueInstance(mcont)
            {
                Name = Constants.QuickQueueName,
                RunningState = Registry.RunningState.Running,
            };
            qi.QueueDefinitionReference.Value = qd;
            qi.Save();

            //      -- database mirror job
            var jd = new JobDefinition(federation)
            {
                Name = typeof(Jhu.Graywulf.Jobs.MirrorDatabase.MirrorDatabaseJob).Name,
                System = system,
                WorkflowTypeName = GetUnversionedTypeName(typeof(Jhu.Graywulf.Jobs.MirrorDatabase.MirrorDatabaseJob)),
            };
            jd.DiscoverWorkflowParameters();
            jd.Save();

            //      -- test job
            jd = new JobDefinition(federation)
            {
                Name = typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name,
                System = system,
                WorkflowTypeName = GetUnversionedTypeName(typeof(Jhu.Graywulf.Jobs.Test.TestJob)),
            };
            jd.DiscoverWorkflowParameters();
            jd.Save();

            return cluster;
        }

        private void GenerateAdminGroup(bool system)
        {
            cluster.LoadDomains(true);
            var domain = cluster.Domains[Constants.SystemDomainName];

            var ug = new UserGroup(domain)
            {
                Name = Constants.ClusterAdministratorUserGroupName,
                System = system,
            };
            ug.Save();
        }

        public void GenerateAdmin(bool system, string username, string email, string password)
        {
            cluster.LoadDomains(true);
            var domain = cluster.Domains[Constants.SystemDomainName];

            domain.LoadUserGroups(true);
            var ug = domain.UserGroups[Constants.ClusterAdministratorUserGroupName];

            var u = new User(domain)
            {
                Name = username,
                System = system,
                Email = email,
                DeploymentState = Registry.DeploymentState.Deployed,
            };
            u.SetPassword(password);
            u.Save();

            u.AddToGroup(ug.Guid);
        }
    }
}
