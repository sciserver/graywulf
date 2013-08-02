using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    [Flags]
    public enum ColumnContext : long
    {
        None = 0,
        SelectList = 1,
        From = 2,
        Where = 4,
        GroupBy = 8,
        Having = 16,
        OrderBy = 32,

        Special = 1024,
    }
}
