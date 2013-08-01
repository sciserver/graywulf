using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin
{
    /// <summary>
    /// Summary description for EntityExtensions
    /// </summary>
    public static class EntityExtensions
    {
        public static string GetDetailsUrl(this Entity entity)
        {
            EntityGroup group = GetDefaultEntityGroup(entity.EntityType);
            return GetDetailsUrl(entity, group);
        }

        public static string GetDetailsUrl(this Entity entity, EntityGroup group)
        {
            return GetDetailsUrl(entity.EntityType, group, entity.Guid);
        }

        public static string GetDetailsUrl(EntityType entityType, EntityGroup group, Guid guid)
        {
            string url = string.Empty;

            OverrideEntityGroup(entityType, ref group);

            url = String.Format(
                "~/{0}/{1}Details.aspx?guid={2}",
                group.ToString().ToLower(),
                entityType.ToString(),
                guid.ToString());

            return url;
        }

        public static string GetFormUrl(this Entity entity)
        {
            EntityGroup group = GetDefaultEntityGroup(entity.EntityType);

            string url = String.Format(
                "~/{0}/{1}Form.aspx?guid={2}",
                group.ToString().ToLower(),
                entity.EntityType.ToString(),
                entity.Guid.ToString());

            return url;
        }

        public static string GetChildDetailsUrl(this Entity entity, EntityGroup group, EntityType entityType, Guid guid)
        {
            string url = String.Format(
                "~/{0}/{1}Details.aspx?guid={2}",
                group.ToString().ToLower(),
                entityType.ToString(),
                guid.ToString());

            return url;
        }

        public static string GetNewChildFormUrl(this Entity entity, EntityType entityType)
        {
            EntityGroup group = GetDefaultEntityGroup(entityType);

            string url = String.Format(
                "~/{0}/{1}Form.aspx?parentGuid={2}",
                group.ToString().ToLower(),
                entityType.ToString(),
                entity.Guid.ToString());

            return url;
        }

        public static string GetParentDetailsUrl(this Entity entity)
        {
            return GetParentDetailsUrl(entity, GetDefaultEntityGroup(entity.EntityType));
        }

        public static string GetParentDetailsUrl(this Entity entity, EntityGroup group)
        {
            string url = String.Format(
                "~/{0}/{1}Details.aspx?guid={2}",
                group.ToString(),
                entity.ParentReference.Value.EntityType.ToString(),
                entity.ParentReference.Guid.ToString());

            return url;
        }

        /*
        public static string GetDeleteFormUrl(this Entity entity)
        {
            return "~/Common/EntityDelete.aspx?guid=" + entity.Guid.ToString();
        }

        public static string GetDiscoverUrl(this Entity entity)
        {
            return "~/Common/EntityDiscover.aspx?guid=" + entity.Guid.ToString();
        }*/

        /// <summary>
        /// Returns the default entity group.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function is used by the web interface.
        /// </remarks>
        private static EntityGroup GetDefaultEntityGroup(EntityType entityType)
        {
            EntityGroup group;

            switch (entityType)
            {
                case EntityType.Cluster:
                case EntityType.MachineRole:
                case EntityType.ServerVersion:
                case EntityType.Machine:
                case EntityType.ServerInstance:
                case EntityType.DiskVolume:
                    group = EntityGroup.Cluster;
                    break;
                case EntityType.Domain:
                case EntityType.Federation:
                case EntityType.DatabaseDefinition:
                case EntityType.RemoteDatabase:
                case EntityType.FileGroup:
                case EntityType.DeploymentPackage:
                case EntityType.DatabaseVersion:
                case EntityType.Slice:
                case EntityType.Partition:
                case EntityType.DistributedPartitionedView:
                    group = EntityGroup.Federation;
                    break;
                case EntityType.DatabaseInstance:
                case EntityType.DatabaseInstanceFileGroup:
                case EntityType.DatabaseInstanceFile:
                case EntityType.UserDatabaseInstance:
                    group = EntityGroup.Layout;
                    break;
                case EntityType.QueueDefinition:
                case EntityType.QueueInstance:
                case EntityType.JobDefinition:
                case EntityType.JobInstance:
                    group = EntityGroup.Jobs;
                    break;
                case EntityType.UserGroup:
                case EntityType.User:
                    group = EntityGroup.Security;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return group;
        }

        /// <summary>
        /// Overrides the default entity group
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="group"></param>
        /// <remarks>
        /// This function is used by the web interface. Certain entities are listed only
        /// under one group (Hardware, Federation etc.) and when clicked, the administrator
        /// has to be redirected to the correct web page.
        /// </remarks>
        private static void OverrideEntityGroup(EntityType entityType, ref EntityGroup group)
        {
            switch (entityType)
            {
                case EntityType.Cluster:
                case EntityType.MachineRole:
                case EntityType.ServerVersion:
                case EntityType.Machine:
                case EntityType.Domain:
                case EntityType.Federation:
                case EntityType.DatabaseDefinition:
                case EntityType.RemoteDatabase:
                case EntityType.FileGroup:
                case EntityType.DeploymentPackage:
                case EntityType.DatabaseVersion:
                case EntityType.DistributedPartitionedView:
                    break;
                case EntityType.ServerInstance:
                case EntityType.DiskVolume:
                    group = EntityGroup.Cluster;
                    break;
                case EntityType.Slice:
                case EntityType.Partition:
                    group = EntityGroup.Federation;
                    break;
                case EntityType.DatabaseInstance:
                case EntityType.DatabaseInstanceFileGroup:
                case EntityType.DatabaseInstanceFile:
                case EntityType.UserDatabaseInstance:
                    group = EntityGroup.Layout;
                    break;
                case EntityType.QueueDefinition:
                case EntityType.QueueInstance:
                case EntityType.JobDefinition:
                case EntityType.JobInstance:
                    group = EntityGroup.Jobs;
                    break;
                case EntityType.UserGroup:
                case EntityType.User:
                    group = EntityGroup.Security;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}