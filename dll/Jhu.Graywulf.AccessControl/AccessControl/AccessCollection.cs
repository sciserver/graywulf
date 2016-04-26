using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.AccessControl
{
    public class AccessCollection
    {
        #region Private member variables

        private bool isAuthenticated;
        private bool isOwner;
        private Dictionary<string, AccessType> list;

        #endregion
        #region Properties

        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
            internal set { isAuthenticated = value; }
        }

        public bool IsOwner
        {
            get { return isOwner; }
            internal set { isOwner = value; }
        }

        public AccessType this[string access]
        {
            get
            {
                return list[access];
            }
        }

        #endregion
        #region Constructors and initializers

        public AccessCollection()
        {
            InitializeMembers();
        }

        public AccessCollection(AccessCollection old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            list = new Dictionary<string, AccessType>(EntityAcl.Comparer);
        }

        private void CopyMembers(AccessCollection old)
        {
            list = new Dictionary<string, AccessType>(old.list);
        }

        #endregion

        internal AccessType Set(string access, AccessType type)
        {
            bool contains = list.ContainsKey(access);

            // Deny has precedence over grant
            if (contains && type == AccessType.Deny)
            {
                list[access] = AccessType.Deny;
            }
            else if (!contains)
            {
                list.Add(access, type);
            }

            return list[access];
        }

        public bool Can(string access)
        {
            return list.ContainsKey(access) && list[access] == AccessType.Grant;
        }

        public bool CanCreate()
        {
            return Can(DefaultAccess.All) || Can(DefaultAccess.Create);
        }

        public bool CanRead()
        {
            return Can(DefaultAccess.All) || Can(DefaultAccess.Read);
        }

        public bool CanUpdate()
        {
            return Can(DefaultAccess.All) || Can(DefaultAccess.Update);
        }

        public bool CanDelete()
        {
            return Can(DefaultAccess.All) || Can(DefaultAccess.Delete);
        }

        public void Ensure(string access)
        {
            if (!Can(access))
            {
                throw Error.AccessDenied();
            }
        }

        public void EnsureCreate()
        {
            if (!CanCreate())
            {
                throw Error.AccessDenied();
            }
        }

        public void EnsureRead()
        {
            if (!CanRead())
            {
                throw Error.AccessDenied();
            }
        }

        public void EnsureUpdate()
        {
            if (!CanUpdate())
            {
                throw Error.AccessDenied();
            }
        }

        public void EnsureDelete()
        {
            if (!CanDelete())
            {
                throw Error.AccessDenied();
            }
        }
    }
}
