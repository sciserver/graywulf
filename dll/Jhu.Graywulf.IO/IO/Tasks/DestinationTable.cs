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
        private string tableNameTemplate;
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
        /// <remarks>
        /// The table name template can contain standard .Net format string
        /// where [$BatchName] is the name of the file, [$ResultsetName] is the name of the resultset
        /// inside the file or the name of the table, or the resultset counter
        /// if the name is not specified.
        /// </remarks>
        public string TableNameTemplate
        {
            get { return tableNameTemplate; }
            set { tableNameTemplate = value; }
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

        /// <summary>
        /// Creates a new destination object.
        /// </summary>
        public DestinationTable()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a destination object based on a target table.
        /// </summary>
        /// <param name="table"></param>
        public DestinationTable(Table table)
        {
            InitializeMembers();

            this.dataset = (SqlServerDataset)table.Dataset;
            this.databaseName = table.DatabaseName;
            this.schemaName = table.SchemaName;
            this.tableNameTemplate = table.TableName;
        }

        public DestinationTable(SqlServerDataset dataset, string databaseName, string schemaName, string tableName, TableInitializationOptions options)
        {
            InitializeMembers();

            this.dataset = dataset;
            this.databaseName = databaseName;
            this.schemaName = schemaName;
            this.tableNameTemplate = tableName;
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
            this.tableNameTemplate = null;
            this.options = TableInitializationOptions.Create;
        }

        private void CopyMembers(DestinationTable old)
        {
            this.dataset = old.dataset;
            this.databaseName = old.databaseName;
            this.schemaName = old.schemaName;
            this.tableNameTemplate = old.tableNameTemplate;
            this.options = old.options;
        }

        #endregion

        private string GetTableName(string batchName, string resultsetName)
        {
            var tableName = tableNameTemplate.Replace("[$BatchName]", batchName);
            tableName = tableName.Replace("[$ResultsetName]", resultsetName);

            return tableName;
        }

        /// <summary>
        /// Creates a table object refering to the destination table
        /// without substituting the tokens in the table name template.
        /// </summary>
        /// <returns></returns>
        public Table GetTable()
        {
            return new Table(dataset)
            {
                DatabaseName = this.databaseName,
                SchemaName = this.schemaName,
                TableName = this.tableNameTemplate,
            };
        }

        /// <summary>
        /// Creates a table object refering to the destination table. This
        /// function substitutes the tokens in the table name template.
        /// </summary>
        /// <param name="batchName"></param>
        /// <param name="resultsetName"></param>
        /// <returns></returns>
        public Table GetTable(string batchName, string resultsetName, DatabaseObjectMetadata metadata)
        {
            return new Table(dataset)
            {
                DatabaseName = this.databaseName,
                SchemaName = this.schemaName,
                TableName = GetTableName(batchName, resultsetName),
            };

            // TODO: attach metadata to table
        }
    }
}
