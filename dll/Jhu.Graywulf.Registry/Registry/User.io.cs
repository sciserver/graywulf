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
        public override void Load()
        {
            base.Load();

            LoadUserGroups();
        }

        private void LoadUserGroups()
        {
            this.userGroups = new List<UserGroup>();

            var sql = "spFindUserGroup_byUser";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = this.Guid;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var ug = new UserGroup(Context);
                        ug.LoadFromDataReader(dr);

                        userGroups.Add(ug);
                    }
                }
            }
        }

        #endregion
    }
}
