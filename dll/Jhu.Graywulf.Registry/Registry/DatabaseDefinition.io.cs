using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseDefinition : Entity
    {
        #region Database IO Functions

        public void LoadFileGroups(bool forceReload)
        {
            LoadChildren<FileGroup>(forceReload);
        }

        public void LoadSlices(bool forceReload)
        {
            LoadChildren<Slice>(forceReload);
        }

        public void LoadDeploymentPackages(bool forceReload)
        {
            LoadChildren<DeploymentPackage>(forceReload);
        }

        public void LoadDatabaseInstances(bool forceReload)
        {
            LoadChildren<DatabaseInstance>(forceReload);
        }

        public void LoadDatabaseVersions(bool forceReload)
        {
            LoadChildren<DatabaseVersion>(forceReload);
        }

        #endregion

        private IEnumerable<DatabaseInstance> FindDatabaseInstances(string databaseVersionName)
        {
            var sql = @"
WITH dvs AS
(
	SELECT dve.*, dv.*
	FROM DatabaseDefinition dd
	INNER JOIN Entity dve ON dve.ParentGuid = dd.EntityGuid
	INNER JOIN DatabaseVersion dv ON dve.Guid = dv.EntityGuid
	WHERE dd.EntityGuid = @databaseDefinitionGuid AND
	      (dve.Name = @databaseVersionName OR @databaseVersionName IS NULL)
), 
dis AS
(
	SELECT die.*, di.*
	FROM DatabaseDefinition dd
	INNER JOIN Entity die ON die.ParentGuid = dd.EntityGuid
	INNER JOIN DatabaseInstance di ON die.Guid = di.EntityGuid
	INNER JOIN EntityReference dvr ON dvr.EntityGuid = die.Guid AND dvr.ReferenceType = 3
	INNER JOIN dvs ON dvs.Guid = dvr.ReferencedEntityGuid
	INNER JOIN EntityReference sir ON sir.EntityGuid = die.Guid AND sir.ReferenceType = 1
	INNER JOIN Entity sie ON sie.Guid = sir.ReferencedEntityGuid
	INNER JOIN Entity mae ON mae.Guid = sie.ParentGuid
	WHERE 
		dd.EntityGuid = @databaseDefinitionGuid AND
		sie.DeploymentState = DeploymentState::Deployed AND sie.RunningState = RunningState::Running AND
		mae.DeploymentState = DeploymentState::Deployed AND mae.RunningState = RunningState::Running AND
        die.DeploymentState = DeploymentState::Deployed AND die.RunningState = RunningState::Attached AND
    	(@ShowHidden = 1 OR die.Hidden = 0) AND
	    (@ShowDeleted = 1 OR die.Deleted = 0)
)
SELECT * 
FROM dis
";

            using (var cmd = Context.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserReference.Guid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@databaseDefinitionGuid", SqlDbType.UniqueIdentifier).Value = this.Guid;
                cmd.Parameters.Add("@databaseVersionName", SqlDbType.NVarChar).Value = databaseVersionName != null ? (object)databaseVersionName : DBNull.Value;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var di = new DatabaseInstance();
                        di.Context = Context;
                        di.LoadFromDataReader(dr);
                        yield return di;
                    }
                    dr.Close();
                }
            }
        }
    }
}
