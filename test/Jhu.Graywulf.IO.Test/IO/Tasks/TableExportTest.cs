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
        private IExportTable GetTableExportTask(string path, bool remote)
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

            IExportTable te = null;
            if (remote)
            {
                te = RemoteServiceHelper.CreateObject<IExportTable>(Test.Constants.Localhost);
            }
            else
            {
                te = new ExportTable();
            }

            te.Source = source;
            te.Destination = destination;

            return te;
        }

        [TestMethod]
        public void ExportTest()
        {
            var path = "TableExportTest_ExportTest.csv";
            var dfe = GetTableExportTask(path, false);

            dfe.Execute();

            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void ExportToUncTest()
        {
            var path = String.Format(@"\\{0}\{1}\{2}.csv", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, "TableExportTest_ExportToUncTest");
            var dfe = GetTableExportTask(path, false);

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

                var path = String.Format(@"\\{0}\{1}\{2}.csv", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, "TableExportTest_RemoteExportTest");
                var dfe = GetTableExportTask(path, false);

                dfe.Execute();

                Assert.IsTrue(File.Exists(path));
                File.Delete(path);
            }
        }
    }
}
