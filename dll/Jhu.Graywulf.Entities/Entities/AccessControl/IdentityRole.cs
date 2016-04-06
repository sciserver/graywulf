using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.AccessControl
{
    public class IdentityRole : ICloneable
    {
        #region Private member variables

        private string group;
        private string role;

        #endregion
        #region Properties

        public string Group
        {
            get { return group; }
            set { group = value; }
        }

        public string Role
        {
            get { return role; }
            set { role = value; }
        }

        #endregion
        #region Constructors and initializers

        public IdentityRole()
        {
            InitializeMembers();
        }

        public IdentityRole(string group, string role)
        {
            InitializeMembers();

            this.group = group;
            this.role = role;
        }

        public IdentityRole(IdentityRole old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.group = null;
            this.role = null;
        }

        private void CopyMembers(IdentityRole old)
        {
            this.group = old.group;
            this.role = old.role;
        }

        public object Clone()
        {
            return new IdentityRole(this);
        }

        #endregion
    }
}
