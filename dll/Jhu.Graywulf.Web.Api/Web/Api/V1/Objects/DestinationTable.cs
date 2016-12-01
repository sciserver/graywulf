using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a destination table.")]
    public class DestinationTable
    {
        private string dataset;
        private string table;

        [DataMember(Name = "dataset")]
        [Description("Destination dataset.")]
        public string Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        [DataMember(Name = "table")]
        [Description("Name of destination table.")]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        public IO.Tasks.DestinationTable GetDestinationTable(FederationContext context)
        {
            if (String.IsNullOrWhiteSpace(dataset))
            {
                throw new InvalidOperationException("Destination dataset must be specified"); // TODO ***
            }

            if (!String.IsNullOrWhiteSpace(table))
            {
                string schemaName, tableName;
                if (Util.SqlParser.TryParseTableName(context, table, out schemaName, out tableName))
                {
                    return GetDestinationTable(context, dataset, schemaName, tableName);
                }
                else
                {
                    throw new InvalidOperationException("Cannot parse destination table name."); // TODO ***
                }
            }

            return GetDestinationTable(context, dataset, null, null);
        }


        public static IO.Tasks.DestinationTable GetDestinationTable(FederationContext context, string datasetName, string token)
        {
            string schemaName, tableName;

            if (Util.SqlParser.TryParseTableName(context, token, out schemaName, out tableName))
            {
                return GetDestinationTable(context, datasetName, schemaName, tableName);
            }
            else
            {
                return GetDestinationTable(context, datasetName, null, null);
            }
        }

        private static IO.Tasks.DestinationTable GetDestinationTable(FederationContext context, string datasetName, string schemaName, string tableName)
        {
            var dataset = (SqlServerDataset)context.SchemaManager.Datasets[datasetName];

            // Make sure dataset is a user dataset
            if (!dataset.IsMutable)
            {
                throw new ArgumentException("Cannot import data into the specified dataset.");  // TODO ***
            }

            var destination = new IO.Tasks.DestinationTable(
                    dataset,
                    dataset.DatabaseName,
                    dataset.DefaultSchemaName,
                    IO.Constants.ResultsetNameToken,        // generate table names automatically
                    TableInitializationOptions.Create | TableInitializationOptions.GenerateUniqueName);

            if (!String.IsNullOrWhiteSpace(schemaName))
            {
                destination.SchemaName = schemaName;
            }

            if (!String.IsNullOrWhiteSpace(tableName))
            {
                destination.TableNamePattern = tableName;   // TODO: handle patterns?
            }

            return destination;
        }
    }
}