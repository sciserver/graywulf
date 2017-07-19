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

        private IImportTableArchive GetImportTableArchiveTask(string path, string tableNamePattern, bool remote, bool generateIdentityColumn)
        {
            var ds = IOTestDataset;
            ds.IsMutable = true;

            var destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = ds.DefaultSchemaName,
                TableNamePattern = tableNamePattern,
            };

            IImportTableArchive it = null;
            if (remote)
            {
                it = RemoteServiceHelper.CreateObject<IImportTableArchive>(Test.Constants.Localhost, false);
            }
            else
            {
                it = new ImportTableArchive();
            }

            it.BatchName = Path.GetFileNameWithoutExtension(path);
            it.Uri = Util.UriConverter.FromFilePath(path);
            it.Destination = destination;
            it.Options = new ImportTableOptions()
            {
                GenerateIdentityColumn = generateIdentityColumn
            };

            return it;
        }

        private void ExecuteImportTableArchiveTest(IImportTableArchive it)
        {
            it.Open();
            it.Execute();
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

        private void ImportTableArchiveTestHelper(ArchiveType type, bool remote, bool generateIdentity)
        {
            ServiceTesterToken token = null;
            string path, tableNamePattern;
            Schema.Table table;
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
                    path = GetTestFilePath(@"graywulf\test\files\csv_numbers.zip");
                    tableNamePattern = GetTestUniqueName();
                    columnCount = new[] { 5 };
                    break;
                case ArchiveType.MultipleFiles:
                    path = GetTestFilePath(@"graywulf\test\files\archive.zip");
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

            var it = GetImportTableArchiveTask(path, tableNamePattern, remote, generateIdentity);
            ExecuteImportTableArchiveTest(it);

            for (int i = 0; i < it.Results.Count(); i++)
            {
                table = IOTestDataset.Tables[null, it.Results[i].SchemaName, it.Results[i].TableName];
                Assert.AreEqual(columnCount[i], table.Columns.Count);
            }
            
            if (remote)
            {
                token.Dispose();
            }
        }

        [TestMethod]
        public void ImportSingleFileTest()
        {
            ImportTableArchiveTestHelper(ArchiveType.SingleFile, false, false);
        }

        [TestMethod]
        public void ImportSingleFileGenerateIdentityTest()
        {
            ImportTableArchiveTestHelper(ArchiveType.SingleFile, false, true);
        }

        [TestMethod]
        public void RemoteImportSingleFileTest()
        {
            ImportTableArchiveTestHelper(ArchiveType.SingleFile, true, false);
        }

        [TestMethod]
        public void RemoteImportSingleFileGenerateIdentityTest()
        {
            ImportTableArchiveTestHelper(ArchiveType.SingleFile, true, true);
        }

        [TestMethod]
        public void ImportArchiveTest()
        {
            ImportTableArchiveTestHelper(ArchiveType.MultipleFiles, false, false);
        }

        [TestMethod]
        public void ImportArchiveGenerateIdentityTest()
        {
            ImportTableArchiveTestHelper(ArchiveType.MultipleFiles, false, true);
        }

        [TestMethod]
        public void RemoteImportArchiveTest()
        {
            ImportTableArchiveTestHelper(ArchiveType.MultipleFiles, true, false);
        }

        [TestMethod]
        public void RemoteImportArchiveGenerateIdentityTest()
        {
            ImportTableArchiveTestHelper(ArchiveType.MultipleFiles, true, true);
        }
    }
}
