using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class QueueInstance : Entity
    {
        #region Database IO Functions

        #endregion
        #region Navigation Functions

        public void LoadJobInstances(bool forceReload)
        {
            LoadChildren<JobInstance>(forceReload);
        }

        #endregion
    }
}
