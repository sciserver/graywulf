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
    public class ExportTableArchiveTest : TestClassBase
    {
        // TODO: change this if same-machine remoting works
        private const bool allowInProc = false;

        private ServiceModel.ServiceProxy<IExportTableArchive> GetTableExportTask(
            CancellationContext cancellationContext, Uri uri, string path, bool remote,
            out SourceQuery[] sources, out DataFileBase[] destinations, 
            out TableCopySettings settings, out TableArchiveSettings archiveSettings)
        {
            var source = new SourceQuery()
            {
                Dataset = new Jhu.Graywulf.Sql.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString),
                Query = "SELECT * FROM SampleData"
            };

            var destination = new DelimitedTextDataFile()
            {
                Uri = Util.UriConverter.FromFilePath(path)
            };

            ServiceModel.ServiceProxy<IExportTableArchive> proxy;
            if (remote)
            {
                proxy = RemoteServiceHelper.CreateObject<IExportTableArchive>(cancellationContext, Test.Constants.Localhost, allowInProc);
            }
            else
            {
                proxy = new ServiceModel.ServiceProxy<IExportTableArchive>(new ExportTableArchive(cancellationContext));
            }

            sources = new[] { source };
            destinations = new[] { destination };
            settings = new TableCopySettings();
            archiveSettings = new TableArchiveSettings()
            {
                Uri = uri
            };
            return proxy;
        }

        [TestMethod]
        public async Task ExportZipTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var zippath = GetTestUniqueName() + ".zip";
                var path = "test.csv";

                using (var task = GetTableExportTask(
                    cancellationContext, 
                    Util.UriConverter.FromFilePath(zippath), path, false,
                    out var sources, out var destinations,
                    out var settings, out var archiveSettings))
                {
                    await task.Value.ExecuteAsyncEx(sources, destinations, settings, archiveSettings);
                }

                Assert.IsTrue(File.Exists(zippath));
                File.Delete(zippath);
            }
        }
        
        [TestMethod]
        public async Task RemoteExportZipTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var zippath = String.Format(@"\\{0}\{1}\files\{2}.zip", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, "TableExportArchiveTest_RemoteExportZipTest");
                    var path = "test.csv";

                    using (var task = GetTableExportTask(
                        cancellationContext, 
                        Util.UriConverter.FromFilePath(zippath), path, true,
                        out var sources, out var destinations,
                    out var settings, out var archiveSettings))
                    {
                        await task.Value.ExecuteAsyncEx(sources, destinations, settings, archiveSettings);
                    }

                    Assert.IsTrue(File.Exists(zippath));
                    File.Delete(zippath);
                }
            }
        }

        // TODO: add multi-source and multi-resultset tests
    }
}
