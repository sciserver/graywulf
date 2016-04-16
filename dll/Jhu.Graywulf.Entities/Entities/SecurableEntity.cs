using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Entities.Mapping;
using Jhu.Graywulf.Entities.AccessControl;

namespace Jhu.Graywulf.Entities
{
    public abstract class SecurableEntity : Entity
    {
        #region Private member variables

        private EntityAcl acl;
        private AccessCollection access;

        #endregion
        #region Properties

        public EntityAcl Permissions
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
                    access = acl.EvaluateAccess(Context.Identity);
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
        }

        protected SecurableEntity(SecurableEntity old)
        {
            CopyMembers(old);
        }

        [OnDeserialized]
        private void InitializeMembers(StreamingContext context)
        {
            this.acl = EntityAcl.Default;
            this.access = null;
        }

        private void CopyMembers(SecurableEntity old)
        {
            this.acl = new EntityAcl(old.acl);
            this.access = new AccessCollection(old.access);
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

        protected override void OnLoaded(EntityEventArgs e)
        {
            base.OnLoaded(e);

            Access.EnsureRead();
        }

        protected override void OnCreating(EntityEventArgs e)
        {
            if (this.acl.Owner == null)
            {
                this.acl.Owner = Context.Identity.Name;
            }

            base.OnCreating(e);
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
