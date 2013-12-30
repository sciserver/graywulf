using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    public enum Endianness
    {
        LittleEndian,
        BigEndian,
    }

    /// <summary>
    /// Represents different types of database objects
    /// </summary>
    [Flags]
    public enum DatabaseObjectType
    {
        Unknown = 0,
        Table = 1,
        View = 2,
        Function = 4,
        SqlTableValuedFunction = 8,
        SqlInlineTableValuedFunction = 16,
        ClrTableValuedFunction = 32,
        TableValuedFunction = SqlTableValuedFunction | SqlInlineTableValuedFunction | ClrTableValuedFunction,
        SqlScalarFunction = 64,
        ClrScalarFunction = 128,
        ScalarFunction = SqlScalarFunction | ClrScalarFunction,
        SqlStoredProcedure = 256,
        ClrStoredProcedure = 512,
        StoredProcedure = SqlStoredProcedure | ClrStoredProcedure,
        Index = 1024,
    }

    /// <summary>
    /// Represents different types of function parameters
    /// </summary>
    [Flags]
    public enum ParameterDirection
    {
        Unknown = 0,
        Input = 1,
        Output = 2,
        InputOutput = Input | Output,
        ReturnValue = 4,
    }

    /// <summary>
    /// Represents different types of index column ordering
    /// </summary>
    public enum IndexColumnOrdering
    {
        Unknown,
        Ascending,
        Descending
    }

    [Flags]
    public enum TableInitializationOptions
    {
        Drop = 1,
        Create = 2,
        Clear = 4,
        Append = 8
    }
}
