using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.AccessControl
{
    public class PrincipalRole : ICloneable
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

        public PrincipalRole()
        {
            InitializeMembers();
        }

        public PrincipalRole(string group, string role)
        {
            InitializeMembers();

            this.group = group;
            this.role = role;
        }

        public PrincipalRole(PrincipalRole old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.group = null;
            this.role = null;
        }

        private void CopyMembers(PrincipalRole old)
        {
            this.group = old.group;
            this.role = old.role;
        }

        public object Clone()
        {
            return new PrincipalRole(this);
        }

        #endregion
    }
}
