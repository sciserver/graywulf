using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Registry
{
    public class Constants
    {
        public const string ClusterName = "Graywulf";
        public const string ClusterAdministratorUserGroupName = "Administrators";
        public const string ClusterAdminName = "admin";
        public const string ClusterAdminEmail = "admin@graywulf.org";
        public const string ClusterAdminPassword = "graywulf";
        public const string ControllerMachineRoleName = "Controller";
        public const string ControllerMachineName = "Controller";
        public const string NodeMachineRoleName = "Node";
        public const string ServerVersionName = "MSSQL";
        public const string ServerInstanceName = "MSSQL";
        public const string MaintenanceQueueDefinitionName = "Maintenance";
        public const string MaintenanceQueueName = "Maintenance";
        public const string LongQueueDefinitionName = "Long";
        public const string LongQueueName = "Long";
        public const string QuickQueueDefinitionName = "Quick";
        public const string QuickQueueName = "Quick";
        public const string MachineHostName = "localhost";
        public const string MachineAdminUrl = @"http://[$HostName]/gwcontrol";
        public const string MachineDeployUncPath = @"\\[$HostName]\GWCode\";
        public const string DiskVolumeName = "Data";
        public const string DiskVolumeLocalPath = @"C:\Data\[$Name]\";
        public const string DiskVolumeUncPath = @"\\[$Parent.HostName]\Data\[$Name]\";

        public const string SlicedDatabaseInstanceNamePattern = "[$DatabaseDefinition.Name]_[$Slice.Name]_[$DatabaseVersion.Name]";
        public const string SlicedDatabaseNamePattern = "[$DatabaseDefinition.Federation.Name]_[$DatabaseDefinition.Name]_[$Slice.Name]_[$DatabaseVersion.Name]";
        public const string MirroredDatabaseInstanceNamePattern = "[$DatabaseDefinition.Name]_[$DatabaseVersion.Name]_[$Number]";
        public const string MirroredDatabaseNamePattern = "[$DatabaseDefinition.Federation.Name]_[$DatabaseDefinition.Name]_[$DatabaseVersion.Name]";
        public const string MonolithicDatabaseInstanceNamePattern = "[$DatabaseDefinition.Name]_[$DatabaseVersion.Name]";
        public const string MonolithicDatabaseNamePattern = "[$DatabaseDefinition.Federation.Name]_[$DatabaseDefinition.Name]_[$DatabaseVersion.Name]";
        
        public const string PrimaryFileGroupName = "PRIMARY";
        public const long PrimaryFileGroupSize = 0x8000000;     // 128 MB

        public const string LogFileGroupName = "LOG";
        public const long LogFileGroupSize = 0x1000000;      // 16 MB

        public const string FullSliceName = "FULL";

        public const string HotDatabaseVersionName = "HOT";
        public const string StatDatabaseVersionName = "STAT";

        public const string SystemDomainName = "System";
        public const string SystemFederationName = "System";

        public const string DefaultUserGroupName = "Public";
        public const string DefaultUserRoleName = "User";

        public const string TempDbName = "TEMP";
        public const string TempDbInstanceNamePattern = "[$DatabaseDefinition.Name]_[$ServerInstance.Machine.Name]_[$ServerInstance.Name]";
        public const string TempDbNamePattern = "[$DatabaseDefinition.Federation.Domain.Cluster.Name]_[$DatabaseDefinition.Name]";

        /// <summary>
        /// CodeDB is generated for each federation and contains functions
        /// </summary>
        public const string CodeDbName = "CODE";
        public const string CodeDbInstanceNamePattern = "[$DatabaseDefinition.Name]_[$ServerInstance.Machine.Name]_[$ServerInstance.Name]";
        public const string CodeDbNamePattern = "[$DatabaseDefinition.Federation.Name]_[$DatabaseDefinition.Name]";
        
        /// <summary>
        /// MyDB is generated for each federation and contains user data
        /// </summary>
        public const string UserDbName = "MYDB";
        public const string UserDbInstanceNamePattern = "[$DatabaseDefinition.Name]_[@Username]";
        public const string UserDbNamePattern = "[$DatabaseDefinition.Federation.Name]_[$DatabaseDefinition.Name]_[@Username]";

        public const string DefaultSchemaName = "dbo";

        internal static Map<EntityType, Type> EntityTypeMap = new Map<EntityType, Type>()
        {
            { EntityType.Cluster, typeof(Cluster) },
            { EntityType.DatabaseDefinition, typeof(DatabaseDefinition) },
            { EntityType.DatabaseInstance, typeof(DatabaseInstance) },
            { EntityType.DatabaseInstanceFile, typeof(DatabaseInstanceFile) },
            { EntityType.DatabaseInstanceFileGroup, typeof(DatabaseInstanceFileGroup) },
            { EntityType.DatabaseVersion, typeof(DatabaseVersion) },
            { EntityType.DeploymentPackage, typeof(DeploymentPackage) },
            { EntityType.DiskVolume, typeof(DiskVolume) },
            { EntityType.Domain, typeof(Domain) },
            { EntityType.Federation, typeof(Federation) },
            { EntityType.FileGroup, typeof(FileGroup) },
            { EntityType.JobDefinition, typeof(JobDefinition) },
            { EntityType.JobInstance, typeof(JobInstance) },
            { EntityType.JobInstanceDependency, typeof(JobInstanceDependency) },
            { EntityType.Machine, typeof(Machine) },
            { EntityType.MachineRole, typeof(MachineRole) },
            { EntityType.Partition, typeof(Partition) },
            { EntityType.QueueDefinition, typeof(QueueDefinition) },
            { EntityType.QueueInstance, typeof(QueueInstance) },
            { EntityType.RemoteDatabase, typeof(RemoteDatabase) },
            { EntityType.ServerInstance, typeof(ServerInstance) },
            { EntityType.ServerVersion, typeof(ServerVersion) },
            { EntityType.Slice, typeof(Slice) },
            { EntityType.User, typeof(User) },
            { EntityType.UserDatabaseInstance, typeof(UserDatabaseInstance) },
            { EntityType.UserIdentity, typeof(UserIdentity) },
            { EntityType.UserGroup, typeof(UserGroup) },
            { EntityType.UserGroupMembership, typeof(UserGroupMembership) },
            { EntityType.UserRole, typeof(UserRole) },
            { EntityType.UserRoleMembership, typeof(UserRoleMembership) },
        };

        public static readonly Dictionary<EntityType, string> EntityNames_Singular = new Dictionary<EntityType, string>()
        { 
            { EntityType.Unknown, EntityNames.Unknown_Singular},
            { EntityType.Cluster, EntityNames.Cluster_Singular},
            { EntityType.MachineRole, EntityNames.MachineRole_Singular},
            { EntityType.Machine, EntityNames.Machine_Singular},
            { EntityType.ServerVersion, EntityNames.ServerVersion_Singular},
            { EntityType.ServerInstance, EntityNames.ServerInstance_Singular},
            { EntityType.DiskVolume, EntityNames.DiskVolume_Singular},
            { EntityType.UserGroup, EntityNames.UserGroup_Singular},
            { EntityType.User, EntityNames.User_Singular},
            { EntityType.UserGroupMembership, EntityNames.UserGroupMembership_Singular},
            { EntityType.UserRole, EntityNames.UserRole_Singular},
            { EntityType.UserRoleMembership, EntityNames.UserRoleMembership_Singular},
            { EntityType.UserIdentity, EntityNames.UserIdentity_Singular},
            { EntityType.Domain, EntityNames.Domain_Singular},
            { EntityType.Federation, EntityNames.Federation_Singular},
            { EntityType.DatabaseDefinition, EntityNames.DatabaseDefinition_Singular},
            { EntityType.RemoteDatabase, EntityNames.RemoteDatabase_Singular},
            { EntityType.FileGroup, EntityNames.FileGroup_Singular},
            { EntityType.Slice, EntityNames.Slice_Singular},
            { EntityType.Partition, EntityNames.Partition_Singular},
            { EntityType.DatabaseVersion, EntityNames.DatabaseVersion_Singular},
            { EntityType.DeploymentPackage, EntityNames.DeploymentPackage_Singular},
            { EntityType.DatabaseInstance, EntityNames.DatabaseInstance_Singular},
            { EntityType.DatabaseInstanceFileGroup, EntityNames.DatabaseInstanceFileGroup_Singular},
            { EntityType.DatabaseInstanceFile, EntityNames.DatabaseInstanceFile_Singular},
            { EntityType.UserDatabaseInstance, EntityNames.UserDatabaseInstance_Singular},
            { EntityType.QueueDefinition, EntityNames.QueueDefinition_Singular},
            { EntityType.QueueInstance, EntityNames.QueueInstance_Singular},
            { EntityType.JobDefinition, EntityNames.JobDefinition_Singular},
            { EntityType.JobInstance, EntityNames.JobInstance_Singular},
            { EntityType.JobInstanceDependency, EntityNames.JobInstanceDependency_Singular},
        };

        public static readonly Dictionary<EntityType, string> EntityNames_Plural = new Dictionary<EntityType, string>()
        { 
            { EntityType.Unknown, EntityNames.Unknown_Plural},
            { EntityType.Cluster, EntityNames.Cluster_Plural},
            { EntityType.MachineRole, EntityNames.MachineRole_Plural},
            { EntityType.Machine, EntityNames.Machine_Plural},
            { EntityType.ServerVersion, EntityNames.ServerVersion_Plural},
            { EntityType.ServerInstance, EntityNames.ServerInstance_Plural},
            { EntityType.DiskVolume, EntityNames.DiskVolume_Plural},
            { EntityType.UserGroup, EntityNames.UserGroup_Plural},
            { EntityType.User, EntityNames.User_Plural},
            { EntityType.UserGroupMembership, EntityNames.UserGroupMembership_Plural},
            { EntityType.UserRole, EntityNames.UserRole_Plural},
            { EntityType.UserRoleMembership, EntityNames.UserRoleMembership_Plural},
            { EntityType.UserIdentity, EntityNames.UserIdentity_Plural},
            { EntityType.Domain, EntityNames.Domain_Plural},
            { EntityType.Federation, EntityNames.Federation_Plural},
            { EntityType.DatabaseDefinition, EntityNames.DatabaseDefinition_Plural},
            { EntityType.RemoteDatabase, EntityNames.RemoteDatabase_Plural},
            { EntityType.FileGroup, EntityNames.FileGroup_Plural},
            { EntityType.Slice, EntityNames.Slice_Plural},
            { EntityType.Partition, EntityNames.Partition_Plural},
            { EntityType.DatabaseVersion, EntityNames.DatabaseVersion_Plural},
            { EntityType.DeploymentPackage, EntityNames.DeploymentPackage_Plural},
            { EntityType.DatabaseInstance, EntityNames.DatabaseInstance_Plural},
            { EntityType.DatabaseInstanceFileGroup, EntityNames.DatabaseInstanceFileGroup_Plural},
            { EntityType.DatabaseInstanceFile, EntityNames.DatabaseInstanceFile_Plural},
            { EntityType.UserDatabaseInstance, EntityNames.UserDatabaseInstance_Plural},
            { EntityType.QueueDefinition, EntityNames.QueueDefinition_Plural},
            { EntityType.QueueInstance, EntityNames.QueueInstance_Plural},
            { EntityType.JobDefinition, EntityNames.JobDefinition_Plural},
            { EntityType.JobInstance, EntityNames.JobInstance_Plural},
            { EntityType.JobInstanceDependency, EntityNames.JobInstanceDependency_Plural},
        };
    }
}
