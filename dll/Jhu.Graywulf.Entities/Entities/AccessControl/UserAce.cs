using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.AccessControl
{
    public sealed class UserAce : EntityAce, IComparable, ICloneable
    {
        #region Constructors and initializers

        public UserAce()
        {
            InitializeMembers();
        }

        public UserAce(string name, string permission, AccessType type)
            : base(name, permission, type)
        {
            InitializeMembers();
        }

        public UserAce(UserAce old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(UserAce old)
        {
        }

        public override object Clone()
        {
            return new UserAce(this);
        }

        #endregion
        #region Interface implementations

        public override int CompareTo(object obj)
        {
            if (obj == null || obj is GroupAce)
            {
                return 1;
            }
            else
            {
                return EntityAcl.Comparer.Compare(this.Name, ((UserAce)obj).Name);
            }
        }

        #endregion
    }
}
