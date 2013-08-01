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

        public override Entity Copy(Entity parent, bool prefixName)
        {
            var m = CreateCopy(parent, prefixName);
            m.Save();

            LoadDiskVolumes(false);
            foreach (DiskVolume d in this.DiskVolumes.Values)
            {
                d.Copy(m, false);
            }

            LoadServerInstances(false);
            foreach (ServerInstance s in this.ServerInstances.Values)
            {
                s.Copy(m, false);
            }

            return m;   
        }
    }
}
