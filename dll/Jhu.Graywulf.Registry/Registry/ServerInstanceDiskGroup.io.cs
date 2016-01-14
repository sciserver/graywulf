using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class ServerInstanceDiskGroup : Entity
    {
        #region Database IO Functions

        #endregion

        public override Entity Copy(Entity parent, bool prefixName)
        {
            var dg = (ServerInstanceDiskGroup)CreateCopy(parent, prefixName);

            // Match old disk groups with new ones
            var nm = ((ServerInstance)parent).Machine;
            nm.LoadDiskGroups(false);
            dg.DiskGroup = nm.DiskGroups[this.DiskGroup.Name];

            dg.Save();

            return dg;
        }
    }
}
