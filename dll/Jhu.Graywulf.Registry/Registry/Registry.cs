using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    [XmlRoot(ElementName = "Registry", Namespace = "")]
    [XmlType(TypeName = "Registry", Namespace = "")]
    public class Registry
    {
        [XmlArrayItem(typeof(Cluster))]
        [XmlArrayItem(typeof(DatabaseDefinition))]
        [XmlArrayItem(typeof(DatabaseInstance))]
        [XmlArrayItem(typeof(DatabaseInstanceFile))]
        [XmlArrayItem(typeof(DatabaseInstanceFileGroup))]
        [XmlArrayItem(typeof(DatabaseVersion))]
        [XmlArrayItem(typeof(DeploymentPackage))]
        [XmlArrayItem(typeof(DiskGroup))]
        [XmlArrayItem(typeof(DiskVolume))]
        [XmlArrayItem(typeof(Domain))]
        [XmlArrayItem(typeof(Federation))]
        [XmlArrayItem(typeof(FileGroup))]
        [XmlArrayItem(typeof(JobDefinition))]
        [XmlArrayItem(typeof(JobInstance))]
        [XmlArrayItem(typeof(JobInstanceDependency))]
        [XmlArrayItem(typeof(Machine))]
        [XmlArrayItem(typeof(MachineRole))]
        [XmlArrayItem(typeof(Partition))]
        [XmlArrayItem(typeof(QueueDefinition))]
        [XmlArrayItem(typeof(QueueInstance))]
        [XmlArrayItem(typeof(ServerInstance))]
        [XmlArrayItem(typeof(ServerInstanceDiskGroup))]
        [XmlArrayItem(typeof(ServerVersion))]
        [XmlArrayItem(typeof(Slice))]
        [XmlArrayItem(typeof(User))]
        [XmlArrayItem(typeof(UserDatabaseInstance))]
        [XmlArrayItem(typeof(UserGroup))]
        [XmlArrayItem(typeof(UserGroupMembership))]
        [XmlArrayItem(typeof(UserRole))]
        [XmlArrayItem(typeof(UserIdentity))]
        public Entity[] Entities;
    }
}
