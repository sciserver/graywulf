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
    public class ExportTableTest : TestClassBase
    {
        protected IExportTable CreateTableExportTask(string path, bool remote)
        {
            return CreateTableExportTask(path, "SELECT * FROM SampleData", remote);
        }

        protected IExportTable CreateTableExportTask(string path, string query, bool remote)
        {
            var source = new SourceTableQuery()
            {
                Dataset = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString),
                Query = query
            };
            
            var destination = new DelimitedTextDataFile()
            {
                Uri = Util.UriConverter.FromFilePath(path)
            };

            IExportTable te = null;
            if (remote)
            {
                te = RemoteServiceHelper.CreateObject<IExportTable>(Test.Constants.Localhost, false);
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
            var path = GetTestUniqueName() + ".csv";
            var dfe = CreateTableExportTask(path, false);

            dfe.Execute();

            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void ExportEmptyTableTest()
        {
            var path = GetTestUniqueName() + ".csv";
            var query = "SELECT * FROM EmptyTable";
            var dfe = CreateTableExportTask(path, query, false);

            dfe.Execute();

            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void ExportToUncTest()
        {
            var path = String.Format(@"\\{0}\{1}\{2}.csv", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, GetTestUniqueName());
            var dfe = CreateTableExportTask(path, false);

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
                var dfe = CreateTableExportTask(path, false);

                dfe.Execute();

                Assert.IsTrue(File.Exists(path));
                File.Delete(path);
            }
        }
    }
}
