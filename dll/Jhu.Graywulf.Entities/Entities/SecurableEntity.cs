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
        private Access access;

        #endregion
        #region Properties

        public EntityAcl Permissions
        {
            get { return acl; }
            internal set { acl = value; }
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

            this.acl.Owner = context.Identity.Name;
        }

        protected SecurableEntity(SecurableEntity old)
        {
            CopyMembers(old);
        }

        [OnDeserialized]
        private void InitializeMembers(StreamingContext context)
        {
            this.acl = EntityAcl.Default;
        }

        private void CopyMembers(SecurableEntity old)
        {
            this.acl = new EntityAcl(old.acl);
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

        protected override void OnLoaded()
        {
            base.OnLoaded();

            access = acl.EvaluateAccess(Context.Identity);
        }

        protected override void OnModifying(ref bool cancel)
        {
            base.OnModifying(ref cancel);

            // TODO: test permissions
        }

        protected override void OnDeleting(ref bool cancel)
        {
            base.OnDeleting(ref cancel);

            // TODO: test permissions
        }
        
        #endregion
    }
}
