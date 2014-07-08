using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.IO.Tasks
{
    /// <summary>
    /// Represents a query, with associated data source and metadata,
    /// that can be used as a source of a data copy operation.
    /// </summary>
    [Serializable]
    public class SourceTableQuery : ICloneable
    {
        #region Private member variables

        private DatasetBase dataset;
        private string query;
        private List<Column> columns;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the dataset on which this query can be executed.
        /// </summary>
        public DatasetBase Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        /// <summary>
        /// Gets or sets the text of the query.
        /// </summary>
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        /// <summary>
        /// Gets a list of columns with associated metadata.
        /// </summary>
        public IList<Column> Columns
        {
            get { return columns; }
        }

        #endregion
        #region Constructors and initializers

        public SourceTableQuery()
        {
            InitializeMembers();
        }

        public SourceTableQuery(SourceTableQuery old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.dataset = null;
            this.query = null;
            this.columns = null;
        }

        private void CopyMembers(SourceTableQuery old)
        {
            this.dataset = Util.DeepCloner.CloneObject(old.dataset);
            this.query = old.query;
            this.columns = null;
        }

        public object Clone()
        {
            return new SourceTableQuery(this);
        }

        #endregion

        /// <summary>
        /// Opens a connection to the underlying dataset.
        /// </summary>
        /// <returns></returns>
        internal IDbConnection OpenConnection()
        {
            return dataset.OpenConnection();
        }

        /// <summary>
        /// Creates a database command that can be used to execute the query.
        /// </summary>
        /// <returns></returns>
        internal ISmartCommand CreateCommand()
        {
            var cmd = dataset.CreateCommand();

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;

            return cmd;
        }

        private void DetectColumns()
        {
            using (var cn = OpenConnection())
            {
                using (var cmd = cn.CreateCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = query;

                    using (var dr = new SmartDataReader(
                        dataset, 
                        cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo)))
                    {
                        CreateColumns(dr.GetColumns());
                    }
                }
            }
        }

        private void CreateColumns(IList<Column> columns)
        {
            columns.Clear();

            this.columns.AddRange(columns);
        }

        public IList<Column> GetColumns()
        {
            // *** TODO
            throw new NotImplementedException();
        }
    }
}
