using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a source table.")]
    public class SourceTable
    {
        private string dataset;
        private string table;

        [DataMember(Name = "dataset")]
        [Description("Source dataset.")]
        public string Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        [DataMember(Name = "table")]
        [Description("Name of source table.")]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        public IO.Tasks.SourceTableQuery GetSourceTableQuery(FederationContext context)
        {
            TableOrView sourcetable;

            if (String.IsNullOrWhiteSpace(table) ||
                String.IsNullOrWhiteSpace(dataset))
            {
                throw new InvalidOperationException("Source dataset and table must be specified"); // TODO ***
            }

            // Parse table name and create source object
            if (!Util.SqlParser.TryParseTableName(context, dataset, table, out sourcetable))
            {
                throw new InvalidOperationException("Invalid table name");    // TODO ***
            }

            sourcetable.Dataset = context.SchemaManager.Datasets[dataset];

            // Make sure dataset is a user dataset, do not allow export from big catalogs
            if (!sourcetable.Dataset.IsMutable)
            {
                throw new InvalidOperationException("Cannot read data from the specified dataset.");  // TODO ***
            }

            var sourcequery = IO.Tasks.SourceTableQuery.Create(sourcetable);

            return sourcequery;
        }
    }
}
