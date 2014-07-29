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
        Dataset[] ListDatasets();

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/{datasetName}/tables")]
        TableList ListTables(string datasetName);

        [OperationContract]
        [WebGet(UriTemplate = "/{datasetName}/tables/{tableName}?top={top}")]
        Stream GetTable(string datasetName, string tableName, string top);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
    [RestServiceBehavior]
    public class DataService : ServiceBase, IDataService
    {
        public Dataset[] ListDatasets()
        {
            return new[]
                {
                new Dataset()
            {
                Name = "test"
            }};
        }

        public TableList ListTables(string datasetName)
        {
            var dataset = FederationContext.SchemaManager.Datasets[datasetName];
            dataset.Tables.LoadAll();

            var res = new TableList()
            {
                Tables = new Table[dataset.Tables.Count]
            };

            int q = 0;
            foreach (var t in dataset.Tables.Values)
            {
                res.Tables[q] = new Table()
                {
                    Name = t.DisplayName,
                };
                q++;
            }

            return res;
        }

        public Stream GetTable(string datasetName, string tableName, string top)
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
            var accept = WebOperationContext.Current.IncomingRequest.Accept ?? WebOperationContext.Current.IncomingRequest.ContentType;
            var ff = FileFormatFactory.Create(FederationContext.Federation.FileFormatFactory);
            var destination = ff.CreateFile(ff.GetFileFromFromAcceptHeader(accept));

            var export = new ExportTable()
            {
                Source = source,
                Destination = destination
            };

            // Set content type based on output file format
            WebOperationContext.Current.OutgoingResponse.ContentType = destination.Description.DefaultMimeType;

            // Stream out results
            return new AdapterStream(stream =>
                {
                    destination.Open(stream, DataFileMode.Write);
                    export.Execute();
                });
        }
    }
}