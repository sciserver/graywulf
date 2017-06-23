using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class Cluster : Entity
    {
        #region Database IO Functions

        public void LoadMachineRoles(bool forceReload)
        {
            LoadChildren<MachineRole>(forceReload);
        }

        public void LoadDomains(bool forceReload)
        {
            LoadChildren<Domain>(forceReload);
        }

        public void LoadQueueDefinitions(bool forceReload)
        {
            LoadChildren<QueueDefinition>(forceReload);
        }

        #endregion

        public IEnumerable<ServerInstance> FindServerInstances()
        {
            var sql = @"
SELECT sie.*, si.*
FROM ServerInstance si
INNER JOIN Entity sie ON sie.Guid = si.EntityGuid
INNER JOIN Machine m ON m.EntityGuid = sie.ParentGuid
INNER JOIN Entity me ON me.Guid = m.EntityGuid
INNER JOIN Entity mre ON mre.Guid = me.ParentGuid
INNER JOIN Entity ce ON ce.Guid = mre.ParentGuid
WHERE
    ce.Guid = @ClusterGuid AND
    me.DeploymentState = DeploymentState::Deployed AND me.RunningState = RunningState::Running AND
	sie.DeploymentState = DeploymentState::Deployed AND sie.RunningState = RunningState::Running AND
    (@ShowHidden = 1 OR sie.Hidden = 0) AND
	(@ShowDeleted = 1 OR sie.Deleted = 0)
";

            using (var cmd = Context.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserReference.Guid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@ClusterGuid", SqlDbType.UniqueIdentifier).Value = this.Guid;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var si = new ServerInstance();
                        si.Context = Context;
                        si.LoadFromDataReader(dr);
                        yield return si;
                    }
                    dr.Close();
                }
            }
        }
    }
}
