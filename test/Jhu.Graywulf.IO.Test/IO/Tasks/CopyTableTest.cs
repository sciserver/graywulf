using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Sql.Schema;
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

        private ServiceModel.ServiceProxy<ICopyTable> GetTableCopy(CancellationContext cancellationContext, string tableName, bool remote)
        {
            ServiceModel.ServiceProxy<ICopyTable> q = null;
            if (remote)
            {
                q = RemoteServiceHelper.CreateObject<ICopyTable>(cancellationContext, Test.Constants.Localhost, false);
            }
            else
            {
                q = new ServiceModel.ServiceProxy<ICopyTable>(new CopyTable(cancellationContext));
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

            q.Value.Source = source;

          
            var destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = Schema.SqlServer.Constants.DefaultSchemaName,
                TableNamePattern = tableName,
                Options = TableInitializationOptions.Create
            };

            q.Value.Destination = destination;

            return q;
        }

        [TestMethod]
        public void ImportTableTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var table = GetTestUniqueName();

                using (var q = GetTableCopy(cancellationContext, table, false))
                {
                    DropTable(q.Value.Destination.GetTable());

                    q.Value.ExecuteAsync().Wait();

                    DropTable(q.Value.Destination.GetTable());
                }
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
                    using (var q = GetTableCopy(cancellationContext, table, true))
                    {
                        DropTable(q.Value.Destination.GetTable());

                        q.Value.ExecuteAsync().Wait();

                        DropTable(q.Value.Destination.GetTable());
                    }
                }
            }
        }

    }
}
