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
            var pattern = tableNamePattern ?? Constants.ResultsetNameToken;

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

            if (name.Length > 0)
            {
                name = name.Substring(1);
            }

            // Substitute into name pattern
            // Strip leading _
            name = pattern.Replace(Constants.ResultsetNameToken, name);

            if ((options & TableInitializationOptions.GenerateUniqueName) != 0)
            {
                name = GetUniqueTableName(name);
            }

            return name;
        }

        private string GetUniqueTableName(string tableName)
        {
            string newname = tableName;
            int q = 1;

            while (dataset.GetObject(dataset.DatabaseName, schemaName, newname) != null)
            {
                newname = String.Format("{0}_{1}", tableName, q);
                q++;
            }

            return newname;
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

        public Table GetQueryOutputTable(string batchName, string queryName, string resultsetName, DatabaseObjectMetadata metadata)
        {
            var table = GetTable(batchName, queryName, resultsetName, metadata);

            // Generate unique name, if necessary
            if ((options & TableInitializationOptions.GenerateUniqueName) != 0)
            {
                // If table name is generated automatically, make sure to use the same name
                // for every call of GetTableName so turn off automatic name creation here
                // TODO: this might not be the best solution if there are mutliple output
                // tables, so review this behavior when implementing multi-step queries
                tableNamePattern = table.TableName;
                options &= ~TableInitializationOptions.GenerateUniqueName;
            }

            return table;
        }

        /// <summary>
        /// Tests whether the destination table exists before query execution.
        /// </summary>
        public void CheckTableExistence()
        {
            var table = GetTable();
            var exists = table.IsExisting;

            if (exists)
            {
                // Make sure table isn't in the way
                if ((options & TableInitializationOptions.Append) == 0 &&
                    (options & TableInitializationOptions.Drop) == 0 &&
                    (options & TableInitializationOptions.GenerateUniqueName) == 0)
                {
                    throw new TableCopyException(ExceptionMessages.DestinationTableExists);
                }
            }
            if (!exists)
            {
                // Make sure query is parameterized to create the table
                if ((options & TableInitializationOptions.Create) == 0)
                {
                    throw new TableCopyException(ExceptionMessages.DestinationTableNotExists);
                }
            }
        }
    }
}
