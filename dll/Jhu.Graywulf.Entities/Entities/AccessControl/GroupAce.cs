using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.AccessControl
{
    public sealed class GroupAce : EntityAce, IComparable, ICloneable
    {
        #region Private member variables

        private string role;

        #endregion
        #region Properties

        public string Role
        {
            get { return role; }
            set { role = value; }
        }

        #endregion
        #region Constructors and initializers

        public GroupAce()
        {
            InitializeMembers();
        }

        public GroupAce(string name, string role, string permission, AccessType type)
            : base(name, permission, type)
        {
            InitializeMembers();

            this.role = role;
        }

        public GroupAce(GroupAce old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.role = null;
        }

        private void CopyMembers(GroupAce old)
        {
            this.role = old.role;
        }

        public override object Clone()
        {
            return new GroupAce(this);
        }

        #endregion
        #region Interface implementations

        public override int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            else if (obj is UserAce)
            {
                return -1;
            }
            else
            {
                int res = EntityAcl.Comparer.Compare(this.Name, ((GroupAce)obj).Name);

                if (res == 0)
                {
                    res = EntityAcl.Comparer.Compare(this.Role, ((GroupAce)obj).Role);
                }

                return res;
            }
        }

        #endregion
    }
}
