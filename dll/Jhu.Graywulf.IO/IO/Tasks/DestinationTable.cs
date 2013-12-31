using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    public class DestinationTable
    {

        private SqlServerDataset dataset;
        private string databaseName;
        private string schemaName;
        private string tableName;
        private TableInitializationOptions options;

        public SqlServerDataset Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        public TableInitializationOptions Options
        {
            get { return options; }
            set { options = value; }
        }

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

        public Table GetTable()
        {
            return new Table(dataset)
            {
                DatabaseName = this.databaseName,
                SchemaName = this.schemaName,
                TableName = this.tableName,
            };
        }
    }
}
