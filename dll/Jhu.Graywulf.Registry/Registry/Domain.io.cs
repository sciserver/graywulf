using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class Domain : Entity
    {
        #region Database IO Functions

        public void LoadFederations(bool forceReload)
        {
            LoadChildren<Federation>(forceReload);
        }

        public void LoadUsers(bool forceReload)
        {
            LoadChildren<User>(forceReload);
        }

        public void LoadUserGroups(bool forceReload)
        {
            LoadChildren<UserGroup>(forceReload);
        }

        public void LoadUserRoles(bool forceReload)
        {
            LoadChildren<UserRole>(forceReload);
        }

        #endregion
    }
}
