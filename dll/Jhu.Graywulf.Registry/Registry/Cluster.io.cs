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
    }
}
