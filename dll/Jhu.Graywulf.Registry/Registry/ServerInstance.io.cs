using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class ServerInstance : Entity
    {
        #region Database IO Functions

        public void LoadDiskGroups(bool forceReload)
        {
            LoadChildren<ServerInstanceDiskGroup>(forceReload);
        }

        #endregion

        public override Entity Copy(Entity parent, bool prefixName)
        {
            var si = (ServerInstance)CreateCopy(parent, prefixName);
            si.Save();

            LoadDiskGroups(false);
            foreach (var dg in this.DiskGroups.Values)
            {
                dg.Copy(si, false);
            }

            return si;
        }
    }
}
