using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Jhu.Graywulf.Format
{
    public class FileDataReader : IDataReader
    {
        private DataFileBase parent;
        private Dictionary<string, int> columnIndex;

        #region Constructors and initializers

        internal FileDataReader(DataFileBase parent)
        {
            this.parent = parent;
            this.columnIndex = null;
        }

        public void Dispose()
        {
            this.parent = null;
            this.columnIndex = null;
        }

        #endregion

        public int RecordsAffected
        {
            get { return -1; }
        }

        public bool Read()
        {
            if (columnIndex == null)
            {
                BuildColumnIndex();
            }

            return parent.Read();
        }

        /*
        public bool Read(XmlReader reader)
        {
            if (columnIndex == null)
            {
                BuildColumnIndex();
            }

            return parent.Read(reader);
        }*/

        public bool NextResult()
        {
            return parent.NextResult();
        }

        public bool IsClosed
        {
            get { return parent.IsClosed; }
        }

        public void Close()
        {
            parent.Close();
        }

        #region Column functions

        private void BuildColumnIndex()
        {
            columnIndex = new Dictionary<string, int>();

            for (int i = 0; i < parent.Columns.Count; i++)
            {
                columnIndex.Add(parent.Columns[i].Name, i);
            }
        }

        public DataTable GetSchemaTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(SchemaTableColumn.ColumnName, typeof(string));
            dt.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int));
            dt.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int));
            dt.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(short));
            dt.Columns.Add(SchemaTableColumn.NumericScale, typeof(short));
            dt.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool));
            dt.Columns.Add(SchemaTableColumn.IsKey, typeof(bool));
            dt.Columns.Add(SchemaTableColumn.DataType, typeof(Type));
            dt.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool));
            dt.Columns.Add(SchemaTableColumn.ProviderType, typeof(string));
            dt.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool));
            dt.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool));
            //dt.Columns.Add(SchemaTableOptionalColumn.IsIdentity, typeof(bool));
            dt.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
            dt.Columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool));
            dt.Columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool));
            dt.Columns.Add(SchemaTableColumn.IsLong, typeof(bool));
            dt.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool));
            dt.Columns.Add(SchemaTableOptionalColumn.ProviderSpecificDataType, typeof(string));
            //dt.Columns.Add(SchemaTableColumn.DataTypeName, typeof(string));

            // Add column ID
            int q = 0;
            foreach (var col in parent.Columns)
            {
                AddSchemaTableColumn(dt, col, q++);
            }

            return dt;
        }

        private void AddSchemaTableColumn(DataTable dt, DataFileColumn col, int ordinal)
        {
            var dr = dt.NewRow();

            dr[SchemaTableColumn.ColumnName] = col.Name;
            dr[SchemaTableColumn.ColumnOrdinal] = ordinal;
            dr[SchemaTableColumn.ColumnSize] = col.DataType.Size;
            dr[SchemaTableColumn.NumericPrecision] = col.DataType.Precision;
            dr[SchemaTableColumn.NumericScale] = col.DataType.Scale;
            dr[SchemaTableColumn.IsUnique] = col.IsIdentity;
            dr[SchemaTableColumn.IsKey] = col.IsIdentity;
            dr[SchemaTableColumn.DataType] = col.DataType.Type;
            dr[SchemaTableColumn.AllowDBNull] = col.IsNullable;
            dr[SchemaTableColumn.ProviderType] = col.DataType.Name;
            dr[SchemaTableColumn.IsAliased] = false;
            dr[SchemaTableColumn.IsExpression] = false;
            //dr[SchemaTableOptionalColumn.IsIdentity] = col.IsIdentity;
            dr[SchemaTableOptionalColumn.IsAutoIncrement] = col.IsIdentity;
            dr[SchemaTableOptionalColumn.IsRowVersion] = false;
            dr[SchemaTableOptionalColumn.IsHidden] = false;
            dr[SchemaTableColumn.IsLong] = col.DataType.IsMax;
            dr[SchemaTableOptionalColumn.IsReadOnly] = true;
            dr[SchemaTableOptionalColumn.ProviderSpecificDataType] = col.DataType.Name;

            dt.Rows.Add(dr);
        }

        #endregion
        #region Simple column accessors

        public int FieldCount
        {
            get { return parent.Columns.Count; }
        }

        public object this[string name]
        {
            get { return parent.RowValues[columnIndex[name]]; }
        }

        public object this[int i]
        {
            get { return parent.RowValues[i]; }
        }

        public string GetDataTypeName(int i)
        {
            return parent.Columns[i].DataType.Name;
        }

        public Type GetFieldType(int i)
        {
            return parent.Columns[i].DataType.Type;
        }

        public string GetName(int i)
        {
            return parent.Columns[i].Name;
        }

        public int GetOrdinal(string name)
        {
            return columnIndex[name];
        }

        public object GetValue(int i)
        {
            return parent.RowValues[i];
        }

        public int GetValues(object[] values)
        {
            for (int i = 0; i < parent.RowValues.Length; i++)
            {
                values[i] = parent.RowValues[i];
            }

            return parent.RowValues.Length;
        }

        public bool IsDBNull(int i)
        {
            return parent.RowValues[i] == null;
        }

        #endregion
        #region Strongly type column accessors

        public bool GetBoolean(int i)
        {
            return (bool)parent.RowValues[i];
        }

        public byte GetByte(int i)
        {
            return (byte)parent.RowValues[i];
        }

        public short GetInt16(int i)
        {
            return (Int16)parent.RowValues[i];
        }

        public int GetInt32(int i)
        {
            return (Int32)parent.RowValues[i];
        }

        public long GetInt64(int i)
        {
            return (Int64)parent.RowValues[i];
        }

        public float GetFloat(int i)
        {
            return (float)parent.RowValues[i];
        }

        public double GetDouble(int i)
        {
            return (double)parent.RowValues[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)parent.RowValues[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)parent.RowValues[i];
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)parent.RowValues[i];
        }

        public char GetChar(int i)
        {
            return (char)parent.RowValues[i];
        }

        public string GetString(int i)
        {
            return (string)parent.RowValues[i];
        }

        #endregion
        #region Blob column accessors

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            // TODO: implement
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            // TODO: implement
            throw new NotImplementedException();
        }

        #endregion
        #region Hierarchical data functions

        public int Depth
        {
            get
            {
                // No hierarchical data sets are supported
                return 0;
            }
        }

        public IDataReader GetData(int i)
        {
            // This is supposed to be used with hierarchical data
            throw new NotImplementedException();
        }

        #endregion
    }
}
