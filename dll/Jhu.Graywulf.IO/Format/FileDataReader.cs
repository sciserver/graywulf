using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Format
{
    public class FileDataReader : DbDataReader, ISmartDataReader
    {
        #region Private member variables

        private DataFileBase file;

        private Dictionary<string, int> columnIndex;
        private object[] rowValues;
        private int[] isIdentity;

        private int blockCounter;
        private long rowCounter;

        private string queryName;
        private string resultsetName;

        #endregion
        #region IDataReader properties

        public override int Depth
        {
            get
            {
                // No hierarchical data sets are supported
                return 0;
            }
        }

        public override bool IsClosed
        {
            get { return file.IsClosed; }
        }

        public override int RecordsAffected
        {
            get { return -1; }
        }

        public override object this[string name]
        {
            get { return rowValues[columnIndex[name]]; }
        }

        public override object this[int i]
        {
            get { return rowValues[i]; }
        }

        public override int FieldCount
        {
            get { return file.CurrentBlock.Columns.Count; }
        }

        #endregion
        #region Properties

        public string QueryName
        {
            get { return queryName ?? file.Name; }
            set { queryName = value; }
        }

        public string ResultsetName
        {
            get { return resultsetName ?? file.CurrentBlock.Name; }
            set { resultsetName = value; }
        }

        public long RecordCount
        {
            get { return file.CurrentBlock.RecordCount; }
        }

        public DatabaseObjectMetadata Metadata
        {
            get { return file.CurrentBlock.Metadata; }
        }

        public List<Column> Columns
        {
            get { return file.CurrentBlock.Columns; }
        }

        public override bool HasRows
        {
            get
            {
                // Individual file readers might now from the
                // header whether there are rows in the file or not
                // but returning true here is safe if there are at
                // least columns.
                return file.CurrentBlock.Columns.Count > 0;
            }
        }

        public List<TypeMapping> TypeMappings
        {
            get
            {
                return file.CurrentBlock.ColumnTypeMappings;
            }
        }

        #endregion
        #region Constructors and initializers

        internal protected FileDataReader(DataFileBase file)
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

            this.queryName = null;
            this.resultsetName = null;
        }

        public new void Dispose()
        {
            this.file = null;
            this.columnIndex = null;

            base.Dispose();
        }

        #endregion
        #region IDataReader functions

        public override bool NextResult()
        {
            return Util.TaskHelper.Wait(NextResultAsync(CancellationToken.None));
        }

        public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            if (await file.ReadNextBlockAsync() != null)
            {
                blockCounter++;
                resultsetName = null;
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
        public override bool Read()
        {
            return Util.TaskHelper.Wait(ReadAsync(CancellationToken.None));
        }

        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            if (rowCounter == -1 && columnIndex == null)
            {
                InitializeColumns();
            }

            var hasNextRow = await file.CurrentBlock.OnReadNextRowAsync(rowValues);

            if (hasNextRow)
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

        public override void Close()
        {
            if (file != null)
            {
                file.Close();
            }
        }

        #endregion
        #region IDataReader schema functions

        public override DataTable GetSchemaTable()
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
            dr["DataTypeName"] = col.DataType.TypeName;
            dr[SchemaTableColumn.AllowDBNull] = col.DataType.IsNullable;
            dr[SchemaTableColumn.ProviderType] = col.DataType.TypeName;
            dr[SchemaTableColumn.IsAliased] = false;
            dr[SchemaTableColumn.IsExpression] = false;
            //dr[SchemaTableOptionalColumn.IsIdentity] = col.IsIdentity;
            dr[SchemaTableOptionalColumn.IsAutoIncrement] = col.IsIdentity;
            dr[SchemaTableOptionalColumn.IsRowVersion] = false;
            dr[SchemaTableOptionalColumn.IsHidden] = false;
            dr[SchemaTableColumn.IsLong] = col.DataType.IsMaxLength;
            dr[SchemaTableOptionalColumn.IsReadOnly] = true;
            dr[SchemaTableOptionalColumn.ProviderSpecificDataType] = col.DataType.TypeName;

            dt.Rows.Add(dr);
        }

        #endregion
        #region Field functions

        public override string GetDataTypeName(int i)
        {
            return file.CurrentBlock.Columns[i].DataType.TypeName;
        }

        public override Type GetFieldType(int i)
        {
            return file.CurrentBlock.Columns[i].DataType.Type;
        }

        public override string GetName(int i)
        {
            return file.CurrentBlock.Columns[i].Name;
        }

        public override int GetOrdinal(string name)
        {
            return columnIndex[name];
        }

        public override bool IsDBNull(int i)
        {
            return rowValues[i] == null || rowValues[i] == DBNull.Value;
        }

        #endregion
        #region Field accessors

        public override object GetValue(int i)
        {
            var value = rowValues[i];

            if (value != null && value != DBNull.Value && TypeMappings[i] != null)
            {
                return TypeMappings[i].Mapping(rowValues[i]);
            }
            else
            {
                return value;
            }
        }

        public override int GetValues(object[] values)
        {
            for (int i = 0; i < rowValues.Length; i++)
            {
                var value = rowValues[i];

                if (value != null && value != DBNull.Value && TypeMappings[i] != null)
                {
                    values[i] = TypeMappings[i].Mapping(value);
                }
                else
                {
                    values[i] = value;
                }
            }

            return rowValues.Length;
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            var data = (byte[])rowValues[i];
            long res;

            if (fieldOffset + length <= data.Length)
            {
                res = length;
            }
            else
            {
                res = data.LongLength - fieldOffset;
            }

            Array.Copy(data, fieldOffset, buffer, bufferoffset, res);

            return res;
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            // TODO: implement
            throw new NotImplementedException();
        }

        #endregion
        #region Strongly type field accessors

        public override bool GetBoolean(int i)
        {
            return (bool)rowValues[i];
        }

        public override byte GetByte(int i)
        {
            return (byte)rowValues[i];
        }

        public override short GetInt16(int i)
        {
            return (Int16)rowValues[i];
        }

        public override int GetInt32(int i)
        {
            return (Int32)rowValues[i];
        }

        public override long GetInt64(int i)
        {
            return (Int64)rowValues[i];
        }

        public override float GetFloat(int i)
        {
            return (float)rowValues[i];
        }

        public override double GetDouble(int i)
        {
            return (double)rowValues[i];
        }

        public override decimal GetDecimal(int i)
        {
            return (decimal)rowValues[i];
        }

        public override Guid GetGuid(int i)
        {
            return (Guid)rowValues[i];
        }

        public override DateTime GetDateTime(int i)
        {
            return (DateTime)rowValues[i];
        }

        public override char GetChar(int i)
        {
            return (char)rowValues[i];
        }

        public override string GetString(int i)
        {
            return (string)rowValues[i];
        }

        #endregion

        internal void CreateColumns(IList<Column> columns)
        {
            file.CurrentBlock.CreateColumns(columns);
        }

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

        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
