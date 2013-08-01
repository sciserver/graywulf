using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class MachineRole : Entity
    {
        #region Database IO Functions

        public void LoadMachines(bool forceReload)
        {
            LoadChildren<Machine>(forceReload);
        }

        public void LoadServerVersions(bool forceReload)
        {
            LoadChildren<ServerVersion>(forceReload);
        }

        #endregion
    }
}
