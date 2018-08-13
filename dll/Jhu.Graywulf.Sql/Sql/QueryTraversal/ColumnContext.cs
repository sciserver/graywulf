using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    [Flags]
    public enum ColumnContext : ulong
    {
        None = 0x00000000,
        Expression = 0x00000001,
        Predicate = 0x00000002,

        SelectList = 0x00000004,
        From = 0x00000008,
        JoinOn = 0x00000010,
        Where = 0x00000020,
        GroupBy = 0x00000040,
        Having = 0x00000080,
        OrderBy = 0x00000100,
        PartitionBy = 0x00000200,

        Default = SelectList | From | Where | GroupBy | Having | OrderBy | PartitionBy,

        Insert = 0x00010000,

        Update = 0x00020000,

        Hint = 0x01000000,
        Special = 0x02000000,
        Key = 0x04000000,             // Columns marked as 'Key' in schema description (usually taken from a data reader's schema table)
        PrimaryKey = 0x08000000,      // Columns of the primary key index
        Identity = 0x10000000,        // Column is an identity column

        AllReferenced = Default | Insert | Update | Hint | Special | PrimaryKey,

        All = 0x7FFFFFFF
    }
}
