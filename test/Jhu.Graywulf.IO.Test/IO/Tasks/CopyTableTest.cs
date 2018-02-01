using System;
using System.Threading.Tasks;
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
            return GetTableCopy(cancellationContext, null, tableName, remote);
        }

        private ServiceModel.ServiceProxy<ICopyTable> GetTableCopy(CancellationContext cancellationContext, string sql, string tableName, bool remote)
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

            var ds = new Jhu.Graywulf.Sql.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString)
            {
                IsMutable = true
            };

            var source = new SourceQuery()
            {
                Dataset = ds,
                Query = sql ?? "SELECT * FROM SampleData_PrimaryKey"
            };

            q.Value.Source = source;

            var destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = Sql.Schema.SqlServer.Constants.DefaultSchemaName,
                TableNamePattern = tableName,
                Options = TableInitializationOptions.Create
            };

            q.Value.Destination = destination;

            return q;
        }

        [TestMethod]
        public void SimpleTableCopyTest()
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
        public void MultipleTableCopyTest()
        {
            var sql =
@"SELECT * FROM SampleData_PrimaryKey
SELECT * FROM SampleData_PrimaryKey";

            using (var cancellationContext = new CancellationContext())
            {
                var table = GetTestUniqueName() + "_" + Constants.ResultsetNameToken;

                using (var q = GetTableCopy(cancellationContext, sql, table, false))
                {
                    DropTable(q.Value.Destination.GetTable(null, null, "0", null));
                    DropTable(q.Value.Destination.GetTable(null, null, "1", null));

                    q.Value.ExecuteAsync().Wait();

                    DropTable(q.Value.Destination.GetTable(null, null, "0", null));
                    DropTable(q.Value.Destination.GetTable(null, null, "1", null));
                }
            }
        }

        [TestMethod]
        public void RemoteSimpleTableCopyTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var table = GetTestUniqueName();
                    using (var q = GetTableCopy(cancellationContext, table, true))
                    {
                        var destination = q.Value.Destination;
                        DropTable(destination.GetTable());

                        q.Value.ExecuteAsync().Wait();

                        destination = q.Value.Destination;
                        DropTable(destination.GetTable());

                        var r = q.Value.Results;

                        Assert.AreEqual(TableCopyStatus.Success, r[0].Status);
                        Assert.AreEqual(2, r[0].RecordsAffected);
                        Assert.AreEqual("Table|TEST|Graywulf_IO_Test|dbo|CopyTableTest_RemoteSimpleTableCopyTest", r[0].DestinationTable);
                    }
                }
            }
        }

        [TestMethod]
        public void RemoteMultipleTableCopyTest()
        {
            var sql =
@"SELECT * FROM SampleData_PrimaryKey
SELECT * FROM SampleData_PrimaryKey";

            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var table = GetTestUniqueName() + "_" + Constants.ResultsetNameToken;

                    using (var q = GetTableCopy(cancellationContext, sql, table, true))
                    {
                        var destination = q.Value.Destination;

                        DropTable(destination.GetTable(null, null, "0", null));
                        DropTable(destination.GetTable(null, null, "1", null));

                        q.Value.ExecuteAsync().Wait();

                        destination = q.Value.Destination;

                        DropTable(destination.GetTable(null, null, "0", null));
                        DropTable(destination.GetTable(null, null, "1", null));

                        var r = q.Value.Results;

                        Assert.AreEqual(TableCopyStatus.Success, r[0].Status);
                        Assert.AreEqual(2, r[0].RecordsAffected);
                        Assert.AreEqual("Table|TEST|Graywulf_IO_Test|dbo|CopyTableTest_RemoteMultipleTableCopyTest_0", r[0].DestinationTable);

                        Assert.AreEqual(TableCopyStatus.Success, r[1].Status);
                        Assert.AreEqual(2, r[1].RecordsAffected);
                        Assert.AreEqual("Table|TEST|Graywulf_IO_Test|dbo|CopyTableTest_RemoteMultipleTableCopyTest_1", r[1].DestinationTable);
                    }
                }
            }
        }

    }
}
