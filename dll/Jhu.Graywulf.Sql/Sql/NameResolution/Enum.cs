using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [Flags]
    public enum ColumnContext : long
    {
        None = 0x00000000,
        NonReferenced = 0x00000001,

        SelectList = 0x00000002,
        From = 0x00000004,
        Where = 0x00000008,
        GroupBy = 0x00000010,
        Having = 0x00000020,
        OrderBy = 0x00000040,
        
        Default = SelectList | From | Where | GroupBy | Having | OrderBy,

        Insert = 0x00010000,

        Update = 0x00020000,

        Hint = 0x00100001,
        Special = 0x00100002,
        Key = 0x00100004,             // Columns marked as 'Key' in schema description (usually taken from a data reader's schema table)
        PrimaryKey = 0x00100008,      // Columns of the primary key index
        Identity = 0x00100010,        // Column is an identity column

        AllReferenced = Default | Insert | Update | Hint | Special | PrimaryKey,

        All = 0x7FFFFFFF
    }

    [Flags]
    public enum TableContext : long
    {
        None = 0,
        From = 1,
        Subquery = 2,
        Into = 4,
        Target = 8,
        CreateTable = 16,
    }

    [Flags]
    public enum QueryContext : long
    {
        None = 0,                       // Node is not part of a query
        SelectStatement = 1,            // Main select
        DeleteStatement = 2,
        UpdateStatement = 4,
        InsertStatement = 8,
        CommonTableExpression = 16,      // CTE definition part
        Subquery = 32,                   // Subquery in FROM
        SemiJoin = 64                    // Subquery in EXISTS, ALL, SOME and ANY, etc.
    }
}
