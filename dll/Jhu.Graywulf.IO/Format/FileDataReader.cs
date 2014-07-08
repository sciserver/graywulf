using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.Format
{
    public class FileDataReader : ISmartDataReader
    {
        #region Private member variables

        private DataFileBase file;

        private Dictionary<string, int> columnIndex;
        private object[] rowValues;
        private int[] isIdentity;

        private int blockCounter;
        private long rowCounter;

        #endregion
        #region IDataReader properties

        public int Depth
        {
            get
            {
                // No hierarchical data sets are supported
                return 0;
            }
        }

        public bool IsClosed
        {
            get { return file.IsClosed; }
        }

        public int RecordsAffected
        {
            get { return -1; }
        }

        public object this[string name]
        {
            get { return rowValues[columnIndex[name]]; }
        }

        public object this[int i]
        {
            get { return rowValues[i]; }
        }

        public int FieldCount
        {
            get { return file.CurrentBlock.Columns.Count; }
        }

        #endregion
        #region Constructors and initializers

        internal FileDataReader(DataFileBase file)
        {
            InizializeMembers();

            this.file = file;

            NextResult();
        }

        private void InizializeMembers()
        {
            this.file = null;

            this.columnIndex = null;
            this.rowValues = null;
            this.isIdentity = null;

            this.blockCounter = -1;
            this.rowCounter = -1;
        }

        public void Dispose()
        {
            this.file = null;
            this.columnIndex = null;
        }

        #endregion
        #region IDataReader functions

        public bool NextResult()
        {
            if (file.ReadNextBlock() != null)
            {
                blockCounter++;
                return true;
            }
            else
            {
                blockCounter = -1;
                return false;
            }
        }

        /// <summary>
        /// Reads the next row from the data file's current block.
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (rowCounter == -1)
            {
                InitializeColumns();
            }

            if (file.CurrentBlock.OnReadNextRow(rowValues))
            {
                rowCounter++;

                // Generate identity values
                for (int i = 0; i < isIdentity.Length; i++)
                {
                    rowValues[i] = rowCounter + 1;  // identity values start from 1, usually
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Close()
        {
            file.Close();
        }

        #endregion
        #region IDataReader schema functions

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
            dt.Columns.Add("DataTypeName", typeof(string));
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

            // Add column ID
            int q = 0;
            foreach (var col in file.CurrentBlock.Columns)
            {
                AddSchemaTableColumn(dt, col, q++);
            }

            return dt;
        }

        private void AddSchemaTableColumn(DataTable dt, Column col, int ordinal)
        {
            var dr = dt.NewRow();

            dr[SchemaTableColumn.ColumnName] = col.Name;
            dr[SchemaTableColumn.ColumnOrdinal] = ordinal;
            dr[SchemaTableColumn.ColumnSize] = col.DataType.Length;
            dr[SchemaTableColumn.NumericPrecision] = col.DataType.Precision;
            dr[SchemaTableColumn.NumericScale] = col.DataType.Scale;
            dr[SchemaTableColumn.IsUnique] = col.IsIdentity;
            dr[SchemaTableColumn.IsKey] = col.IsIdentity;
            dr[SchemaTableColumn.DataType] = col.DataType.Type;
            dr["DataTypeName"] = col.DataType.Name;
            dr[SchemaTableColumn.AllowDBNull] = col.DataType.IsNullable;
            dr[SchemaTableColumn.ProviderType] = col.DataType.Name;
            dr[SchemaTableColumn.IsAliased] = false;
            dr[SchemaTableColumn.IsExpression] = false;
            //dr[SchemaTableOptionalColumn.IsIdentity] = col.IsIdentity;
            dr[SchemaTableOptionalColumn.IsAutoIncrement] = col.IsIdentity;
            dr[SchemaTableOptionalColumn.IsRowVersion] = false;
            dr[SchemaTableOptionalColumn.IsHidden] = false;
            dr[SchemaTableColumn.IsLong] = col.DataType.IsMaxLength;
            dr[SchemaTableOptionalColumn.IsReadOnly] = true;
            dr[SchemaTableOptionalColumn.ProviderSpecificDataType] = col.DataType.Name;

            dt.Rows.Add(dr);
        }

        #endregion
        #region Field functions

        public string GetDataTypeName(int i)
        {
            return file.CurrentBlock.Columns[i].DataType.Name;
        }

        public Type GetFieldType(int i)
        {
            return file.CurrentBlock.Columns[i].DataType.Type;
        }

        public string GetName(int i)
        {
            return file.CurrentBlock.Columns[i].Name;
        }

        public int GetOrdinal(string name)
        {
            return columnIndex[name];
        }

        public bool IsDBNull(int i)
        {
            return rowValues[i] == null;
        }

        #endregion
        #region Field accessors

        public IDataReader GetData(int i)
        {
            // This is supposed to be used with hierarchical data
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            return rowValues[i];
        }

        public int GetValues(object[] values)
        {
            for (int i = 0; i < rowValues.Length; i++)
            {
                values[i] = rowValues[i];
            }

            return rowValues.Length;
        }

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
        #region Strongly type field accessors

        public bool GetBoolean(int i)
        {
            return (bool)rowValues[i];
        }

        public byte GetByte(int i)
        {
            return (byte)rowValues[i];
        }

        public short GetInt16(int i)
        {
            return (Int16)rowValues[i];
        }

        public int GetInt32(int i)
        {
            return (Int32)rowValues[i];
        }

        public long GetInt64(int i)
        {
            return (Int64)rowValues[i];
        }

        public float GetFloat(int i)
        {
            return (float)rowValues[i];
        }

        public double GetDouble(int i)
        {
            return (double)rowValues[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)rowValues[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)rowValues[i];
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)rowValues[i];
        }

        public char GetChar(int i)
        {
            return (char)rowValues[i];
        }

        public string GetString(int i)
        {
            return (string)rowValues[i];
        }

        #endregion
        
       
        private void InitializeColumns()
        {
            var colc = file.CurrentBlock.Columns.Count;
            var ids = new List<int>();

            columnIndex = new Dictionary<string, int>();
            rowValues = new object[colc];

            for (int i = 0; i < colc; i++)
            {
                columnIndex.Add(file.CurrentBlock.Columns[i].Name, i);

                if (file.CurrentBlock.Columns[i].IsIdentity)
                {
                    ids.Add(i);
                }
            }

            isIdentity = ids.ToArray();
        }

        public IList<Column> GetColumns()
        {
            return file.CurrentBlock.Columns;
        }

        public long GetRowCount()
        {
            return file.CurrentBlock.RowCount;
        }

    }
}
