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

        public void LoadUserDatabaseInstances(bool forceReload)
        {
            LoadChildren<UserDatabaseInstance>(forceReload);
        }

        #endregion
        #region Group membership

        public UserGroupMembership MakeMemberOf(Guid userGroupGuid)
        {
            if (IsMemberOf(userGroupGuid))
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

            return ugm;
        }

        public void RemoveMemberOf(Guid userGroupGuid)
        {
            LoadUserGroupMemberships(true);

            foreach (var ugm in UserGroupMemberships.Values)
            {
                if (ugm.Guid == userGroupGuid)
                {
                    ugm.Delete();
                }
            }
        }

        public bool IsMemberOf(Guid userGroupGuid)
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
    }
}
