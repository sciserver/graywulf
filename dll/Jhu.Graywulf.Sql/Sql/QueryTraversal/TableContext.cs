using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    [Flags]
    public enum TableContext : ulong
    {
        None = 0x00000000,

        TableOrView = 0x00000001,
        Subquery = 0x00000002,
        Variable = 0x00000004,
        UserDefinedFunction = 0x00000008,

        CommonTable = 0x00000100,
        SelectList = 0x00000200,
        From = 0x00000400,
        Into = 0x00000800,                   // SELECT INTO
        Where = 0x00001000,
        GroupBy = 0x00002000,
        Having = 0x00004000,
        OrderBy = 0x00008000,
        PartitionBy = 0x00010000,

        Insert = 0x00100000,
        Update = 0x00200000,
        Delete = 0x00400000,

        Create = 0x01000000,
        Alter = 0x02000000,
        Drop = 0x04000000,
        Truncate = 0x08000000,

        Output = Into | Create,
        Target = Insert | Update | Delete | Alter | Drop | Truncate
    }
}
