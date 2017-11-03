using System;
using System.Threading;
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

        private IExportTableArchive GetTableExportTask(CancellationContext cancellationContext, Uri uri, string path, bool remote)
        {
            var source = new SourceTableQuery()
            {
                Dataset = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString),
                Query = "SELECT * FROM SampleData"
            };

            var destination = new DelimitedTextDataFile()
            {
                Uri = Util.UriConverter.FromFilePath(path)
            };

            IExportTableArchive te = null;
            if (remote)
            {
                te = RemoteServiceHelper.CreateObject<IExportTableArchive>(cancellationContext, Test.Constants.Localhost, allowInProc);
            }
            else
            {
                te = new ExportTableArchive(cancellationContext);
            }

            te.Sources = new[] { source };
            te.Destinations = new[] { destination };
            te.Uri = uri;

            return te;
        }

        [TestMethod]
        public void ExportZipTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var zippath = "TableExportArchiveTest_ExportZipTest.zip";
                var path = "test.csv";
                var task = GetTableExportTask(cancellationContext, Util.UriConverter.FromFilePath(zippath), path, false);

                task.Open();
                task.ExecuteAsync().Wait();
                task.Close();

                Assert.IsTrue(File.Exists(zippath));
                File.Delete(zippath);
            }
        }
        
        [TestMethod]
        public void RemoteExportZipTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var zippath = String.Format(@"\\{0}\{1}\files\{2}.zip", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, "TableExportArchiveTest_RemoteExportZipTest");
                    var path = "test.csv";
                    var task = GetTableExportTask(cancellationContext, Util.UriConverter.FromFilePath(zippath), path, true);

                    task.Open();
                    task.ExecuteAsync().Wait();
                    task.Close();

                    Assert.IsTrue(File.Exists(zippath));
                    File.Delete(zippath);
                }
            }
        }
    }
}
