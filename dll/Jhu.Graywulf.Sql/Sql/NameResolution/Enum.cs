using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [Flags]
    public enum ColumnContext : long
    {
        None = 0,
        NonReferenced = 1,

        SelectList = 2,
        From = 4,
        Where = 8,
        GroupBy = 16,
        Having = 32,
        OrderBy = 64,

        Default = SelectList | From | Where | GroupBy | Having | OrderBy,

        Hint = 128,
        Special = 1024,
        Key = 4096,             // Columns marked as 'Key' in schema description (usually taken from a data reader's schema table)
        PrimaryKey = 2048,      // Columns of the primary key index
        Identity = 8192,        // Column is an identity column

        AllReferenced = Default | Hint | Special | PrimaryKey,

        All = NonReferenced | AllReferenced
    }

    [Flags]
    public enum TableContext : long
    {
        None = 0,
        From = 1,
        Subquery = 2,
        Into = 4,
        CreateTable = 8,
    }

    [Flags]
    public enum QueryContext : long
    {
        None,                       // Node is not part of a query
        SelectStatement,            // Main select
        CommonTableExpression,      // CTE definition part
        Subquery,                   // Subquery in FROM
        SemiJoin                    // Subquery in EXISTS, ALL, SOME and ANY, etc.
    }
}
