using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class User : Entity
    {
        #region Database IO Functions

        public void LoadUserDatabaseInstances(bool forceReload)
        {
            LoadChildren<UserDatabaseInstance>(forceReload);
        }

        /// <summary>
        /// Loads the user from the database. Also loads user group associations.
        /// </summary>
        protected override void OnLoaded()
        {
            base.OnLoaded();

            LoadUserGroups();
        }

        #endregion
        #region Group membership

        private void LoadUserGroups()
        {
            this.userGroupReferences = new List<EntityProperty<UserGroup>>();

            var sql = "spFindUserGroupMembership";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = this.Guid;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var p = new EntityProperty<UserGroup>(Context);

                        p.Guid = dr.GetGuid(1);

                        userGroupReferences.Add(p);
                    }
                }
            }
        }

        private void SaveUserGroups()
        {
            var sql = "spCreateUserGroupMembership";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = this.Guid;
                cmd.Parameters.Add("@UserGroupGuid", SqlDbType.UniqueIdentifier);

                foreach (var ug in userGroupReferences)
                {
                    ug.Context = this.Context;
                    cmd.Parameters["@UserGroupGuid"].Value = ug.Guid;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void DeleteParameters()
        {
            string sql = "spDeleteUserGroupMembership";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = this.Guid;

                cmd.ExecuteNonQuery();
            }
        }

        public void MakeMemberOf(Guid userGroupGuid)
        {
            var ug = new EntityProperty<UserGroup>(Context);
            ug.Guid = userGroupGuid;
            userGroupReferences.Add(ug);
        }

        public void RemoveMemberOf(Guid userGroupGuid)
        {
            userGroupReferences = new List<EntityProperty<UserGroup>>(
                userGroupReferences.Where(ug => ug.Guid != userGroupGuid));
        }

        public bool IsMemberOf(Guid userGroupGuid)
        {
            return userGroupReferences.Where(ug => ug.Guid == userGroupGuid).FirstOrDefault() != null;
        }

        

        #endregion
    }
}
