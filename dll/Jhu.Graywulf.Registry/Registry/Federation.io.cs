using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class Federation : Entity
    {
        #region Database IO Functions

        public void LoadDatabaseDefinitions(bool forceReload)
        {
            LoadChildren<DatabaseDefinition>(forceReload);
        }

        public void LoadRemoteDatabases(bool forceReload)
        {
            LoadChildren<RemoteDatabase>(forceReload);
        }

        public void LoadJobDefinitions(bool forceReload)
        {
            LoadChildren<JobDefinition>(forceReload);
        }

        #endregion

        public IEnumerable<DatabaseInstance> FindDatabaseInstances()
        {
            var sql = @"
SELECT die.*, di.*
FROM DatabaseInstance di
INNER JOIN Entity die ON die.Guid = di.EntityGuid
INNER JOIN DatabaseDefinition dd ON dd.EntityGuid = die.ParentGuid
INNER JOIN Entity dde ON dde.Guid = dd.EntityGuid
INNER JOIN Entity fe ON fe.Guid = dde.ParentGuid
WHERE
    fe.Guid = @FederationGuid AND
    dde.DeploymentState = DeploymentState::Deployed AND dde.RunningState = RunningState::Running AND
	die.DeploymentState = DeploymentState::Deployed AND die.RunningState = RunningState::Attached AND
    (@ShowHidden = 1 OR die.Hidden = 0) AND
	(@ShowDeleted = 1 OR die.Deleted = 0)
";

            using (var cmd = RegistryContext.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = RegistryContext.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = RegistryContext.ShowDeleted;
                cmd.Parameters.Add("@FederationGuid", SqlDbType.UniqueIdentifier).Value = this.Guid;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var di = new DatabaseInstance();
                        di.RegistryContext = RegistryContext;
                        di.LoadFromDataReader(dr);
                        yield return di;
                    }
                    dr.Close();
                }
            }
        }
    }
}
