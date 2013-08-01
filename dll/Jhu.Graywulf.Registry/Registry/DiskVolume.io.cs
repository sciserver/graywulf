using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class DiskVolume : Entity
    {
        #region Database IO Functions

        #endregion

        public override Entity Copy(Entity parent, bool prefixName)
        {
            var d = (DiskVolume)CreateCopy(parent, prefixName);
            d.Save();

            return d;
        }
    }
}
