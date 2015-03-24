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
    public class ImportTableTest
    {
        private IImportTable GetImportTableTask(string path, string table, bool remote)
        {
            var ds = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString);
            ds.IsMutable = true;


            var ff = FileFormatFactory.Create(null);

            var source = ff.CreateFile(new Uri(path, UriKind.RelativeOrAbsolute));
           
            var destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = ds.DefaultSchemaName,
                TableNamePattern = table,
            };

            IImportTable it = null;
            if (remote)
            {
                it = RemoteServiceHelper.CreateObject<IImportTable>(Test.Constants.Localhost, false);
            }
            else
            {
                it = new ImportTable();
            }

            it.Source = source;
            it.Destination = destination;

            return it;
        }

        [TestMethod]
        public void ImportTest()
        {
            var path = @"..\..\..\graywulf\test\files\csv_numbers.csv";
            var table = "TableImportTest_ImportTest";
            var it = GetImportTableTask(path, table, false);

            it.Execute();

            it.Destination.GetTable().Drop();
        }

        [TestMethod]
        public void RemoteImportTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var path = @"..\..\..\graywulf\test\files\csv_numbers.csv";
                var table = "TableImportTest_RemoteImportTest";
                var it = GetImportTableTask(path, table, false);

                it.Execute();

                it.Destination.GetTable().Drop();
            }
        }

        [TestMethod]
        public void ImportFromHttpTest()
        {
            var path = @"http://localhost/graywulf_io_test/csv_numbers.csv";
            var table = "TableImportTest_ImportFromHttpTest";
            var it = GetImportTableTask(path, table, false);

            try
            {
                it.Destination.GetTable().Drop();
            }
            catch (Exception)
            {
            }

            it.Execute();

            it.Destination.GetTable().Drop();
        }
    }
}
