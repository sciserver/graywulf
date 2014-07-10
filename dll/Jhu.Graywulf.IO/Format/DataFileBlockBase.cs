using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Jhu.Graywulf.Schema;
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

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
            protected set { name = value; }
        }

        public long RecordCount
        {
            get { return recordCount; }
            protected set { recordCount = value; }
        }

        public DatabaseObjectMetadata Metadata
        {
            get { return metadata; }
            protected set { metadata = value; }
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
        }

        private void CopyMembers(DataFileBlockBase old)
        {
            this.file = old.file;
            this.name = old.name;
            this.recordCount = old.recordCount;
            this.metadata = Util.DeepCloner.CloneObject(old.metadata);
            this.columns = new List<Column>(Util.DeepCloner.CloneCollection(old.columns));
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
        protected void CreateColumns(IList<Column> dataReaderColumns)
        {
            this.columns.Clear();

            if (file.FileMode == DataFileMode.Read && file.GenerateIdentityColumn)
            {
                var col = new Column("__ID", DataTypes.SqlBigInt);  // *** TODO
                col.IsIdentity = true;
                this.columns.Add(col);
            }

            this.columns.AddRange(dataReaderColumns);

            OnColumnsCreated();
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

        protected internal abstract void OnReadHeader();

        /// <summary>
        /// When overriden in derived classes, reads the next data row from
        /// the data file
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected internal abstract bool OnReadNextRow(object[] values);

        /// <summary>
        /// When overriden in a derived class, reads all remaining data rows.
        /// </summary>
        protected internal abstract void OnReadToFinish();

        /// <summary>
        /// When overriden in derived classes, reads the file block footer.
        /// </summary>
        protected internal abstract void OnReadFooter();

        #endregion
        #region Write functions

        internal void Write(ISmartDataReader dr)
        {
            var values = new object[dr.FieldCount];

            OnWriteHeader();

            while (dr.Read())
            {
                dr.GetValues(values);
                OnWriteNextRow(values);
            }

            OnWriteFooter();
        }

        protected abstract void OnWriteHeader();

        protected abstract void OnWriteNextRow(object[] values);

        protected abstract void OnWriteFooter();

        #endregion
    }
}
