using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class ImportTableArchiveTest : TestClassBase
    {
        enum ArchiveType
        {
            SingleFile,
            MultipleFiles,
        }

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

        private ServiceModel.ServiceProxy<IImportTableArchive> GetImportTableArchiveTask(
            CancellationContext cancellationContext, string path, string tableNamePattern, bool remote, bool generateIdentityColumn,
            out DataFileBase[] sources, out DestinationTable[] destinations,
            out TableCopySettings settings, out TableArchiveSettings archiveSettings)
        {
            var ds = IOTestDataset;
            ds.IsMutable = true;

            sources = null;
            destinations = new DestinationTable[]
            {
                new DestinationTable()
                {
                    Dataset = ds,
                    DatabaseName = ds.DatabaseName,
                    SchemaName = ds.DefaultSchemaName,
                    TableNamePattern = tableNamePattern,
                }
            };

            settings = new TableCopySettings()
            {
                BatchName = Path.GetFileNameWithoutExtension(path)
            };

            archiveSettings = new TableArchiveSettings()
            {
                Uri = Util.UriConverter.FromFilePath(path),
                GenerateIdentityColumn = generateIdentityColumn
            };
            
            ServiceModel.ServiceProxy<IImportTableArchive> it = null;
            if (remote)
            {
                it = RemoteServiceHelper.CreateObject<IImportTableArchive>(cancellationContext, Test.Constants.Localhost, false);
            }
            else
            {
                it = new ServiceModel.ServiceProxy<IImportTableArchive>(new ImportTableArchive(cancellationContext));
            }
            
            return it;
        }

        private void DropTestTables()
        {
            var name = GetTestClassName();
            var ds = IOTestDataset;
            ds.IsMutable = true;

            ds.Tables.LoadAll(true);
            foreach (var t in ds.Tables.Values)
            {
                if (t.ObjectName.StartsWith(name))
                {
                    t.Drop();
                }
            }
        }

        private async Task ImportTableArchiveTestHelper(CancellationContext cancellationContext, ArchiveType type, bool remote, bool generateIdentity)
        {
            ServiceTesterToken token = null;
            string path, tableNamePattern;
            Sql.Schema.Table table;
            int[] columnCount;

            DropTestTables();

            if (remote)
            {
                token = RemoteServiceTester.Instance.GetToken();
                RemoteServiceTester.Instance.EnsureRunning();
            }

            switch (type)
            {
                case ArchiveType.SingleFile:
                    path = GetTestFilePath(@"modules\graywulf\test\files\csv_numbers.zip");
                    tableNamePattern = GetTestUniqueName();
                    columnCount = new[] { 5 };
                    break;
                case ArchiveType.MultipleFiles:
                    path = GetTestFilePath(@"modules\graywulf\test\files\archive.zip");
                    tableNamePattern = GetTestUniqueName() + "_[$ResultsetName]";
                    columnCount = new[] { 5, 2 };
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (generateIdentity)
            {
                for (int i = 0; i < columnCount.Length; i++)
                {
                    columnCount[i]++;
                }
            }

            using (var it = GetImportTableArchiveTask(
                cancellationContext, path, tableNamePattern, remote, generateIdentity,
                out var sources, out var destinations, out var settings, out var archiveSettings))
            {
                var results = await it.Value.ExecuteAsyncEx(
                    sources, destinations, settings, archiveSettings);

                for (int i = 0; i < results.Count(); i++)
                {
                    table = IOTestDataset.Tables[results[i].DestinationTable];
                    Assert.AreEqual(columnCount[i], table.Columns.Count);
                }
            }

            if (remote)
            {
                token.Dispose();
            }
        }

        [TestMethod]
        public void ImportSingleFileTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                ImportTableArchiveTestHelper(cancellationContext, ArchiveType.SingleFile, false, false);
            }
        }

        [TestMethod]
        public void ImportSingleFileGenerateIdentityTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                ImportTableArchiveTestHelper(cancellationContext, ArchiveType.SingleFile, false, true);
            }
        }

        [TestMethod]
        public void RemoteImportSingleFileTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                ImportTableArchiveTestHelper(cancellationContext, ArchiveType.SingleFile, true, false);
            }
        }

        [TestMethod]
        public void RemoteImportSingleFileGenerateIdentityTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                ImportTableArchiveTestHelper(cancellationContext, ArchiveType.SingleFile, true, true);
            }
        }

        [TestMethod]
        public void ImportArchiveTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                ImportTableArchiveTestHelper(cancellationContext, ArchiveType.MultipleFiles, false, false);
            }
        }

        [TestMethod]
        public void ImportArchiveGenerateIdentityTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                ImportTableArchiveTestHelper(cancellationContext, ArchiveType.MultipleFiles, false, true);
            }
        }

        [TestMethod]
        public void RemoteImportArchiveTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                ImportTableArchiveTestHelper(cancellationContext, ArchiveType.MultipleFiles, true, false);
            }
        }

        [TestMethod]
        public void RemoteImportArchiveGenerateIdentityTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                ImportTableArchiveTestHelper(cancellationContext, ArchiveType.MultipleFiles, true, true);
            }
        }
    }
}
