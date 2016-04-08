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





        // -----------------------------------------

        /*
        public virtual void Save()
        {
            Save(true);
        }

        public virtual void Save(bool checkAccess)
        {
            if (IsExisting)
            {
                if (checkAccess && !access.Update)
                {
                    throw Error.AccessDenied();
                }

                Create();
            }
            else
            {
                if (checkAccess && !access.Create)
                {
                    throw Error.AccessDenied();
                }

                Modify();
            }
        }*/

        #endregion

#if false
        /*
        protected bool CheckAccess()
        {
            // TODO
            return false;
        }

        // ------------------------------------------------------------

        /*
        protected virtual Rules GetAccess()
        {
            return Rules.Default;
        }*/


        /*
        private void Create()
        {
            /*using (var cmd = GetCreateCommand())
            {
                long retval = (long)Context.ExecuteCommandNonQuery(cmd);   
                
                if (retval == 0)
                {
                    // TODO
                    throw new Exception("Cannot create Footprint.");
                }
                else
                {
                    this.id = (long)cmd.Parameters["@NewID"].Value;
                }
            }*/
        }

        private void Modify()
        {
            /*using (var cmd = GetModifyCommand())
            {
                long retval = (long)Context.ExecuteCommandNonQuery(cmd); 

                if (retval == 0)
                {
                    // TODO
                    throw new Exception("Cannot update Footprint");
                }
            }*/
        }

        public virtual void Delete()
        {
            /*using (var cmd = GetDeleteCommand())
            {
                long retval = (long)Context.ExecuteCommandNonQuery(cmd);

                if (retval == 0)
                {
                    // TODO
                    throw new Exception("Cannot delete Footprint.");
                }
            }*/
        }

        public void Load()
        {
            /*using (var cmd = GetLoadCommand())
            {
                Context.ExecuteCommandAsSingleObject(cmd, this);
            }*/
        }
        
        protected virtual void AppendCreateModifyParameters(SqlCommand cmd)
        {
            /*cmd.Parameters.Add("@Owner", SqlDbType.NVarChar, 256).Value = owner;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 256).Value = name;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 256).Value = description;
            cmd.Parameters.Add("@Hidden", SqlDbType.Bit).Value = hidden;
            cmd.Parameters.Add("@ReadOnly", SqlDbType.Bit).Value = readOnly;
            cmd.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = comments;
            cmd.Parameters.Add("@Permissions", SqlDbType.NVarChar).Value = permissions.ToXml();*/
        }
#endif
    }
}
