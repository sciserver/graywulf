﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [Flags]
    public enum ColumnContext : long
    {
        None = 0x00000000,
        NotReferenced = 0x00000001,

        SelectList = 0x00000002,
        From = 0x00000004,
        JoinOn = 0x00000008,
        Where = 0x00000010,
        GroupBy = 0x00000020,
        Having = 0x00000040,
        OrderBy = 0x00000080,

        Default = SelectList | From | Where | GroupBy | Having | OrderBy,

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
        From = 0x00000200,
        Into = 0x00000400,                   // SELECT INTO
        Where = 0x00000800,
        GroupBy = 0x00001000,
        Having = 0x00002000,
        OrderBy = 0x00004000,
        SelectList = 0x00008000,

        Insert = 0x00010000,
        Update = 0x00020000,
        Delete = 0x00040000,

        Create = 0x00100000,
        Alter = 0x00200000,
        Drop = 0x00400000,
        Truncate = 0x00800000,

        Output = Into | Create,
        Target = Insert | Update | Delete | Alter | Drop | Truncate
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
