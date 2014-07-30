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
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/")]
        DatasetList ListDatasets();

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/{datasetName}/tables")]
        TableList ListTables(string datasetName);

        [OperationContract]
        [WebGet(UriTemplate = "/{datasetName}/tables/{tableName}?top={top}")]
        Message GetTable(string datasetName, string tableName, string top);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [RestServiceBehavior]
    public class DataService : ServiceBase, IDataService
    {
        public DatasetList ListDatasets()
        {
            return new DatasetList()
            {
                Datasets = FederationContext.SchemaManager.Datasets.Values.Select(d => new Dataset(d)).ToArray()
            };
        }

        public TableList ListTables(string datasetName)
        {
            var dataset = FederationContext.SchemaManager.Datasets[datasetName];
            dataset.Tables.LoadAll();

            return new TableList()
            {
                Tables = dataset.Tables.Values.Select(t => new Table(t)).ToArray(),
            };
        }

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
    }
}