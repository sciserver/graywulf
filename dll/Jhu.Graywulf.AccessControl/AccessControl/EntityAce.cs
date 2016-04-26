using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Jhu.Graywulf.AccessControl
{
    public abstract class EntityAce : IComparable, ICloneable
    {
        #region Private member variables

        private string name;
        private string access;
        private AccessType type;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Access
        {
            get { return access; }
            set { access = value; }
        }

        public AccessType Type
        {
            get { return type; }
            set { type = value; }
        }

        internal virtual string UniqueKey
        {
            get
            {
                return name + "|" + access;
            }
        }

        #endregion
        #region Constructors and initializers

        public EntityAce()
        {
            InitializeMembers();
        }

        public EntityAce(string name, string access, AccessType type)
        {
            InitializeMembers();

            this.name = name;
            this.access = access;
            this.type = type;
        }

        public EntityAce(EntityAce old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.access = null;
            this.type = AccessType.Undefined;
        }

        private void CopyMembers(EntityAce old)
        {
            this.name = old.name;
            this.access = old.access;
            this.type = old.type;
        }

        public abstract object Clone();

        #endregion
        #region Interface implementations

        public abstract int CompareTo(object obj);

        #endregion
    }
}
