using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf.Schema.Clr
{
    public static partial class ClrObjects
    {
        [SqlProcedure]
        public static SqlInt32 ClrStoredProc(SqlInt32 parameter)
        {
            return 0;
        }
    }
}
