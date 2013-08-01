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

        #endregion

        public override Entity Copy(Entity parent, bool prefixName)
        {
            var si = (ServerInstance)CreateCopy(parent, prefixName);
            si.Save();

            return si;
        }
    }
}
