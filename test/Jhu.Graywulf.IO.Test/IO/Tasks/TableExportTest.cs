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
    public class TableExportTest : TestClassBase
    {
        private IExportTable GetTableExportTask(Uri path, bool remote)
        {
            var source = new SourceTableQuery()
            {
                Dataset = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString),
                Query = "SELECT * FROM SampleData"
            };
            
            var destination = new DelimitedTextDataFile()
            {
                Uri = new Uri("test.txt", UriKind.Relative)
            };

            IExportTable te = null;
            if (remote)
            {
                te = RemoteServiceHelper.CreateObject<IExportTable>(Test.Constants.Localhost);
            }
            else
            {
                te = new ExportTable();
            }

            te.Sources = new SourceTableQuery[] { source };
            te.Destinations = new DataFileBase[] { destination };
            te.Uri = path;

            return te;
        }

        [TestMethod]
        public void ExportZipTest()
        {
            var path = "DataFileExporterTest_ExportZipTest.zip";
            var dfe = GetTableExportTask(Util.UriConverter.FromFilePath(path), false);

            dfe.Execute();

            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void ExportToDirectoryTest()
        {
            var path = "DataFileExporterTest_ExportToDirectoryTest";
            var dfe = GetTableExportTask(Util.UriConverter.FromFilePath(path), false);
            dfe.Archival = DataFileArchival.None;

            dfe.Execute();

            Assert.IsTrue(Directory.Exists(path));
            Directory.Delete(path, true);
        }

        [TestMethod]
        public void ExportToRelativeTest()
        {
            var path = "DataFileExporterTest_ExportToRelativeTest.zip";
            var dfe = GetTableExportTask(Util.UriConverter.FromFilePath(path), false);

            dfe.Execute();

            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void ExportToAbsoluteTest()
        {
            var path = @"C:\DataFileExporterTest_ExportToAbsoluteTest.zip";
            var dfe = GetTableExportTask(Util.UriConverter.FromFilePath(path), false);

            dfe.Execute();

            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void ExportToUncTest()
        {
            var path = String.Format(@"\\{0}\{1}\{2}.zip", Test.Constants.RemoteHost1, Test.Constants.GWCode, "DataFileExporterTest_ExportToUncTest");
            var dfe = GetTableExportTask(Util.UriConverter.FromFilePath(path), false);

            dfe.Execute();

            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void RemoteExportTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var path = String.Format(@"\\{0}\{1}\{2}.zip", Test.Constants.RemoteHost1, Test.Constants.GWCode, "DataFileExporterTest_RemoteExportTest");
                var dfe = GetTableExportTask(Util.UriConverter.FromFilePath(path), false);

                dfe.Execute();

                var d = dfe.Destinations[0];
                var p = dfe.Destinations[0].Uri;

                Assert.IsTrue(File.Exists(path));
                File.Delete(path);
            }
        }
    }
}
