using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Types;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    public abstract class DataFileBlockBase
    {
        [NonSerialized]
        protected DataFileBase file;
        
        private List<DataFileColumn> columns;

        /// <summary>
        /// Gets the collection containing columns of the data file
        /// </summary>
        public List<DataFileColumn> Columns
        {
            get { return columns; }
        }

        #region Constructors and initializer

        public DataFileBlockBase(DataFileBase file)
        {
            InitializeMembers();

            this.file = file;
        }

        public DataFileBlockBase(DataFileBlockBase old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.file = null;
            this.columns = new List<DataFileColumn>();
        }

        private void CopyMembers(DataFileBlockBase old)
        {
            this.file = old.file;
            this.columns = new List<DataFileColumn>(old.columns);
        }

        #endregion

        #region Columns functions

        /// <summary>
        /// Generates a column list from a data reader.
        /// </summary>
        /// <param name="dr"></param>
        internal void DetectColumns(IDataReader dr)
        {
            var dt = dr.GetSchemaTable();
            DataFileColumn[] cols;

            if (this.Columns.Count == dt.Rows.Count)
            {
                cols = Columns.ToArray();

                // *** TODO verify type mismatch, or update types
                // keep column name and format
            }
            else
            {
                cols = new DataFileColumn[dt.Rows.Count];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cols[i] = DataFileColumn.Create(dt.Rows[i]);
                }
            }

            CreateColumns(cols);
        }

        /// <summary>
        /// Call this function to set column list from derived classes
        /// </summary>
        /// <param name="columns"></param>
        protected void CreateColumns(DataFileColumn[] columns)
        {
            this.columns.Clear();

            if (file.FileMode == DataFileMode.Read && file.GenerateIdentityColumn)
            {
                var col = new DataFileColumn("__ID", DataType.SqlBigInt);  // *** TODO
                col.IsIdentity = true;
                this.columns.Add(col);
            }

            this.columns.AddRange(columns);

            OnColumnsCreated();
        }

        /// <summary>
        /// Called by the framework after columns are created.
        /// </summary>
        /// <remarks>
        /// This function can be used the wire up parsing and serializer delegates.
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

        internal void Write(IDataReader dr)
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
