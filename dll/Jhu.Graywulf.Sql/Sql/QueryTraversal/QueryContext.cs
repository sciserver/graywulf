using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    [Flags]
    public enum QueryContext : ulong
    {
        None = 0x00000000,                       // Node is not part of a query
        SelectStatement = 0x00000001,            // Main select
        DeleteStatement = 0x00000002,
        UpdateStatement = 0x00000004,
        InsertStatement = 0x00000008,
        CommonTableExpression = 0x00000010,      // CTE definition part
        Subquery = 0x00000020,                   // Subquery in FROM
        SemiJoin = 0x00000040,                   // Subquery in EXISTS, ALL, SOME and ANY, etc.

        Expression = 0x01000000,
        LogicalExpression = 0x02000000,

        AnyExpression = 0xFF000000
    }
}
