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

        public void LoadUserRoleMemberships(bool forceReload)
        {
            LoadChildren<UserRoleMembership>(forceReload);
        }

        public void LoadUserIdentities(bool forceReload)
        {
            LoadChildren<UserIdentity>(forceReload);
        }

        #endregion
        #region Group membership

        public UserGroupMembership AddToGroup(Guid userGroupGuid)
        {
            if (IsMemberOfGroup(userGroupGuid))
            {
                throw new Exception("Already a member of ..."); // TODO ***
            }

            var ug = new UserGroup(Context);
            ug.Guid = userGroupGuid;
            ug.Load();

            var ugm = new UserGroupMembership(this);
            ugm.Name = ug.Name;
            ugm.UserGroup = ug;
            ugm.Save();

            LoadUserGroupMemberships(true);

            return ugm;
        }

        public void RemoveFromGroup(Guid userGroupGuid)
        {
            LoadUserGroupMemberships(true);

            foreach (var ugm in UserGroupMemberships.Values)
            {
                if (ugm.Guid == userGroupGuid)
                {
                    ugm.Delete();
                }
            }

            LoadUserGroupMemberships(true);
        }

        public bool IsMemberOfGroup(Guid userGroupGuid)
        {
            LoadUserGroupMemberships(true);

            foreach (var ugm in UserGroupMemberships.Values)
            {
                if (ugm.Guid == userGroupGuid)
                {
                    return true;
                }
            }

            return false;

        }

        #endregion
        #region Role membership
        
        public UserRoleMembership AddToRole(Guid userRoleGuid)
        {
            if (IsMemberOfRole(userRoleGuid))
            {
                throw new Exception("Already a member of ..."); // TODO ***
            }

            var ur = new UserRole(Context);
            ur.Guid = userRoleGuid;
            ur.Load();

            var ugm = new UserRoleMembership(this);
            ugm.Name = ur.Name;
            ugm.UserRole = ur;
            ugm.Save();

            LoadUserRoleMemberships(true);

            return ugm;
        }

        public void RemoveFromRole(Guid userRoleGuid)
        {
            LoadUserRoleMemberships(true);

            foreach (var urm in UserRoleMemberships.Values)
            {
                if (urm.Guid == userRoleGuid)
                {
                    urm.Delete();
                }
            }

            LoadUserRoleMemberships(true);
        }

        public bool IsMemberOfRole(Guid userRoleGuid)
        {
            LoadUserRoleMemberships(true);

            foreach (var urm in UserRoleMemberships.Values)
            {
                if (urm.Guid == userRoleGuid)
                {
                    return true;
                }
            }

            return false;

        }

        #endregion
    }
}
