using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.AccessControl
{
    public class Identity : ICloneable
    {
        #region Private member variables

        private string name;
        private bool isAuthenticated;
        private IdentityRoleCollection roles;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
            set { isAuthenticated = value; }
        }
        
        public IdentityRoleCollection Roles
        {
            get { return roles; }
        }

        #endregion
        #region Constructors and initializers

        public Identity()
        {
            InitializeMembers();
        }

        public Identity(Identity old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.isAuthenticated = false;
            this.roles = new IdentityRoleCollection();
        }

        private void CopyMembers(Identity old)
        {
            this.name = old.name;
            this.isAuthenticated = true;
            this.roles = new IdentityRoleCollection(old.roles);
        }

        public object Clone()
        {
            return new Identity(this);
        }

        #endregion
    }
}
