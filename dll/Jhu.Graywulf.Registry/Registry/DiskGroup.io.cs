using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class DiskGroup : Entity
    {
        #region Database IO Functions

        public void LoadDiskVolumes(bool forceReload)
        {
            LoadChildren<DiskVolume>(forceReload);
        }

        #endregion

        public override Entity Copy(Entity parent, bool prefixName)
        {
            var g = CreateCopy(parent, prefixName);
            g.Save();

            LoadDiskVolumes(false);
            foreach (DiskVolume s in this.DiskVolumes.Values)
            {
                s.Copy(g, false);
            }

            return g;   
        }
    }
}
