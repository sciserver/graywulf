using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Implements core functions to read and write data file blocks.
    /// A data file blocks corresponds to tables.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class DataFileBlockBase : ICloneable
    {
        #region Private member variables

        /// <summary>
        /// Holds a reference to the underlying file
        /// </summary>
        /// <remarks>
        /// This value is set by the constructor when a new data file block
        /// is created based on a data file.
        /// </remarks>
        [NonSerialized]
        protected DataFileBase file;

        private string name;
        private long recordCount;
        private DatabaseObjectMetadata metadata;

        /// <summary>
        /// Collection of table columns
        /// </summary>
        [NonSerialized]
        private List<Column> columns;

        /// <summary>
        /// Collection of type mappings
        /// </summary>
        [NonSerialized]
        private List<TypeMapping> columnTypeMappings;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public long RecordCount
        {
            get { return recordCount; }
            protected set { recordCount = value; }
        }

        public DatabaseObjectMetadata Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }

        /// <summary>
        /// Gets the collection containing columns of the data file
        /// </summary>
        [IgnoreDataMember]
        public List<Column> Columns
        {
            get { return columns; }
        }

        /// <summary>
        /// Gets the collection of data file columns for XML serialization.
        /// Do not use.
        /// </summary>
        [DataMember(Name = "Columns")]
        [XmlArray]
        public Column[] Columns_ForXml
        {
            get { return columns.ToArray(); }
            set { columns = new List<Schema.Column>(value); }
        }

        [IgnoreDataMember]
        public List<TypeMapping> ColumnTypeMappings
        {
            get { return columnTypeMappings; }
        }

        #endregion
        #region Constructors and initializers

        protected DataFileBlockBase()
        {
            InitializeMembers(new StreamingContext());
        }

        public DataFileBlockBase(DataFileBase file)
        {
            InitializeMembers(new StreamingContext());

            this.file = file;
        }

        public DataFileBlockBase(DataFileBlockBase old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.file = null;
            this.name = null;
            this.recordCount = -1;
            this.metadata = null;
            this.columns = new List<Column>();
            this.columnTypeMappings = new List<TypeMapping>();
        }

        private void CopyMembers(DataFileBlockBase old)
        {
            this.file = old.file;
            this.name = old.name;
            this.recordCount = old.recordCount;
            this.metadata = Util.DeepCloner.CloneObject(old.metadata);
            this.columns = new List<Column>(Util.DeepCloner.CloneCollection(old.columns));
            this.columnTypeMappings = new List<TypeMapping>(Util.DeepCloner.CloneCollection(old.columnTypeMappings));
        }

        internal void SetProperties(ISmartDataReader dr)
        {
            this.name = dr.Name;
            this.recordCount = dr.RecordCount;
            this.metadata = Util.DeepCloner.CloneObject(dr.Metadata);

            // File blocks can predefine columns when writing files. If columns are
            // not predefined, simply create new columns based on the data reader. If there
            // are columns already defined, use them but compare with reality first.
            if (this.columns == null || this.columns.Count == 0)
            {
                CreateColumns(dr.Columns);
            }
            else
            {
                VerifyColumns(dr.Columns);
            }
        }

        public abstract object Clone();

        #endregion
        #region Column functions

        /// <summary>
        /// Call this function to set column list from derived classes
        /// </summary>
        /// <param name="dataReaderColumns"></param>
        public void CreateColumns(IList<Column> dataReaderColumns)
        {
            this.columns.Clear();

            if (file.FileMode == DataFileMode.Read && file.GenerateIdentityColumn)
            {
                var col = new Column("ID", DataTypes.SqlBigInt);
                col.IsIdentity = true;
                col.IsKey = true;
                this.columns.Add(col);

                this.columnTypeMappings.Add(null);
            }

            foreach (var c in dataReaderColumns)
            {
                this.columns.Add(c);
                this.columnTypeMappings.Add(null);
            }

            // Renumber columns
            for (int i = 0; i < this.columns.Count; i ++)
            {
                this.columns[i].ID = i + 1;
            }

            MakeColumnNamesUnique();
            OnColumnsCreated();
        }

        private void MakeColumnNamesUnique()
        {
            var names = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            for (int i = 0; i < this.columns.Count; i++)
            {
                var name = columns[i].Name;

                int q = 0;
                while (names.Contains(name))
                {
                    q++;
                    name = String.Format("{0}_{1}", columns[i].Name, q);
                }

                columns[i].Name = name;
                names.Add(name);
            }
        }

        /// <summary>
        /// When writing the file, detects the columns from the data reader
        /// providing the data to be written.
        /// </summary>
        /// <param name="dr"></param>
        internal void VerifyColumns(IList<Column> dataReaderColumns)
        {
            throw new NotImplementedException();

            /* TODO: old code, fix
            // See if predefined columns can be used
            if (this.columns.Count == dataReaderColumns.Count)
            {
                // *** TODO verify type mismatch, or update types
                // keep column name and format
                throw new NotImplementedException();
            }
            else
            {
                CreateColumns(cols);
            }*/
        }

        /// <summary>
        /// Called by the framework after columns are created.
        /// </summary>
        /// <remarks>
        /// Use this function to wire up parser and serializer delegates.
        /// </remarks>
        protected abstract void OnColumnsCreated();

        #endregion
        #region Read functions

        /// <summary>
        /// When implemented in derived classes, reads the header of the file
        /// block and extracts columns and metadata.
        /// </summary>
        protected internal abstract Task OnReadHeaderAsync();

        // TODO: add comments and rename to more meaningful
        protected internal virtual void OnSetMetadata(int blockCounter)
        {
            Name = blockCounter == 0 ? "" : blockCounter.ToString();
            Metadata = null;    // TODO: set metadata
        }

        /// <summary>
        /// When implemented in derived classes, reads the next data row from
        /// the data file
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected internal abstract Task<bool> OnReadNextRowAsync(object[] values);

        /// <summary>
        /// When implemented in a derived class, reads all remaining data rows
        /// and advanced the file to the begining of the next file block.
        /// </summary>
        protected internal abstract Task OnReadToFinishAsync();

        /// <summary>
        /// When overriden in derived classes, reads the file block footer.
        /// </summary>
        protected internal abstract Task OnReadFooterAsync();

        #endregion
        #region Write functions

        internal async Task WriteFromDataReaderAsync(ISmartDataReader dr)
        {
            // Apply type mappings to the data reader

            // TODO: create function on dr to do this
            dr.TypeMappings.Clear();
            dr.TypeMappings.AddRange(columnTypeMappings);

            // Certain data files store the number of data rows in the header.
            // This prevents streaming of query results directly into the file
            // because we don't know the number of rows without enumerating the
            // entire resultset. There are two solutions: either count the records
            // before streaming down the data form the server, or buffer the results
            // into memory, count the rows, write the header and then stream the rows
            // out into the file. Buffering rows is obviously very expensive.

            // SmartCommand and SmartDataReader support counting rows before query
            // execution by running a COUNT(*) query first and the number of columns
            // is stored in the RecordCount property by the function SetProperties.
            // If the rows are counted, the value of the RecordCount property should be
            // larger than -1. If the value is -1 the resultset needs to be buffered
            // first.

            if (recordCount == -1 && file.Description.RequiresRecordCount)
            {
                // Buffering required
                var buffer = new List<object[]>();

                while (dr.Read())
                {
                    var values = new object[dr.FieldCount];

                    dr.GetValues(values);

                    buffer.Add(values);
                }

                recordCount = buffer.Count;

                // Now that the number of rows in known, the header can be written
                await OnWriteHeaderAsync();

                // Stream out rows from the buffer
                foreach (var values in buffer)
                {
                    await OnWriteNextRowAsync(values);
                }
            }
            else
            {
                // We already know the number of rows, or the number of rows
                // is not needed to write the header
                await OnWriteHeaderAsync();

                // Stream out data directly from the data reader.
                var values = new object[dr.FieldCount];
                while (dr.Read())
                {
                    dr.GetValues(values);
                    await OnWriteNextRowAsync(values);
                }
            }

            await OnWriteFooterAsync();
        }

        /// <summary>
        /// When implemented in derived classes, writes the header of the file
        /// block.
        /// </summary>
        protected abstract Task OnWriteHeaderAsync();

        /// <summary>
        /// When implemented in derived classes, writes the next data row into
        /// the file block.
        /// </summary>
        /// <param name="values"></param>
        protected abstract Task OnWriteNextRowAsync(object[] values);

        /// <summary>
        /// When implemented in derived classes, writes the footer of the
        /// file block.
        /// </summary>
        protected abstract Task OnWriteFooterAsync();

        #endregion
    }
}
