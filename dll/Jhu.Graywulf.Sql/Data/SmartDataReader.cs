using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Schema;
using System.Collections;

namespace Jhu.Graywulf.Data
{
    public class SmartDataReader : DbDataReader, ISmartDataReader
    {
        #region Private member variables

        private DatasetBase dataset;
        private DbDataReader dataReader;
        private int resultsetCounter;
        private List<long> recordCounts;
        private string name;
        private DatabaseObjectMetadata metadata;
        private List<Column> columns;
        private List<TypeMapping> typeMappings;

        #endregion
        #region IDataReader properties

        public override int Depth
        {
            get { return dataReader.Depth; }
        }

        public override bool IsClosed
        {
            get { return dataReader.IsClosed; }
        }

        public override bool HasRows
        {
            get { return dataReader.HasRows; }
        }

        public override int RecordsAffected
        {
            get { return dataReader.RecordsAffected; }
        }

        public override object this[string name]
        {
            get { return dataReader[name]; }
        }

        public override object this[int i]
        {
            get { return dataReader[i]; }
        }

        public override int FieldCount
        {
            get { return dataReader.FieldCount; }
        }

        #endregion
        #region Properties

        public DatasetBase Dataset
        {
            get { return dataset; }
        }

        public string Name
        {
            get
            {
                if (name == null)
                {
                    LoadMetadata();
                }

                return name;
            }
            internal set { name = value; }
        }

        public long RecordCount
        {
            get
            {
                if (recordCounts != null)
                {
                    return recordCounts[resultsetCounter];
                }
                else
                {
                    return -1;
                }
            }
        }

        public DatabaseObjectMetadata Metadata
        {
            get
            {
                if (metadata == null)
                {
                    LoadMetadata();
                }

                return metadata;
            }
        }

        public List<Column> Columns
        {
            get
            {
                if (columns == null)
                {
                    LoadColumns();
                }

                return columns;
            }
        }

        public List<TypeMapping> TypeMappings
        {
            get
            {
                if (typeMappings == null)
                {
                    LoadColumns();
                }

                return typeMappings;
            }
        }

        #endregion
        #region Constructors and initializers

        // TODO: rewrite this to get rowcount only from smart command
        // the rest needs to be figured out by this class (with the help of the dataset class)
        internal SmartDataReader(DatasetBase dataset, DbDataReader dataReader, List<long> recordCounts)
        {
            InitializeMembers();

            this.dataset = dataset;
            this.dataReader = dataReader;
            this.recordCounts = recordCounts;
        }

        private void InitializeMembers()
        {
            this.dataReader = null;
            this.dataReader = null;
            this.resultsetCounter = 0;
            this.recordCounts = null;
            this.name = null;
            this.metadata = null;
            this.columns = null;
            this.typeMappings = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (dataReader != null)
            {
                dataReader.Dispose();
                dataReader = null;
            }
        }

        #endregion
        #region IDataReader functions

        public override bool NextResult()
        {
            name = null;
            metadata = null;
            columns = null;

            resultsetCounter++;

            return dataReader.NextResult();
        }

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return dataReader.NextResultAsync(cancellationToken);
        }

        public override bool Read()
        {
            return dataReader.Read();
        }

        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return dataReader.ReadAsync(cancellationToken);
        }

        public override void Close()
        {
            dataReader.Close();
        }

        #endregion
        #region IDataReader schema functions

        public override DataTable GetSchemaTable()
        {
            return dataReader.GetSchemaTable();
        }

        #endregion
        #region Field functions

        public override IEnumerator GetEnumerator()
        {
            return dataReader.GetEnumerator();
        }

        public override string GetDataTypeName(int i)
        {
            return dataReader.GetDataTypeName(i);
        }

        public override Type GetFieldType(int i)
        {
            return dataReader.GetFieldType(i);
        }

        public override string GetName(int i)
        {
            return dataReader.GetName(i);
        }

        public override int GetOrdinal(string name)
        {
            return dataReader.GetOrdinal(name);
        }

        public override bool IsDBNull(int i)
        {
            return dataReader.IsDBNull(i);
        }

        #endregion
        #region Field accessors

        public override object GetValue(int i)
        {
            var value = dataReader.GetValue(i);

            if (value != null && value != DBNull.Value && typeMappings[i] != null)
            {
                return typeMappings[i].Mapping(value);
            }
            else
            {
                return value;
            }
        }

        public override int GetValues(object[] values)
        {
            var res = dataReader.GetValues(values);

            for (int i = 0; i < res; i++)
            {
                var value = values[i];

                if (value != null && value != DBNull.Value && typeMappings[i] != null)
                {
                    values[i] = typeMappings[i].Mapping(value);
                }
            }

            return res;
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        #endregion
        #region Strongly type field accessors

        public override bool GetBoolean(int i)
        {
            return dataReader.GetBoolean(i);
        }

        public override byte GetByte(int i)
        {
            return dataReader.GetByte(i);
        }

        public override short GetInt16(int i)
        {
            return dataReader.GetInt16(i);
        }

        public override int GetInt32(int i)
        {
            return dataReader.GetInt32(i);
        }

        public override long GetInt64(int i)
        {
            return dataReader.GetInt64(i);
        }

        public override float GetFloat(int i)
        {
            return dataReader.GetFloat(i);
        }

        public override double GetDouble(int i)
        {
            return dataReader.GetDouble(i);
        }

        public override decimal GetDecimal(int i)
        {
            return dataReader.GetDecimal(i);
        }

        public override Guid GetGuid(int i)
        {
            return dataReader.GetGuid(i);
        }

        public override DateTime GetDateTime(int i)
        {
            return dataReader.GetDateTime(i);
        }

        public override char GetChar(int i)
        {
            return dataReader.GetChar(i);
        }

        public override string GetString(int i)
        {
            return dataReader.GetString(i);
        }

        #endregion

        /// <summary>
        /// Loads the properties of the recordset from the schema table and additional
        /// database metadata.
        /// </summary>
        private void LoadColumns()
        {
            columns = dataset.DetectColumns(dataReader);
            typeMappings = new List<TypeMapping>();

            for (int i = 0; i < columns.Count; i++)
            {
                typeMappings.Add(null);
            }
        }

        private void LoadMetadata()
        {
            // TODO: detect additional properties
        }
    }
}
