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
    }
}
