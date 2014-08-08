using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class TableExportArchiveTest : TestClassBase
    {
        private IExportTableArchive GetTableExportTask(Uri uri, string path, bool remote)
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
                te = RemoteServiceHelper.CreateObject<IExportTableArchive>(Test.Constants.Localhost);
            }
            else
            {
                te = new ExportTableArchive();
            }

            te.Sources = new[] { source };
            te.Destinations = new[] { destination };
            te.Uri = uri;

            return te;
        }

        [TestMethod]
        public void ExportZipTest()
        {
            var zippath = "TableExportArchiveTest_ExportZipTest.zip";
            var path = "test.csv";
            var task = GetTableExportTask(Util.UriConverter.FromFilePath(zippath), path, false);

            task.Open();
            task.Execute();
            task.Close();

            Assert.IsTrue(File.Exists(zippath));
            File.Delete(zippath);
        }
        
        [TestMethod]
        public void RemoteExportZipTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var zippath = String.Format(@"\\{0}\{1}\{2}.zip", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, "TableExportArchiveTest_RemoteExportZipTest");
                var path = "test.csv";
                var task = GetTableExportTask(Util.UriConverter.FromFilePath(zippath), path, true);

                task.Open();
                task.Execute();
                task.Close();

                Assert.IsTrue(File.Exists(zippath));
                File.Delete(zippath);
            }
        }
    }
}
