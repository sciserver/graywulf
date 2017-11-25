using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class CopyTableTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            StopLogger();
        }

        private ICopyTable GetTableCopy(CancellationContext cancellationContext, string tableName, bool remote)
        {
            ICopyTable q = null;
            if (remote)
            {
                q = RemoteServiceHelper.CreateObject<ICopyTable>(cancellationContext, Test.Constants.Localhost, false);
            }
            else
            {
                q = new CopyTable(cancellationContext);
            }

            var ds = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString)
            {
                IsMutable = true
            };

            var source = new SourceTableQuery()
            {
                Dataset = ds,
                Query = "SELECT * FROM SampleData_PrimaryKey"
            };

            q.Source = source;

          
            var destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = Schema.SqlServer.Constants.DefaultSchemaName,
                TableNamePattern = tableName,
                Options = TableInitializationOptions.Create
            };

            q.Destination = destination;

            return q;
        }

        [TestMethod]
        public void ImportTableTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var table = GetTestUniqueName();
                var q = GetTableCopy(cancellationContext, table, false);

                DropTable(q.Destination.GetTable());

                q.ExecuteAsync().Wait();

                DropTable(q.Destination.GetTable());
            }
        }

        [TestMethod]
        public void RemoteTableTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var table = GetTestUniqueName();
                    var q = GetTableCopy(cancellationContext, table, true);

                    DropTable(q.Destination.GetTable());

                    q.ExecuteAsync().Wait();

                    DropTable(q.Destination.GetTable());
                }
            }
        }

    }
}
