using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
using System.ServiceModel.Channels;
using System.Security.Permissions;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    [ServiceContract]
    [ServiceName(Name = "Data", Version = "V1")]
    [Description("Manages table downloads and uploads.")]
    public interface IDataService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "OPTIONS")]
        void HandleHttpOptionsRequest();

        [OperationContract]
        [WebGet(UriTemplate = "/{datasetName}/{tableName}?top={top}")]
        [Description("Downloads a table in any supported data format.")]
        Message DownloadTable(
            [Description("Name of the dataset to download from.")]
            string datasetName,
            [Description("Name of the table to download from.")]
            string tableName,
            [Description("Maximum number of rows to return.")]
            string top);

        [OperationContract]
        [WebInvoke(Method = HttpMethod.Put, UriTemplate = "/{datasetName}/{tableName}")]
        [Description("Uploads a table to a user database.")]
        void UploadTable(
            [Description("Name of the dataset to upload to.")]
            string datasetName,
            [Description("Name of the table to upload to.")]
            string tableName,
            Stream data);

        [OperationContract]
        [WebInvoke(Method = HttpMethod.Delete, UriTemplate = "/{datasetName}/{tableName}")]
        [Description("Drops a table from the user database.")]
        void DropTable(
            [Description("Name of the dataset.")]
            string datasetName,
            [Description("Name of the table to drop.")]
            string tableName);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [RestServiceBehavior]
    public class DataService : RestServiceBase, IDataService
    {
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public Message DownloadTable(string datasetName, string tableName, string top)
        {
            int toplimit = top == null ? int.MaxValue : int.Parse(top);

            // Get table from the schema
            var dataset = FederationContext.SchemaManager.Datasets[datasetName];

            // Set a hard limit on the number of rows returned for datasets
            // not owned by the user
            if (!dataset.IsMutable)
            {
                toplimit = Math.Min(toplimit, 5000);        // TODO: make this a setting
            }

            var table = ParserTableName(dataset, tableName);
            table = dataset.Tables[dataset.DatabaseName, table.SchemaName, table.ObjectName];

            // Create source

            // -- Build a query to export everything
            var source = SourceTableQuery.Create(table, toplimit);

            // Pick a random server, if necessary
            if (table.Dataset is GraywulfDataset)
            {
                var ds = (GraywulfDataset)table.Dataset;

                if (!ds.DatabaseDefinitionReference.IsEmpty)
                {
                    var dd = ds.DatabaseDefinitionReference.Value;
                    var di = dd.GetRandomDatabaseInstance(Registry.Constants.ProdDatabaseVersionName);

                    source.Dataset = new SqlServerDataset()
                    {
                        Name = ds.Name,
                        ConnectionString = di.GetConnectionString().ConnectionString
                    };
                }
            }

            // Create destination

            // -- Figure out output type from request header
            var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();
            var ff = FileFormatFactory.Create(FederationContext.Federation.FileFormatFactory);
            var destination = ff.CreateFileFromAcceptHeader(accept);

            // Create export task
            var export = new ExportTable()
            {
                Source = source,
                Destination = destination
            };

            // Set content type based on output file format
            var message = WebOperationContext.Current.CreateStreamResponse(
                stream =>
                {
                    export.Destination.Open(stream, DataFileMode.Write);

                    // TODO: make this whole service async
                    Jhu.Graywulf.Util.TaskHelper.Wait(export.ExecuteAsync());
                },
                destination.Description.MimeType);

            return message;
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void UploadTable(string datasetName, string tableName, Stream data)
        {
            // Get dataset name from the parameters
            var dataset = (Schema.SqlServer.SqlServerDataset)FederationContext.SchemaManager.Datasets[datasetName];

            // Make sure the user has access to this dataset.
            // Because we have no access control in the registry yet, it is simply enough
            // to check whether the dataset is mutable
            if (!dataset.IsMutable)
            {
                throw new System.Security.SecurityException("Access denied.");  // TODO
            }

            // Create source

            // -- Figure out file format from the content-type header
            var contentType = WebOperationContext.Current.IncomingRequest.ContentType;
            var ff = FileFormatFactory.Create(FederationContext.Federation.FileFormatFactory);
            var source = ff.CreateFileFromMimeType(contentType);

            source.Open(data, DataFileMode.Read);

            // Create destination

            // Use a SQL parser to extract table name parts from the string
            var table = ParserTableName(dataset, tableName);

            // Create a destination object targeted to the specific table
            var destination = new IO.Tasks.DestinationTable(table, Schema.TableInitializationOptions.Create);

            // Create import task and execute it
            var import = new ImportTable()
            {
                Source = source,
                Destination = destination
            };

            // TODO: async
            Jhu.Graywulf.Util.TaskHelper.Wait(import.ExecuteAsync());
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void DropTable(string datasetName, string tableName)
        {
            // Get table from the schema
            var dataset = FederationContext.SchemaManager.Datasets[datasetName];

            // Make sure the user has access to this dataset.
            // Because we have no access control in the registry yet, it is simply enough
            // to check whether the dataset is mutable
            if (!dataset.IsMutable)
            {
                throw new System.Security.SecurityException("Access denied.");  // TODO
            }

            var table = ParserTableName(dataset, tableName);
            table = dataset.Tables[dataset.DatabaseName, table.SchemaName, table.ObjectName];

            table.Drop();
        }

        private Schema.Table ParserTableName(Schema.DatasetBase dataset, string tableName)
        {
            // Use a SQL parser to extract table name parts from the string
            var parser = new Sql.Parsing.SqlParser();
            var tn = (Sql.Parsing.TableOrViewName)parser.Execute(new Sql.Parsing.TableOrViewName(), tableName);
            var tr = tn.TableReference;
            tr.SubstituteDefaults(FederationContext.SchemaManager, dataset.Name);

            return new Schema.Table()
            {
                Dataset = dataset,
                DatabaseName = dataset.DatabaseName,
                SchemaName = tr.SchemaName,
                ObjectName = tr.DatabaseObjectName,
            };
        }
    }
}