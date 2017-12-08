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

        private ServiceModel.ServiceProxy<IExportTableArchive> GetTableExportTask(CancellationContext cancellationContext, Uri uri, string path, bool remote)
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

            ServiceModel.ServiceProxy<IExportTableArchive> te = null;
            if (remote)
            {
                te = RemoteServiceHelper.CreateObject<IExportTableArchive>(cancellationContext, Test.Constants.Localhost, allowInProc);
            }
            else
            {
                te = new ServiceModel.ServiceProxy<IExportTableArchive>(new ExportTableArchive(cancellationContext));
            }

            te.Value.Sources = new[] { source };
            te.Value.Destinations = new[] { destination };
            te.Value.Uri = uri;

            return te;
        }

        [TestMethod]
        public void ExportZipTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var zippath = "TableExportArchiveTest_ExportZipTest.zip";
                var path = "test.csv";

                using (var task = GetTableExportTask(cancellationContext, Util.UriConverter.FromFilePath(zippath), path, false))
                {
                    Util.TaskHelper.Wait(task.Value.OpenAsync());
                    Util.TaskHelper.Wait(task.Value.ExecuteAsync());
                    task.Value.Close();

                    Assert.IsTrue(File.Exists(zippath));
                    File.Delete(zippath);
                }
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

                    using (var task = GetTableExportTask(cancellationContext, Util.UriConverter.FromFilePath(zippath), path, true))
                    {
                        Util.TaskHelper.Wait(task.Value.OpenAsync());
                        Util.TaskHelper.Wait(task.Value.ExecuteAsync());

                        task.Value.Close();

                        Assert.IsTrue(File.Exists(zippath));
                        File.Delete(zippath);
                    }
                }
            }
        }
    }
}
