using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.IO
{
    [TestClass]
    public class DataFileImporterTest : TestClassBase
    {
        private IDataFileImporter GetDataFileImporter(string name, bool remote)
        {
            var source = new DelimitedTextDataFile(
                new Uri(String.Format(@"file://{0}/{1}/RemoteImportTest.csv", Test.Constants.RemoteHost1, Test.Constants.GWCode)),
                DataFileMode.Read);
            source.GenerateIdentityColumn = true;
            source.ColumnNamesInFirstLine = true;

            File.WriteAllText(source.Uri.PathAndQuery, "#first, second, third, fourth\r\n1,2,3,4");

            var destination = new DestinationTableParameters();
            destination.Table = new Schema.Table()
                {
                    Dataset = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset("", Test.Constants.TestConnectionString),
                    SchemaName = "dbo",
                    TableName = name
                };
            destination.Operation = DestinationTableOperation.Create;

            DropTable(destination);

            IDataFileImporter dfi = null;
            if (remote)
            {
                dfi = RemoteServiceHelper.CreateObject<IDataFileImporter>(Test.Constants.Localhost);
            }
            else
            {
                dfi = new DataFileImporter();
            }

            dfi.Source = source;
            dfi.Destination = destination;

            return dfi;
        }

        [TestMethod]
        public void ImportTest()
        {
            var dfi = GetDataFileImporter("DataFileImporterTest_ImportTest", false);

            dfi.Execute();

            Assert.IsTrue(IsTableExisting(dfi.Destination));
            DropTable(dfi.Destination);

            File.Delete(dfi.Source.Uri.PathAndQuery);
            Assert.IsFalse(File.Exists(dfi.Source.Uri.PathAndQuery));
        }

        [TestMethod]
        public void RemoteImportTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var dfi = GetDataFileImporter("DataFileImporterTest_RemoteImportTest", true);

                dfi.Execute();

                Assert.IsTrue(IsTableExisting(dfi.Destination));
                DropTable(dfi.Destination);

                File.Delete(dfi.Source.Uri.PathAndQuery);
                Assert.IsFalse(File.Exists(dfi.Source.Uri.PathAndQuery));
            }
        }
    }
}
