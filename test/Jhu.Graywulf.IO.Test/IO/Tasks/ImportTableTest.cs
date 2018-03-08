using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public sealed class ImportTableTest : ImportTableTestBase
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
                    var t = await ExecuteImportTableTaskAsync(it.Value, source, destination, settings);
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
                    var t = await ExecuteImportTableTaskAsync(it.Value, source, destination, settings);
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
                        var t = await ExecuteImportTableTaskAsync(it.Value, source, destination, settings);

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
                    var t = await ExecuteImportTableTaskAsync(it.Value, source, destination, settings);

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
