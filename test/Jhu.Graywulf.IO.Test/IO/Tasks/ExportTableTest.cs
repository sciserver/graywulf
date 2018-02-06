using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class ExportTableTest : TestClassBase
    {
        protected ServiceModel.ServiceProxy<IExportTable> CreateTableExportTask(
            CancellationContext cancellationContext, string path, bool remote,
            out SourceQuery source, out DataFileBase destination, out TableCopySettings settings)
        {
            return CreateTableExportTask(
                cancellationContext, path, "SELECT * FROM SampleData", remote,
                out source, out destination, out settings);
        }

        protected ServiceModel.ServiceProxy<IExportTable> CreateTableExportTask(
            CancellationContext cancellationContext, string path, string query, bool remote,
            out SourceQuery source, out DataFileBase destination, out TableCopySettings settings)
        {
            source = new SourceQuery()
            {
                Dataset = new Jhu.Graywulf.Sql.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString),
                Query = query
            };

            var uri = Util.UriConverter.FromFilePath(path);
            var ff = FileFormatFactory.Create(null);
            destination = ff.CreateFile(uri, out string filename, out string extensions, out DataFileCompression compression);

            settings = new TableCopySettings()
            {
            };

            ServiceModel.ServiceProxy<IExportTable> te = null;
            if (remote)
            {
                te = RemoteServiceHelper.CreateObject<IExportTable>(cancellationContext, Test.Constants.Localhost, false);
            }
            else
            {
                te = new ServiceModel.ServiceProxy<IExportTable>(new ExportTable(cancellationContext));
            }

            return te;
        }

        [TestMethod]
        public async Task ExportTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestUniqueName() + ".csv";

                using (var dfe = CreateTableExportTask(
                    cancellationContext, path, false,
                    out var source, out var destination, out var settings))
                {
                    await dfe.Value.ExecuteAsyncEx(source, destination, settings);

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public async Task ExportEmptyTableTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestUniqueName() + ".csv";
                var query = "SELECT * FROM EmptyTable";

                using (var dfe = CreateTableExportTask(
                    cancellationContext, path, query, false,
                    out var source, out var destination, out var settings))
                {
                    await dfe.Value.ExecuteAsyncEx(source, destination, settings);

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public async Task ExportMultipleTablesTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestUniqueName() + ".html";
                var query =
@"SELECT * FROM SampleData
SELECT * FROM SampleData";

                using (var dfe = CreateTableExportTask(
                    cancellationContext, path, query, false,
                    out var source, out var destination, out var settings))
                {
                    await dfe.Value.ExecuteAsyncEx(source, destination, settings);

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public async Task ExportToUncTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = String.Format(@"\\{0}\{1}\{2}.csv", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, GetTestUniqueName());

                using (var dfe = CreateTableExportTask(
                    cancellationContext, path, false,
                    out var source, out var destination, out var settings))
                {
                    await dfe.Value.ExecuteAsyncEx(source, destination, settings);

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public async Task RemoteExportTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var path = String.Format(@"\\{0}\{1}\{2}.csv", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, "TableExportTest_RemoteExportTest");

                    using (var dfe = CreateTableExportTask(
                        cancellationContext, path, false,
                        out var source, out var destination, out var settings))
                    {
                        await dfe.Value.ExecuteAsyncEx(source, destination, settings);

                        Assert.IsTrue(File.Exists(path));
                        File.Delete(path);
                    }
                }
            }
        }
    }
}
