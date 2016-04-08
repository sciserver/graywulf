using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.AccessControl
{
    public class AccessCollection
    {
        #region Private member variables

        private Dictionary<string, AccessType> list;

        #endregion
        #region Properties

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

        public AccessType Set(string access, AccessType type)
        {
            // Deny has precedence over grant
            if (list.ContainsKey(access) && type == AccessType.Deny)
            {
                list[access] = AccessType.Deny;
            }
            else
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
