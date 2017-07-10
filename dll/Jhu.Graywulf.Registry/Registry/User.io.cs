using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class User : Entity
    {
        #region Database IO Functions

        public void LoadUserGroupMemberships(bool forceReload)
        {
            LoadChildren<UserGroupMembership>(forceReload);
        }

        public void LoadUserIdentities(bool forceReload)
        {
            LoadChildren<UserIdentity>(forceReload);
        }

        #endregion
        #region Group membership

        public UserGroupMembership AddToGroup(Guid userGroupGuid, Guid userRoleGuid)
        {
            if (IsMemberOfGroup(userGroupGuid, userRoleGuid))
            {
                throw new Exception("Already a member of ..."); // TODO ***
            }

            var group = new UserGroup(RegistryContext);
            group.Guid = userGroupGuid;
            group.Load();

            var role = new UserRole(RegistryContext);
            role.Guid = userRoleGuid;
            role.Load();

            var ugm = new UserGroupMembership(this)
            {
                Name = group.Name + "_" + role.Name,
                UserGroup = group,
                UserRole = role,
            };
            ugm.Save();

            LoadUserGroupMemberships(true);

            return ugm;
        }

        public void RemoveFromGroup(Guid userGroupMembershipGuid)
        {
            throw new NotImplementedException();

            /*
            LoadUserGroupMemberships(true);

            foreach (var ugm in UserGroupMemberships.Values)
            {
                if (ugm.Guid == userGroupMembershipGuid)
                {
                    ugm.Delete();
                }
            }

            LoadUserGroupMemberships(true);
            */
        }

        public bool IsMemberOfGroup(Guid userGroupGuid, Guid userRoleGuid)
        {
            LoadUserGroupMemberships(true);

            foreach (var ugm in UserGroupMemberships.Values)
            {
                if (ugm.UserGroupReference.Guid == userGroupGuid && 
                    ugm.UserRoleReference.Guid == userRoleGuid)
                {
                    return true;
                }
            }

            return false;

        }

        #endregion
    }
}
