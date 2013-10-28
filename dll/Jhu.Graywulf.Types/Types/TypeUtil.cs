using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Jhu.Graywulf.Types
{
    public static class TypeUtil
    {
        public static void CopyColumnFromSchemaTableRow(IColumn column, DataRow dr)
        {
            column.ID = (int)dr[SchemaTableColumn.ColumnOrdinal];
            column.Name = (string)dr[SchemaTableColumn.ColumnName];
            column.IsIdentity = dr[SchemaTableColumn.IsUnique] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsUnique];  //
            column.IsKey = dr[SchemaTableColumn.IsKey] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsKey];  //
            column.IsNullable = dr[SchemaTableColumn.AllowDBNull] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.AllowDBNull];
            column.IsHidden = dr[SchemaTableOptionalColumn.IsHidden] == DBNull.Value ? false : (bool)dr[SchemaTableOptionalColumn.IsHidden];

            column.DataType = DataType.Create(dr);
        }
    }
}
