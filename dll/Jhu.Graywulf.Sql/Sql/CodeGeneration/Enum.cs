using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.CodeGeneration
{
    /// <summary>
    /// Column list nullable type.
    /// </summary>
    public enum ColumnListNullType
    {
        /// <summary>
        /// No 'NULL' or 'NOT NULL' added after columns.
        /// </summary>
        /// <remarks>
        /// For use with select lists, view definitions.
        /// </remarks>
        Nothing,

        Defined,

        /// <summary>
        /// 'NULL' added after each column.
        /// </summary>
        /// <remarks>
        /// Not used.
        /// </remarks>
        Null,

        /// <summary>
        /// 'NOT NULL' added after each column
        /// </summary>
        /// <remarks>
        /// From use with create table.
        /// </remarks>
        NotNull
    }

    /// <summary>
    /// Column list type.
    /// </summary>
    public enum ColumnListType
    {
        /// <summary>
        /// To use with 'CREATE TABLE' column list.
        /// </summary>
        /// <remarks>
        /// Original name used, column type is added.
        /// </remarks>
        CreateTableWithOriginalName,

        /// <summary>
        /// To use with 'CREATE TABLE' column list.
        /// </summary>
        /// <remarks>
        /// Original name used, column type is added.
        /// </remarks>
        CreateTableWithEscapedName,

        /// <summary>
        /// To use with 'CREATE VIEW' column list.
        /// </summary>
        /// <remarks>
        /// Escaped name used, column type is not added.
        /// </remarks>
        CreateView,

        /// <summary>
        /// To use with index creation queries
        /// </summary>
        CreateIndex,

        /// <summary>
        /// To use with 'INSERT' column list.
        /// </summary>
        /// <remarks>
        /// Escaped name used, column type is not added.
        /// </remarks>
        Insert,

        /// <summary>
        /// To use with 'SELECT'.
        /// </summary>
        /// <remarks>
        /// Original name used. To use with zone table create
        /// from source tables.
        /// </remarks>
        SelectWithOriginalName,

        /// <summary>
        /// To use with 'SELECT'.
        /// </summary>
        /// <remarks>
        /// Escaped name used. To use with anything but zone table
        /// create from source tables.
        /// </remarks>
        SelectWithEscapedName,

        /// <summary>
        /// To use with 'SELECT'
        /// </summary>
        /// <remarks>
        /// Original name used.
        /// </remarks>
        SelectWithOriginalNameNoAlias,

        /// <summary>
        /// To use with 'SELECT'.
        /// </summary>
        /// <remarks>
        /// Escaped name used, no column alias added. To use
        /// with 'INSERT' or 'CREATE INDEX'
        /// </remarks>
        SelectWithEscapedNameNoAlias,

        /// <summary>
        /// To use in join condition with the same column names
        /// </summary>
        JoinConditionWithOriginalName,

        /// <summary>
        /// Join condition with escaped column name
        /// </summary>
        JoinConditionWithEscapedName
    }
}
