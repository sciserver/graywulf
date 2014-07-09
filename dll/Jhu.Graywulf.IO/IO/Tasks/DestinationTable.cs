using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.IO.Tasks
{
    /// <summary>
    /// Represents the destination of a table copy operation.
    /// </summary>
    [Serializable]
    public class DestinationTable
    {
        // TODO: figure out automatic table naming when importing
        // multiple tables from archives etc.

        #region Private member variables

        private SqlServerDataset dataset;
        private string databaseName;
        private string schemaName;
        private string tableName;
        private TableInitializationOptions options;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the destination dataset.
        /// </summary>
        public SqlServerDataset Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        /// <summary>
        /// Gets or sets the name of the destination database.
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Gets or sets the schema name of the destination table.
        /// </summary>
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the destination table.
        /// </summary>
        public string TableName
        {
            // TODO: modify this to table name pattern to support multi-table imports.

            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// Gets or sets the destination table initialization method.
        /// </summary>
        public TableInitializationOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        #endregion
        #region Constructors and initializers

        public DestinationTable()
        {
            InitializeMembers();
        }

        public DestinationTable(Table table)
        {
            InitializeMembers();

            this.dataset = (SqlServerDataset)table.Dataset;
            this.databaseName = table.DatabaseName;
            this.schemaName = table.SchemaName;
            this.tableName = table.TableName;
        }

        public DestinationTable(SqlServerDataset dataset, string databaseName, string schemaName, string tableName, TableInitializationOptions options)
        {
            InitializeMembers();

            this.dataset = dataset;
            this.databaseName = databaseName;
            this.schemaName = schemaName;
            this.tableName = tableName;
            this.options = options;
        }

        public DestinationTable(DestinationTable old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.dataset = null;
            this.databaseName = null;
            this.schemaName = null;
            this.tableName = null;
            this.options = TableInitializationOptions.Create;
        }

        private void CopyMembers(DestinationTable old)
        {
            this.dataset = old.dataset;
            this.databaseName = old.databaseName;
            this.schemaName = old.schemaName;
            this.tableName = old.tableName;
            this.options = old.options;
        }

        #endregion

        public Table GetTable()
        {
            // TODO: this needs to be modified for multi-table imports
            // to generate table name automatically

            return new Table(dataset)
            {
                DatabaseName = this.databaseName,
                SchemaName = this.schemaName,
                TableName = this.tableName,
            };
        }
    }
}
