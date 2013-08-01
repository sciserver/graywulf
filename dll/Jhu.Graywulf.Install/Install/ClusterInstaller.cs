using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class ClusterInstaller : ContextObject
    {
        public ClusterInstaller(Context context)
            : base(context)
        {
        }

        public void Install()
        {
            Install(
                true,
                Constants.ClusterName,
                Constants.ClusterAdminName,
                Constants.ClusterAdminEmail,
                Constants.ClusterAdminPassword);
        }

        public void Install(bool system, string clusterName, string username, string email, string password)
        {
            var cluster = new Cluster(Context)
            {
                Name = clusterName,
                System = system,
            };
            cluster.Save();

            // Create administrator group and user

            var ug = new UserGroup(cluster)
            {
                Name = Constants.ClusterAdministratorUserGroupName,
                System = system,
            };
            ug.Save();

            var u = new User(cluster)
            {
                Name = username,
                System = system,
                Email = email,
                DeploymentState = Registry.DeploymentState.Deployed,
            };
            u.SetPassword(password);
            u.Save();
            u.MakeMemberOf(ug.Guid);

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
            };
            sv.Save();

            var mcont = new Machine(mrcont)
            {
                Name = Constants.ControllerMachineName,
            };
            mcont.Save();

            var si = new ServerInstance(mcont)
            {
                Name = Constants.ServerInstanceName,
                ServerVersion = sv,
            };
            si.Save();


            //      -- node role
            var mrnode = new MachineRole(cluster)
            {
                Name = Constants.NodeMachineRoleName,
                MachineRoleType = MachineRoleType.MirroredSet,
            };
            mrnode.Save();

            sv = new ServerVersion(mrnode)
            {
                Name = Constants.ServerVersionName,
            };
            sv.Save();

            //      -- Create a node
            /*
            Machine mnode = new Machine(Context, mrnode);
            mnode.Name = Constants.NodeMachineName;
            mnode.Save();

            si = new ServerInstance(Context, mnode);
            si.Name = Constants.ServerInstanceName;
            si.ServerVersionReference.Value = sv;
            si.Save();*/

            // Temp database definition
            var tempdd = new DatabaseDefinition(cluster)
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
            tempddi.GenerateDefaultChildren(sv, Constants.TempDbName);

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
            var jd = new JobDefinition(cluster)
            {
                Name = typeof(Jhu.Graywulf.Jobs.MirrorDatabase.MirrorDatabaseJob).Name,
                System = system,
                WorkflowTypeName = typeof(Jhu.Graywulf.Jobs.MirrorDatabase.MirrorDatabaseJob).AssemblyQualifiedName,
            };
            jd.Save();

            //      -- test job
            jd = new JobDefinition(cluster)
            {
                Name = typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name,
                System = system,
                WorkflowTypeName = typeof(Jhu.Graywulf.Jobs.Test.TestJob).AssemblyQualifiedName,
            };
            jd.Save();
        }
    }
}
