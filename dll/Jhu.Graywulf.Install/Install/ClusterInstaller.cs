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
        private bool system;
        private string clusterName;
        private string adminUserName;
        private string adminEmail;
        private string adminPassword;
        private bool createNode;
        
        Cluster cluster;

        public bool System
        {
            get { return system; }
            set { system = value; }
        }

        public string ClusterName
        {
            get { return clusterName; }
            set { clusterName = value; }
        }

        public string AdminUserName
        {
            get { return adminUserName; }
            set { adminUserName = value; }
        }

        public string AdminEmail
        {
            get { return adminEmail; }
            set { adminEmail = value; }
        }

        public string AdminPassword
        {
            get { return adminPassword; }
            set { adminPassword = value; }
        }

        public bool CreateNode
        {
            get { return createNode; }
            set { createNode = value; }
        }

        public ClusterInstaller(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        public ClusterInstaller(Cluster cluster)
            : base(cluster.Context)
        {
            InitializeMembers();

            this.cluster = cluster;
        }

        private void InitializeMembers()
        {
            this.system = true;
            this.clusterName = Constants.ClusterName;
            this.adminUserName = Constants.ClusterAdminUserName;
            this.adminEmail = Constants.ClusterAdminUserEmail;
            this.adminPassword = Constants.ClusterAdminUserPassword;
            this.createNode = true; ;

            this.cluster = null;
        }

        public Cluster Install()
        {
            cluster = new Cluster(Context)
            {
                Name = clusterName,
                System = system,
            };
            cluster.Save();

            // Create machine roles and machines
            MachineRole controllerMachineRole;
            Machine controllerMachine;
            ServerVersion controllerServerVersion;
            GenerateController(system, out controllerMachineRole, out controllerMachine, out controllerServerVersion);

            // Create the shared domain for cluster level databases and users
            var domain = new Domain(cluster)
            {
                Name = Constants.SystemDomainName,
                Email = adminEmail,
                System = system,
            };
            domain.Save();

            // Create administrator group and user
            GenerateAdminGroup(system);
            GenerateAdminRole(system);
            GenerateAdmin(system, adminUserName, adminEmail, adminPassword);

            // Create the shared feredation
            var federation = new Federation(domain)
            {
                Name = Constants.SystemFederationName,
                Email = adminEmail,
                System = system,
                ControllerMachineRole = controllerMachineRole,
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

            if (createNode)
            {
                ServerVersion nodeServerVersion;
                GenerateNode(system, out nodeServerVersion);

                var tempddi = new DatabaseDefinitionInstaller(tempdd);
                tempddi.GenerateDefaultChildren(nodeServerVersion, Constants.TempDbName);
                // TODO: use temp designation for temp database file groups
            }

            // Create cluster level jobs and queues

            //      -- admin queue definition
            QueueDefinition qd = new QueueDefinition(cluster)
            {
                Name = Constants.MaintenanceQueueDefinitionName,
                System = true,
            };
            qd.Save();

            QueueInstance qi = new QueueInstance(controllerMachineRole)
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

            qi = new QueueInstance(controllerMachineRole)
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

            qi = new QueueInstance(controllerMachineRole)
            {
                Name = Constants.QuickQueueName,
                RunningState = Registry.RunningState.Running,
            };
            qi.QueueDefinitionReference.Value = qd;
            qi.Save();

            //      -- database mirror job
            JobInstallerBase jdi = new Jhu.Graywulf.Jobs.MirrorDatabase.MirrorDatabaseJobInstaller(federation);
            var jd = jdi.Install();

            //      -- test job
            jdi = new Jhu.Graywulf.Jobs.Test.TestJobInstaller(federation);
            jd = jdi.Install();

            return cluster;
        }

        private void GenerateController(bool system, out MachineRole controllerMachineRole, out Machine controllerMachine, out ServerVersion controllerServerVersion)
        {
            controllerMachineRole = new MachineRole(cluster)
            {
                Name = Constants.ControllerMachineRoleName,
                System = system,
                MachineRoleType = MachineRoleType.StandAlone,
            };
            controllerMachineRole.Save();

            controllerServerVersion = new ServerVersion(controllerMachineRole)
            {
                Name = Constants.ServerVersionName,
                System = system,
            };
            controllerServerVersion.Save();

            controllerMachine = new Machine(controllerMachineRole)
            {
                Name = Constants.ControllerMachineName,
            };
            controllerMachine.Save();

            var controllerServerInstance = new ServerInstance(controllerMachine)
            {
                Name = Constants.ServerInstanceName,
                ServerVersion = controllerServerVersion,
            };
            controllerServerInstance.Save();
        }

        private void GenerateNode(bool system, out ServerVersion nodeServerVersion)
        {
            var mrnode = new MachineRole(cluster)
            {
                Name = Constants.NodeMachineRoleName,
                MachineRoleType = MachineRoleType.MirroredSet,
            };
            mrnode.Save();

            nodeServerVersion = new ServerVersion(mrnode)
            {
                Name = Constants.ServerVersionName,
            };
            nodeServerVersion.Save();

            //      -- Create a node with a disk group and a disk volume and a server instance
            var mnode = new Machine(mrnode)
            {
                Name = Constants.NodeMachineRoleName + "00"
            };
            mnode.Save();

            var gnode = new DiskGroup(mnode)
            {
                Name = Constants.DataDiskGroupName + "0",
                Type = DiskGroupType.Jbod,
            };
            gnode.Save();

            var dnode = new DiskVolume(gnode)
            {
                Name = Constants.DiskVolumeName + "0",
                Type = DiskVolumeType.Unknown
            };
            dnode.Save();

            var sinode = new ServerInstance(mnode)
            {
                Name = Constants.ServerInstanceName,
                ServerVersion = nodeServerVersion,
            };
            sinode.Save();

            var sidgnode = new ServerInstanceDiskGroup(sinode)
            {
                Name = Constants.DataDiskGroupName,
                DiskDesignation = DiskDesignation.Data | DiskDesignation.Log,
                DiskGroup = gnode,
            };
            sidgnode.Save();
        }

        private void GenerateAdminGroup(bool system)
        {
            cluster.LoadDomains(true);
            var domain = cluster.Domains[Constants.SystemDomainName];

            var ug = new UserGroup(domain)
            {
                Name = Constants.ClusterAdminUserGroupName,
                System = system,
            };
            ug.Save();
        }

        private void GenerateAdminRole(bool system)
        {
            cluster.LoadDomains(true);
            var domain = cluster.Domains[Constants.SystemDomainName];

            var ur = new UserRole(domain)
            {
                Name = Constants.ClusterAdminUserRoleName,
                System = system,
            };
            ur.Save();
        }

        public void GenerateAdmin(bool system, string username, string email, string password)
        {
            cluster.LoadDomains(true);
            var domain = cluster.Domains[Constants.SystemDomainName];

            domain.LoadUserGroups(true);
            var ug = domain.UserGroups[Constants.ClusterAdminUserGroupName];

            domain.LoadUserRoles(true);
            var ur = domain.UserRoles[Constants.ClusterAdminUserRoleName];

            var u = new User(domain)
            {
                Name = username,
                System = system,
                Email = email,
                DeploymentState = Registry.DeploymentState.Deployed,
            };
            u.SetPassword(password);
            u.Save();

            u.AddToGroup(ug.Guid, ur.Guid);
        }
    }
}
