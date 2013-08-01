using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class User : Entity
    {
        #region Database IO Functions

        public void LoadMyDBDatabaseInstances(bool forceReload)
        {
            LoadChildren<MyDBDatabaseInstance>(forceReload);
        }

        #endregion
    }
}
