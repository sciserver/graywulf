using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class QueryImporterTest : TestClassBase
    {
        private IQueryImporter GetQueryImporter(string tableName, bool remote)
        {
            var d = new DestinationTableParameters();
            d.Table = new Jhu.Graywulf.Schema.Table()
            {
                Dataset = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset("", Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString),
                SchemaName = "dbo",
                TableName = tableName
            };
            d.Operation = DestinationTableOperation.Create;
            

            var s = new SourceQueryParameters();
            s.Dataset = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);
            s.Query = "SELECT 1 AS one, 2 AS two, 3 AS three";

            IQueryImporter q = null;
            if (remote)
            {
                q = RemoteServiceHelper.CreateObject<IQueryImporter>(Test.Constants.Localhost);
            }
            else
            {
                q = new QueryImporter();
            }

            q.Source = s;
            q.Destination = d;

            return q;
        }

        [TestMethod]
        public void CreateTableTest()
        {
            var q = GetQueryImporter("QueryImporterTest_CreateTableTest", false);

            q.CreateDestinationTable();

            DropTable(Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString, q.Destination.Table.SchemaName, q.Destination.Table.TableName);
        }

        [TestMethod]
        public void ImportTableTest()
        {
            var q = GetQueryImporter("QueryImporterTest_ImportTableTest", false);

            q.Execute();

            DropTable(Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString, q.Destination.Table.SchemaName, q.Destination.Table.TableName);
        }

        [TestMethod]
        public void RemoteCreateTableTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var q = GetQueryImporter("QueryImporterTest_RemoteCreateTableTest", true);

                q.CreateDestinationTable();

                DropTable(Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString, q.Destination.Table.SchemaName, q.Destination.Table.TableName);
            }
        }

        [TestMethod]
        public void RemoteImportTableTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var q = GetQueryImporter("QueryImporterTest_RemoteImportTableTest", true);

                q.Execute();

                DropTable(Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString, q.Destination.Table.SchemaName, q.Destination.Table.TableName);
            }
        }
    }
}
