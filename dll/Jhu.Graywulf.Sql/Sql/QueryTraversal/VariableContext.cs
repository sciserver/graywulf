using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    [Flags]
    public enum VariableContext : ulong
    {
        None,
        System = 0x0001,

        Scalar = 0x0010,
        Table = 0x0020,
        Cursor = 0x0030,
    }
}
