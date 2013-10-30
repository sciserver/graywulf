using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Types;

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

        public DataFileBlockBase(DataFileBase file)
        {
            InitializeMembers();

            this.file = file;
        }

        private void InitializeMembers()
        {
            this.file = null;
            this.columns = new List<DataFileColumn>();
        }

        protected internal abstract void OnReadHeader();

        protected void CreateColumns(DataFileColumn[] columns)
        {
            this.columns.Clear();

            if (file.GenerateIdentityColumn)
            {
                var col = new DataFileColumn("__ID", DataType.BigInt);
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
    }
}
