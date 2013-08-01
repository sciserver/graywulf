using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class Machine : Entity
    {
        #region Database IO Functions

        public void LoadDiskVolumes(bool forceReload)
        {
            LoadChildren<DiskVolume>(forceReload);
        }

        public void LoadServerInstances(bool forceReload)
        {
            LoadChildren<ServerInstance>(forceReload);
        }

        public void LoadQueueInstances(bool forceReload)
        {
            LoadChildren<QueueInstance>(forceReload);
        }

        #endregion
    }
}
