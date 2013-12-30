using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    public abstract class DataFileBlockBase : ICloneable
    {
        [NonSerialized]
        protected DataFileBase file;
        
        private List<Column> columns;

        /// <summary>
        /// Gets the collection containing columns of the data file
        /// </summary>
        public List<Column> Columns
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
            this.columns = new List<Column>();
        }

        private void CopyMembers(DataFileBlockBase old)
        {
            this.file = old.file;

            // Deep copy columns
            this.columns = new List<Column>();
            foreach (var c in old.columns)
            {
                this.columns.Add((Column)c.Clone());
            }
        }

        public abstract object Clone();

        #endregion

        #region Columns functions

        /// <summary>
        /// Generates a column list from a data reader.
        /// </summary>
        /// <param name="dr"></param>
        internal void DetectColumns(IDataReader dr)
        {
            var dt = dr.GetSchemaTable();
            Column[] cols;

            if (this.Columns.Count == dt.Rows.Count)
            {
                cols = Columns.ToArray();

                // *** TODO verify type mismatch, or update types
                // keep column name and format
            }
            else
            {
                cols = new Column[dt.Rows.Count];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cols[i] = Column.Create(dt.Rows[i]);
                }
            }

            CreateColumns(cols);
        }

        /// <summary>
        /// Call this function to set column list from derived classes
        /// </summary>
        /// <param name="columns"></param>
        protected void CreateColumns(Column[] columns)
        {
            this.columns.Clear();

            if (file.FileMode == DataFileMode.Read && file.GenerateIdentityColumn)
            {
                var col = new Column("__ID", DataType.SqlBigInt);  // *** TODO
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
