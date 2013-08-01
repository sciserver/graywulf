using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Jhu.Graywulf.Registry
{
    public class Constants
    {
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
        public const string DiskVolumeLocalPath = @"C:\Data\[$Name]\";
        public const string DiskVolumeUncPath = @"\\[$Parent.HostName]\Data\[$Name]\";

        public const string SlicedDatabaseInstanceNamePattern = "[$DatabaseDefinition.Name]_[$Slice.Name]_[$DatabaseVersion.Name]";
        public const string SlicedDatabaseNamePattern = "[$DatabaseDefinition.Federation.Name]_[$DatabaseDefinition.Name]_[$Slice.Name]_[$DatabaseVersion.Name]";
        public const string MirroredDatabaseInstanceNamePattern = "[$DatabaseDefinition.Name]_[$DatabaseVersion.Name]_[$Number]";
        public const string MirroredDatabaseNamePattern = "[$DatabaseDefinition.Federation.Name]_[$DatabaseDefinition.Name]_[$DatabaseVersion.Name]";
        public const string MonolithicDatabaseInstanceNamePattern = "[$DatabaseDefinition.Name]_[$DatabaseVersion.Name]";
        public const string MonolithicDatabaseNamePattern = "[$DatabaseDefinition.Federation.Name]_[$DatabaseDefinition.Name]_[$DatabaseVersion.Name]";
        public const string PrimaryFileGroupName = "PRIMARY";
        public const string LogFileGroupName = "LOG";

        public const string FullSliceName = "FULL";

        public const string StandardUserGroupName = "StandardUser";

        public const string TempDbName = "TEMP";
        public const string TempDbInstanceNamePattern = "[$ServerInstance.Machine.Name]_[$ServerInstance.Name]";
        public const string TempDbNamePattern = "[$DatabaseDefinition.Cluster.Name]_[$DatabaseDefinition.Name]";

        public const string CodeDbName = "CODE";
        public const string CodeDbInstanceNamePattern = "[$ServerInstance.Machine.Name]_[$ServerInstance.Name]";
        public const string CodeDbNamePattern = "[$DatabaseDefinition.Cluster.Name]_[$DatabaseDefinition.Name]";
        
        public const string MyDbName = "MYDB";
        public const string MyDbInstanceNamePattern = "{0}_[@Username]";
        public const string MyDbNamePattern = "{0}_{1}_[@Username]";        

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
            { EntityType.MyDBDatabaseInstance, EntityNames.MyDBDatabaseInstance_Singular},
            { EntityType.QueueDefinition, EntityNames.QueueDefinition_Singular},
            { EntityType.QueueInstance, EntityNames.QueueInstance_Singular},
            { EntityType.JobDefinition, EntityNames.JobDefinition_Singular},
            { EntityType.JobInstance, EntityNames.JobInstance_Singular},
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
            { EntityType.MyDBDatabaseInstance, EntityNames.MyDBDatabaseInstance_Plural},
            { EntityType.QueueDefinition, EntityNames.QueueDefinition_Plural},
            { EntityType.QueueInstance, EntityNames.QueueInstance_Plural},
            { EntityType.JobDefinition, EntityNames.JobDefinition_Plural},
            { EntityType.JobInstance, EntityNames.JobInstance_Plural},
        };
    }
}
