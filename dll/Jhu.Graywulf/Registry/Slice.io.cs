using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class Slice : Entity
    {
        #region Database IO Functions

        public void LoadPartitions(bool forceReload)
        {
            LoadChildren<Partition>(forceReload);
        }

        #endregion
    }
}
