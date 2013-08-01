/* Copyright */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Cluster</b> entity
    /// </summary>
    public partial class Cluster : Entity
    {
        public static class AppSettings
        {
            private static string GetValue(string key)
            {
                return (string)((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Registry/Cluster"))[key];
            }

            public static string ClusterName
            {
                get { return GetValue("ClusterName"); }
            }
        }

        #region Validation Properties
        #endregion
        #region Navigation Properties

        public Dictionary<string, MachineRole> MachineRoles
        {
            get { return GetChildren<MachineRole>(); }
            set { SetChildren<MachineRole>(value); }
        }

        public Dictionary<string, Domain> Domains
        {
            get { return GetChildren<Domain>(); }
            set { SetChildren<Domain>(value); }
        }

        public Dictionary<string, DatabaseDefinition> DatabaseDefinitions
        {
            get { return GetChildren<DatabaseDefinition>(); }
            set { SetChildren<DatabaseDefinition>(value); }
        }

        public Dictionary<string, QueueDefinition> QueueDefinitions
        {
            get { return GetChildren<QueueDefinition>(); }
            set { SetChildren<QueueDefinition>(value); }
        }

        public Dictionary<string, JobDefinition> JobDefinitions
        {
            get { return GetChildren<JobDefinition>(); }
            set { SetChildren<JobDefinition>(value); }
        }

        public Dictionary<string, User> Users
        {
            get { return GetChildren<User>(); }
            set { SetChildren<User>(value); }
        }

        public Dictionary<string, UserGroup> UserGroups
        {
            get { return GetChildren<UserGroup>(); }
            set { SetChildren<UserGroup>(value); }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public Cluster()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Cluster</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public Cluster(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Cluster</b> objects.
        /// </summary>
        /// <param name="old">The <b>Cluster</b> to copy from.</param>
        public Cluster(Cluster old)
            : base(old)
        {
            CopyMembers(old);
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            base.EntityType = EntityType.Cluster;
            base.EntityGroup = EntityGroup.All;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Cluster</b> object to create the deep copy from.</param>
        private void CopyMembers(Cluster old)
        {
        }

        protected override Type[] CreateChildTypes()
        {
            return new Type[] {
                    typeof(MachineRole),
                    typeof(Domain),
                    typeof(DatabaseDefinition),
                    typeof(QueueDefinition),
                    typeof(JobDefinition),
                    typeof(User),
                    typeof(UserGroup)
            };
        }

        #endregion

        public void GenerateDefaultChildren()
        {
            GenerateDefaultChildren(Constants.ClusterAdminName, Constants.ClusterAdminEmail, Constants.ClusterAdminPassword);
        }

        public void GenerateDefaultChildren(string username, string email, string password)
        {
            // Create administrator group and user

            UserGroup ug = new UserGroup(Context, this);
            ug.Name = Constants.ClusterAdministratorUserGroupName;
            ug.Save();

            User u = new User(Context, this);
            u.Name = username;
            u.Email = email;
            u.SetPassword(password);
            u.DeploymentState = Registry.DeploymentState.Deployed;
            u.Save();

            // TODO: add user to group

            // Create machine roles and machines

            //      -- controller
            MachineRole mrcont = new MachineRole(Context, this);
            mrcont.Name = Constants.ControllerMachineRoleName;
            mrcont.MachineRoleType = MachineRoleType.StandAlone;
            mrcont.Save();

            ServerVersion sv = new ServerVersion(Context, mrcont);
            sv.Name = Constants.ServerVersionName;
            sv.Save();

            Machine mcont = new Machine(Context, mrcont);
            mcont.Name = Constants.ControllerMachineName;
            mcont.Save();

            ServerInstance si = new ServerInstance(Context, mcont);
            si.Name = Constants.ServerInstanceName;
            si.ServerVersionReference.Value = sv;
            si.Save();


            //      -- node
            MachineRole mrnode = new MachineRole(Context, this);
            mrnode.Name = Constants.NodeMachineRoleName;
            mrnode.MachineRoleType = MachineRoleType.MirroredSet;
            mrnode.Save();

            sv = new ServerVersion(Context, mrnode);
            sv.Name = Constants.ServerVersionName;
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

            // Create temp database definition

            DatabaseDefinition dd = new DatabaseDefinition(Context, this);
            dd.Name = Constants.TempDbName;
            dd.LayoutType = DatabaseLayoutType.Monolithic;
            dd.DatabaseInstanceNamePattern = Constants.TempDbInstanceNamePattern;
            dd.DatabaseNamePattern = Constants.TempDbNamePattern;
            dd.SliceCount = 1;
            dd.PartitionCount = 1;
            dd.Save();

            FileGroup fg = new FileGroup(Context, dd);
            fg.FileGroupType = FileGroupType.Data;
            fg.LayoutType = FileGroupLayoutType.Monolithic;
            fg.Name = Constants.PrimaryFileGroupName;
            fg.DiskVolumeType = DiskVolumeType.Data;
            fg.FileGroupName = Constants.PrimaryFileGroupName;
            fg.AllocatedSpace = 0x8000000;  // 128 MB
            fg.AllocationType = FileGroupAllocationType.CrossVolume;
            fg.FileCount = 0;
            fg.Save();

            fg = new FileGroup(Context, dd);
            fg.FileGroupType = FileGroupType.Log;
            fg.LayoutType = FileGroupLayoutType.Monolithic;
            fg.Name = Constants.LogFileGroupName;
            fg.DiskVolumeType = DiskVolumeType.Log;
            fg.FileGroupName = Constants.LogFileGroupName;
            fg.AllocatedSpace = 0x1000000;  // 16 MB
            fg.AllocationType = FileGroupAllocationType.CrossVolume;
            fg.FileCount = 0;
            fg.Save();

            Slice sl = new Slice(Context, dd);
            sl.Name = Constants.FullSliceName;
            sl.Save();

            DatabaseVersion dv = new DatabaseVersion(Context, dd);
            dv.Name = Constants.TempDbName;
            dv.ServerVersionReference.Value = sv;
            dv.Save();

            // Create cluster level jobs and queues

            //      -- admin queue definition
            QueueDefinition qd = new QueueDefinition(Context, this);
            qd.Name = Constants.MaintenanceQueueDefinitionName;
            qd.Save();

            QueueInstance qi = new QueueInstance(Context, mcont);
            qi.Name = Constants.MaintenanceQueueName;
            qi.QueueDefinitionReference.Value = qd;
            qi.RunningState = Registry.RunningState.Running;
            qi.Save();

            //      -- long queue definition
            qd = new QueueDefinition(Context, this);
            qd.Name = Constants.LongQueueDefinitionName;
            qd.Save();

            qi = new QueueInstance(Context, mcont);
            qi.Name = Constants.LongQueueName;
            qi.QueueDefinitionReference.Value = qd;
            qi.RunningState = Registry.RunningState.Running;
            qi.Save();

            //      -- quick queue definition
            qd = new QueueDefinition(Context, this);
            qd.Name = Constants.QuickQueueDefinitionName;
            qd.Save();

            qi = new QueueInstance(Context, mcont);
            qi.Name = Constants.QuickQueueName;
            qi.QueueDefinitionReference.Value = qd;
            qi.RunningState = Registry.RunningState.Running;
            qi.Save();

            //      -- database mirror job
            var jd = new JobDefinition(Context, this);
            jd.Name = typeof(Jobs.MirrorDatabase.MirrorDatabaseJob).Name;
            jd.WorkflowTypeName = typeof(Jobs.MirrorDatabase.MirrorDatabaseJob).AssemblyQualifiedName;
            jd.Save();

            //      -- test job
            jd = new JobDefinition(Context, this);
            jd.Name = typeof(Jobs.Test.TestJob).Name;
            jd.WorkflowTypeName = typeof(Jobs.Test.TestJob).AssemblyQualifiedName;
            jd.Save();
        }
    }
}
