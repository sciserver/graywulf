using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
using System.ServiceModel.Channels;
using System.Web.Routing;
using System.Security.Permissions;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.Api
{
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/{datasetName}/{tableName}?top={top}")]
        Message GetTable(string datasetName, string tableName, string top);

        [OperationContract]
        [WebInvoke(Method="PUT", UriTemplate = "/{datasetName}/{tableName}")]
        void PutTable(string datasetName, string tableName, Stream data);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [RestServiceBehavior]
    public class DataService : ServiceBase, IDataService
    {
        public Message GetTable(string datasetName, string tableName, string top)
        {
            // Get table from the schema
            var parts = tableName.Split('.');

            var dataset = FederationContext.SchemaManager.Datasets[datasetName];
            var table = dataset.Tables[dataset.DatabaseName, parts[0], parts[1]];

            // Create source

            // -- Build a query to export everything
            var codegen = SqlCodeGeneratorFactory.CreateCodeGenerator(table.Dataset);
            var sql = codegen.GenerateSelectStarQuery(
                table,
                top == null ? 0 : int.Parse(top));

            var source = new SourceTableQuery()
            {
                Dataset = dataset,
                Query = sql
            };

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
                    export.Execute();
                },
                destination.Description.MimeType);

            return message;
        }

        public void PutTable(string datasetName, string tableName, Stream data)
        {
            // Create source

            // -- Figure out file format from the content-type header
            var contentType = WebOperationContext.Current.IncomingRequest.ContentType;
            var ff = FileFormatFactory.Create(FederationContext.Federation.FileFormatFactory);
            var source = ff.CreateFileFromMimeType(contentType);

            source.Open(data, DataFileMode.Read);

            // Create destination

            // -- Get table from the schema
            var parts = tableName.Split('.');

            var dataset = FederationContext.SchemaManager.Datasets[datasetName];
            var table = new Schema.Table(dataset)
            {
                SchemaName = parts[0],
                TableName = parts[1],
            };

            var destination = new DestinationTable(table)
            {
                Options = Schema.TableInitializationOptions.Create
            };

            // Create import task

            var import = new ImportTable()
            {
                Source = source,
                Destination = destination
            };

            import.Execute();
        }
    }
}