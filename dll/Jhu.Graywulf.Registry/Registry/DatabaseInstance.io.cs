using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseInstance : Entity
    {
        #region Database IO Functions

        public void LoadFileGroups(bool forceReload)
        {
            LoadChildren<DatabaseInstanceFileGroup>(forceReload);
        }

        #endregion
    }
}
