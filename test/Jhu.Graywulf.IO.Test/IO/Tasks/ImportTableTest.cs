using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class ImportTableTest : TestClassBase
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

        protected virtual FileFormatFactory CreateFileFormatFactory()
        {
            return FileFormatFactory.Create(null);
        }

        protected ServiceModel.ServiceProxy<IImportTable> GetImportTableTask(CancellationContext cancellationContext, string path, bool remote, bool generateIdentityColumn)
        {
            var ds = IOTestDataset;
            ds.IsMutable = true;

            var table = GetTestUniqueName();
            var ff = CreateFileFormatFactory();
            var source = ff.CreateFile(new Uri(path, UriKind.RelativeOrAbsolute));
            var destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = ds.DefaultSchemaName,
                TableNamePattern = table,
            };

            ServiceModel.ServiceProxy<IImportTable> it = null;
            if (remote)
            {
                it = RemoteServiceHelper.CreateObject<IImportTable>(cancellationContext, Test.Constants.Localhost, false);
            }
            else
            {
                it = new ServiceModel.ServiceProxy<IImportTable>(new ImportTable(cancellationContext));
            }

            it.Value.Source = source;
            it.Value.Destination = destination;
            it.Value.Options = new ImportTableOptions()
            {
                GenerateIdentityColumn = generateIdentityColumn
            };

            return it;
        }

        protected Jhu.Graywulf.Schema.Table ExecuteImportTableTask(IImportTable it)
        {
            var t = it.Destination.GetTable();
            DropTable(t);

            Util.TaskHelper.Wait(it.ExecuteAsync());

            var table = it.Destination.GetTable();

            return table;
        }

        [TestMethod]
        public void ImportTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestFilePath(@"modules\graywulf\test\files\csv_numbers.csv");

                using (var it = GetImportTableTask(cancellationContext, path, false, false))
                {
                    var t = ExecuteImportTableTask(it.Value);
                    Assert.AreEqual(5, t.Columns.Count);
                    DropTable(t);
                }
            }
        }

        [TestMethod]
        public void ImportGenerateIdentityTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestFilePath(@"modules\graywulf\test\files\csv_numbers.csv");

                using (var it = GetImportTableTask(cancellationContext, path, false, false))
                {
                    var t = ExecuteImportTableTask(it.Value);

                    Assert.AreEqual(5, t.Columns.Count);
                    DropTable(t);
                }
            }
        }

        [TestMethod]
        public void RemoteImportTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var path = GetTestFilePath(@"modules\graywulf\test\files\csv_numbers.csv");

                    using (var it = GetImportTableTask(cancellationContext, path, false, false))
                    {
                        var t = ExecuteImportTableTask(it.Value);

                        Assert.AreEqual(5, t.Columns.Count);
                        DropTable(t);
                    }
                }
            }
        }

        [TestMethod]
        public void ImportFromHttpTestHelper(string url)
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = url;

                using (var it = GetImportTableTask(cancellationContext, path, false, false))
                {
                    var t = ExecuteImportTableTask(it.Value);

                    Assert.AreEqual(5, t.Columns.Count);
                    DropTable(t);
                }
            }
        }

        [TestMethod]
        public void ImportFromHttpTest()
        {
            ImportFromHttpTestHelper(@"http://localhost/graywulf_io_test/csv_numbers.csv");
        }

        [TestMethod]
        public void ImportFromHttpTest2()
        {
            ImportFromHttpTestHelper(@"http://localhost/~graywulf_io_test/csv_numbers.csv");
        }

        [TestMethod]
        public void ImportFromHttpTest3()
        {
            ImportFromHttpTestHelper(@"http://localhost/graywulf-io-test/csv_numbers.csv");
        }
    }
}
