using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Jhu.Graywulf.AccessControl
{
    public class Identity : IIdentity, ICloneable
    {
        #region Private member variables

        private string name;
        private string authenticationType;
        private bool isAuthenticated;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual string AuthenticationType
        {
            get { return authenticationType; }
            set { authenticationType = value; }
        }

        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
            set { isAuthenticated = value; }
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
            this.authenticationType = null;
            this.isAuthenticated = false;
        }

        private void CopyMembers(Identity old)
        {
            this.name = old.name;
            this.authenticationType = old.authenticationType;
            this.isAuthenticated = old.isAuthenticated;
        }

        public object Clone()
        {
            return new Identity(this);
        }

        #endregion

        public bool CompareByName(string other)
        {
            return EntityAcl.Comparer.Compare(this.name, other) == 0;
        }
    }
}
