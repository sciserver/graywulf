using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.IO.Tasks
{
    /// <summary>
    /// Represents the destination of a table copy operation.
    /// </summary>
    [Serializable]
    [DataContract]
    public class DestinationTable
    {
        // TODO: figure out automatic table naming when importing
        // multiple tables from archives etc.

        #region Private member variables

        [IgnoreDataMember]
        private SqlServerDataset dataset;

        [IgnoreDataMember]
        private string databaseName;

        [IgnoreDataMember]
        private string schemaName;

        [IgnoreDataMember]
        private string tableNamePattern;

        [IgnoreDataMember]
        private TableInitializationOptions options;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the destination dataset.
        /// </summary>
        [DataMember]
        public SqlServerDataset Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        /// <summary>
        /// Gets or sets the name of the destination database.
        /// </summary>
        [DataMember]
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Gets or sets the schema name of the destination table.
        /// </summary>
        [DataMember]
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the destination table.
        /// </summary>
        [DataMember]
        public string TableNamePattern
        {
            get { return tableNamePattern; }
            set { tableNamePattern = value; }
        }

        /// <summary>
        /// Gets or sets the destination table initialization method.
        /// </summary>
        [DataMember]
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
        public DestinationTable(Table table, TableInitializationOptions options)
        {
            InitializeMembers();

            this.dataset = (SqlServerDataset)table.Dataset;
            this.databaseName = table.DatabaseName;
            this.schemaName = table.SchemaName;
            this.tableNamePattern = table.TableName;
            this.options = options;
        }

        public DestinationTable(SqlServerDataset dataset, string databaseName, string schemaName, string tableName, TableInitializationOptions options)
        {
            InitializeMembers();

            this.dataset = dataset;
            this.databaseName = databaseName;
            this.schemaName = schemaName;
            this.tableNamePattern = tableName;
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
            this.tableNamePattern = null;
            this.options = TableInitializationOptions.Create;
        }

        private void CopyMembers(DestinationTable old)
        {
            this.dataset = old.dataset;
            this.databaseName = old.databaseName;
            this.schemaName = old.schemaName;
            this.tableNamePattern = old.tableNamePattern;
            this.options = old.options;
        }

        #endregion

        private string GetTableName(string batchName, string queryName, string resultsetName)
        {
            // When importing archives:
            // -- Batch name is usually the archive name
            // -- Query name is the file name
            // -- Resultset name is the table within the file

            var name = "";

            if (!String.IsNullOrWhiteSpace(batchName))
            {
                name += "_" + batchName;
            }

            if (!String.IsNullOrWhiteSpace(queryName))
            {
                name += "_" + queryName;
            }

            if (!String.IsNullOrWhiteSpace(resultsetName))
            {
                name += "_" + resultsetName;
            }

            return tableNamePattern.Replace(Constants.ResultsetNameToken, name.Substring(1));
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
                TableName = this.tableNamePattern,
            };
        }

        /// <summary>
        /// Creates a table object refering to the destination table. This
        /// function substitutes the tokens in the table name template.
        /// </summary>
        /// <param name="batchName"></param>
        /// <param name="resultsetName"></param>
        /// <returns></returns>
        public Table GetTable(string batchName, string queryName, string resultsetName, DatabaseObjectMetadata metadata)
        {
            return new Table(dataset)
            {
                DatabaseName = this.databaseName,
                SchemaName = this.schemaName,
                TableName = GetTableName(batchName, queryName, resultsetName),
            };

            // TODO: attach metadata to table
        }
    }
}
