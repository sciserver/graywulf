using System;
using System.Threading.Tasks;
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

        protected ServiceModel.ServiceProxy<IImportTable> GetImportTableTask(
            CancellationContext cancellationContext, string path, bool remote, bool generateIdentityColumn,
            out DataFileBase source, out DestinationTable destination, out TableCopySettings settings)
        {
            var ds = IOTestDataset;
            ds.IsMutable = true;

            var table = GetTestUniqueName();
            var ff = CreateFileFormatFactory();
            source = ff.CreateFile(new Uri(path, UriKind.RelativeOrAbsolute));
            source.GenerateIdentityColumn = generateIdentityColumn;
            destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = ds.DefaultSchemaName,
                TableNamePattern = table,
            };
            settings = new TableCopySettings()
            {
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
            
            return it;
        }

        protected async Task<Jhu.Graywulf.Sql.Schema.Table> ExecuteImportTableTask(IImportTable it, DataFileBase source, DestinationTable destination, TableCopySettings settings)
        {
            var t = destination.GetTable();
            DropTable(t);

            await it.ExecuteAsyncEx(source, destination, settings);

            var table = destination.GetTable();

            return table;
        }

        [TestMethod]
        public async Task ImportTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestFilePath(@"modules\graywulf\test\files\csv_numbers.csv");

                using (var it = GetImportTableTask(
                    cancellationContext, path, false, false,
                    out var source, out var destination, out var settings))
                {
                    var t = await ExecuteImportTableTask(it.Value, source, destination, settings);
                    Assert.AreEqual(5, t.Columns.Count);
                    DropTable(t);
                }
            }
        }

        [TestMethod]
        public async Task ImportGenerateIdentityTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestFilePath(@"modules\graywulf\test\files\csv_numbers.csv");

                using (var it = GetImportTableTask(
                    cancellationContext, path, false, false,
                    out var source, out var destination, out var settings))
                {
                    var t = await ExecuteImportTableTask(it.Value, source, destination, settings);
                    Assert.AreEqual(5, t.Columns.Count);
                    DropTable(t);
                }
            }
        }

        [TestMethod]
        public async Task RemoteImportTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var path = GetTestFilePath(@"modules\graywulf\test\files\csv_numbers.csv");

                    using (var it = GetImportTableTask(
                        cancellationContext, path, false, false,
                        out var source, out var destination, out var settings))
                    {
                        var t = await ExecuteImportTableTask(it.Value, source, destination, settings);

                        Assert.AreEqual(5, t.Columns.Count);
                        DropTable(t);
                    }
                }
            }
        }

        public async Task ImportFromHttpTestHelper(string url)
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = url;

                using (var it = GetImportTableTask(
                    cancellationContext, path, false, false,
                    out var source, out var destination, out var settings))
                {
                    var t = await ExecuteImportTableTask(it.Value, source, destination, settings);

                    Assert.AreEqual(5, t.Columns.Count);
                    DropTable(t);
                }
            }
        }

        [TestMethod]
        public async Task ImportFromHttpTest()
        {
            await ImportFromHttpTestHelper(@"http://localhost/graywulf_io_test/csv_numbers.csv");
        }

        [TestMethod]
        public async Task ImportFromHttpTest2()
        {
            await ImportFromHttpTestHelper(@"http://localhost/~graywulf_io_test/csv_numbers.csv");
        }

        [TestMethod]
        public async Task ImportFromHttpTest3()
        {
            await ImportFromHttpTestHelper(@"http://localhost/graywulf-io-test/csv_numbers.csv");
        }
    }
}
