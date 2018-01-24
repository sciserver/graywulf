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
    public class ExportTableTest : TestClassBase
    {
        protected ServiceModel.ServiceProxy<IExportTable> CreateTableExportTask(CancellationContext cancellationContext, string path, bool remote)
        {
            return CreateTableExportTask(cancellationContext, path, "SELECT * FROM SampleData", remote);
        }

        protected ServiceModel.ServiceProxy<IExportTable> CreateTableExportTask(CancellationContext cancellationContext, string path, string query, bool remote)
        {
            var source = new SourceTableQuery()
            {
                Dataset = new Jhu.Graywulf.Sql.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString),
                Query = query
            };
            
            var destination = new DelimitedTextDataFile()
            {
                Uri = Util.UriConverter.FromFilePath(path)
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

            te.Value.Source = source;
            te.Value.Destination = destination;

            return te;
        }

        [TestMethod]
        public void ExportTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestUniqueName() + ".csv";

                using (var dfe = CreateTableExportTask(cancellationContext, path, false))
                {
                    dfe.Value.ExecuteAsync().Wait();

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public void ExportEmptyTableTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = GetTestUniqueName() + ".csv";
                var query = "SELECT * FROM EmptyTable";

                using (var dfe = CreateTableExportTask(cancellationContext, path, query, false))
                {
                    dfe.Value.ExecuteAsync().Wait();

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public void ExportToUncTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var path = String.Format(@"\\{0}\{1}\{2}.csv", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, GetTestUniqueName());

                using (var dfe = CreateTableExportTask(cancellationContext, path, false))
                {
                    dfe.Value.ExecuteAsync().Wait();

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public void RemoteExportTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var path = String.Format(@"\\{0}\{1}\{2}.csv", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, "TableExportTest_RemoteExportTest");

                    using (var dfe = CreateTableExportTask(cancellationContext, path, false))
                    {
                        dfe.Value.ExecuteAsync().Wait();

                        Assert.IsTrue(File.Exists(path));
                        File.Delete(path);
                    }
                }
            }
        }
    }
}
