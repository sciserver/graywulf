using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

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

            dt.Columns.Add("ColumnName", typeof(string));
            dt.Columns.Add("ColumnOrdinal", typeof(int));
            dt.Columns.Add("ColumnSize", typeof(int));
            dt.Columns.Add("NumericPrecision", typeof(int));
            dt.Columns.Add("NumericScale", typeof(int));
            dt.Columns.Add("IsUnique", typeof(bool));
            dt.Columns.Add("IsKey", typeof(bool));
            dt.Columns.Add("DataType", typeof(Type));
            dt.Columns.Add("AllowDBNull", typeof(bool));
            dt.Columns.Add("ProviderType", typeof(string));
            dt.Columns.Add("IsAliased", typeof(bool));
            dt.Columns.Add("IsExpression", typeof(bool));
            dt.Columns.Add("IsIdentity", typeof(bool));
            dt.Columns.Add("IsAutoIncrement", typeof(bool));
            dt.Columns.Add("IsRowVersion", typeof(bool));
            dt.Columns.Add("IsHidden", typeof(bool));
            dt.Columns.Add("IsLong", typeof(bool));
            dt.Columns.Add("IsReadOnly", typeof(bool));
            dt.Columns.Add("ProviderSpecificDataType", typeof(string));
            dt.Columns.Add("DataTypeName", typeof(string));

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

            int size;
            byte precision, scale;
            bool islong;

            col.GetTypeInfo(out size, out precision, out scale, out islong);

            dr["ColumnName"] = col.Name;
            dr["ColumnOrdinal"] = ordinal;
            dr["ColumnSize"] = size;
            dr["NumericPrecision"] = precision;
            dr["NumericScale"] = scale;
            dr["IsUnique"] = col.IsIdentity;
            dr["IsKey"] = col.IsIdentity;
            dr["DataType"] = col.Type;
            dr["AllowDBNull"] = col.AllowNull;
            dr["ProviderType"] = col.Type.Name;
            dr["IsAliased"] = false;
            dr["IsExpression"] = false;
            dr["IsIdentity"] = col.IsIdentity;
            dr["IsAutoIncrement"] = col.IsIdentity;
            dr["IsRowVersion"] = false;
            dr["IsHidden"] = false;
            dr["IsLong"] = islong;
            dr["IsReadOnly"] = true;
            dr["ProviderSpecificDataType"] = col.Type.Name;

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
            return parent.Columns[i].Type.Name;
        }

        public Type GetFieldType(int i)
        {
            return parent.Columns[i].Type;
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
