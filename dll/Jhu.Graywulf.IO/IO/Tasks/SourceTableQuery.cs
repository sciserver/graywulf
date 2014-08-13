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
        private string sourceObjectName;
        private string query;

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
        /// Gets or sets the name table from which data is returned.
        /// </summary>
        public string SourceObjectName
        {
            get { return sourceObjectName; }
            set { sourceObjectName = value; }
        }

        /// <summary>
        /// Gets or sets the text of the query.
        /// </summary>
        public string Query
        {
            get { return query; }
            set { query = value; }
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

        public static SourceTableQuery Create(TableOrView table)
        {
            return Create(table, Int32.MaxValue);
        }

        public static SourceTableQuery Create(TableOrView table, int top)
        {
            var cg = table.Dataset.CreateCodeGenerator();

            return new SourceTableQuery()
            {
                Dataset = table.Dataset,
                SourceObjectName = table.DisplayName,
                Query = cg.GenerateSelectStarQuery(table, top)
            };
        }

        private void InitializeMembers()
        {
            this.dataset = null;
            this.sourceObjectName = null;
            this.query = null;
        }

        private void CopyMembers(SourceTableQuery old)
        {
            this.dataset = Util.DeepCloner.CloneObject(old.dataset);
            this.sourceObjectName = old.sourceObjectName;
            this.query = old.query;
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

        public IList<Column> GetColumns()
        {
            using (var cn = OpenConnection())
            {
                using (var cmd = CreateCommand())
                {
                    cmd.Connection = cn;

                    using (var dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                    {
                        // TODO: test this
                        return dr.Columns;
                    }
                }
            }
        }
    }
}
