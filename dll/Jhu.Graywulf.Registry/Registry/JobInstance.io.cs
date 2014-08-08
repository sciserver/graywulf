using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.IO;

namespace Jhu.Graywulf.Registry
{
    public partial class JobInstance : Entity
    {
        #region Database IO Functions

        #endregion
        #region Navigation Functions

        public void LoadJobInstancesDependencies(bool forceReload)
        {
            LoadChildren<JobInstanceDependency>(forceReload);
        }

        #endregion
    }
}
