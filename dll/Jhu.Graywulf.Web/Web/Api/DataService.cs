using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
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
        [WebGet(UriTemplate = "MYDB/tables")]
        Table[] ListTables();

        [OperationContract]
        [WebGet(UriTemplate = "MYDB/tables/{tableName}")]
        Stream GetTable(string tableName);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
    [RestServiceBehavior]
    public class DataService : ServiceBase, IDataService
    {
        public Table[] ListTables()
        {
            var mydb = FederationContext.MyDBDataset;

            mydb.Tables.LoadAll();

            var tables = new Table[mydb.Tables.Count];

            int q = 0;
            foreach (var t in mydb.Tables.Values)
            {
                tables[q] = new Table()
                {
                    Name = t.DisplayName,
                };
                q++;
            }

            return tables;
        }

        public Stream GetTable(string tableName)
        {
            // Get table from the schema
            var parts = tableName.Split('.');

            var mydb = FederationContext.MyDBDataset;
            var table = mydb.Tables[mydb.DatabaseName, parts[0], parts[1]];

            // Build a query to export everything
            var codegen = SqlCodeGeneratorFactory.CreateCodeGenerator(table.Dataset);
            var sql = codegen.GenerateSelectStarQuery(table, 100);

            // Create source
            var source = new SourceTableQuery()
            {
                Dataset = mydb,
                Query = sql
            };

            // Create destination
            var stream = new MemoryStream();
            var destination = new DelimitedTextDataFile(stream, DataFileMode.Write);

            var export = new ExportTable()
            {
                Source = source,
                Destination = destination
            };

            HttpContext.Response.AddHeader("Content-Type", "application/octet-stream");

            export.Execute();

            return null;
        }
    }
}