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

    [Flags]
    public enum TableContext : long
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

    [Flags]
    public enum QueryContext : long
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
    }

    [Flags]
    public enum VariableContext : long
    {
        None,
        System = 0x0001,

        Scalar = 0x0010,
        Table = 0x0020,
        Cursor = 0x0030,
    }
}
