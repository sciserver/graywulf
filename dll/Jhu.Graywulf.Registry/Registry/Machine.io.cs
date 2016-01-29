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

        public void LoadDiskGroups(bool forceReload)
        {
            LoadChildren<DiskGroup>(forceReload);
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

            LoadDiskGroups(false);
            foreach (DiskGroup d in this.DiskGroups.Values)
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
