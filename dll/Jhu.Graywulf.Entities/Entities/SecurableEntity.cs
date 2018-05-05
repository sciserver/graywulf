using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Entities.Mapping;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Entities
{
    public abstract class SecurableEntity : Entity
    {
        #region Private member variables

        private AccessControlList acl;
        private AccessCollection access;

        #endregion
        #region Properties

        public AccessControlList Permissions
        {
            get { return acl; }
            internal set { acl = value; }
        }

        internal AccessCollection Access
        {
            get
            {
                if (access == null || acl.IsDirty)
                {
                    access = acl.EvaluateAccess(Context.Principal);
                }

                return access;
            }
        }

        #endregion
        #region Constructors and initializers

        protected SecurableEntity()
        {
            InitializeMembers(new StreamingContext());
        }

        protected SecurableEntity(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());

            this.acl.Owner = context.Principal.Identity.Name;
        }

        protected SecurableEntity(SecurableEntity old)
        {
            CopyMembers(old);
        }

        [OnDeserialized]
        private void InitializeMembers(StreamingContext context)
        {
            this.acl = AccessControlList.Default;
            this.access = null;
        }

        private void CopyMembers(SecurableEntity old)
        {
            this.acl = old.acl == null ? null : new AccessControlList(old.acl);
            this.access = old.access == null ? null : new AccessCollection(old.access);
        }

        #endregion
        #region Database column functions

        protected override IEnumerable<Mapping.DbColumn> GetColumnList(Mapping.DbColumnBinding binding)
        {
            var columns = base.GetColumnList(binding);
            columns = columns.Concat(new[] { DbColumn.AclColumn });
            return columns;
        }

        #endregion
        #region CRUD functions

        protected virtual void OnSetDefaultPermissions(EntityEventArgs e)
        {
            if (Permissions.Owner == null)
            {
                Permissions.Owner = Context.Principal.Identity.Name;
            }
        }

        protected override void OnSaving(EntityEventArgs e)
        {
            OnSetDefaultPermissions(e);

            base.OnSaving(e);
        }

        protected override void OnLoaded(EntityEventArgs e)
        {
            base.OnLoaded(e);

            Access.EnsureRead();
        }

        protected override void OnModifying(EntityEventArgs e)
        {
            Access.EnsureUpdate();

            base.OnModifying(e);
        }

        protected override void OnDeleting(EntityEventArgs e)
        {
            Access.EnsureDelete();

            base.OnDeleting(e);
        }
        
        #endregion
    }
}
