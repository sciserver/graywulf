using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Data
{
    public class SmartDataReader : ISmartDataReader
    {
        #region Private member variables

        private DatasetBase dataset;
        private IDataReader dataReader;

        #endregion
        #region IDataReader properties

        public int Depth
        {
            get { return dataReader.Depth; }
        }

        public bool IsClosed
        {
            get { return dataReader.IsClosed; }
        }

        public int RecordsAffected
        {
            get { return dataReader.RecordsAffected; }
        }

        public object this[string name]
        {
            get { return dataReader[name]; }
        }

        public object this[int i]
        {
            get { return dataReader[i]; }
        }

        public int FieldCount
        {
            get { return dataReader.FieldCount; }
        }

        #endregion
        #region Properties

        public DatasetBase Dataset
        {
            get { return dataset; }
        }

        #endregion
        #region Constructors and initializers

        public SmartDataReader(DatasetBase dataset, IDataReader dataReader)
        {
            this.dataset = dataset;
            this.dataReader = dataReader;
        }

        public void Dispose()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
                dataReader = null;
            }
        }

        #endregion
        #region IDataReader functions

        public bool NextResult()
        {
            return dataReader.NextResult();
        }

        public bool Read()
        {
            return dataReader.Read();
        }

        public void Close()
        {
            dataReader.Close();
        }

        #endregion
        #region IDataReader schema functions

        public DataTable GetSchemaTable()
        {
            return dataReader.GetSchemaTable();
        }

        #endregion
        #region Field functions

        public string GetDataTypeName(int i)
        {
            return dataReader.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return dataReader.GetFieldType(i);
        }

        public string GetName(int i)
        {
            return dataReader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return dataReader.GetOrdinal(name);
        }

        public bool IsDBNull(int i)
        {
            return dataReader.IsDBNull(i);
        }

        #endregion
        #region Field accessors

        public IDataReader GetData(int i)
        {
            return dataReader.GetData(i);
        }

        public object GetValue(int i)
        {
            return dataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return dataReader.GetValues(values);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        #endregion
        #region Strongly type field accessors

        public bool GetBoolean(int i)
        {
            return dataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return dataReader.GetByte(i);
        }

        public short GetInt16(int i)
        {
            return dataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return dataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return dataReader.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return dataReader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return dataReader.GetDouble(i);
        }

        public decimal GetDecimal(int i)
        {
            return dataReader.GetDecimal(i);
        }

        public Guid GetGuid(int i)
        {
            return dataReader.GetGuid(i);
        }

        public DateTime GetDateTime(int i)
        {
            return dataReader.GetDateTime(i);
        }

        public char GetChar(int i)
        {
            return dataReader.GetChar(i);
        }

        public string GetString(int i)
        {
            return dataReader.GetString(i);
        }

        #endregion




        public IList<Column> GetColumns()
        {
            return dataset.CreateColumns(dataReader);
        }

        public long GetRowCount()
        {
            return -1;
        }
    }
}
