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
    public class DataFileExporterTest : TestClassBase
    {
        private IDataFileExporter GetDataFileExporter(string name, bool remote)
        {
            var source = new SourceQueryParameters();
            source.Dataset = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerConnectionString);
            source.Query = "SELECT * FROM SampleData";

            var uri = new Uri(String.Format(@"file://{0}/{1}/{2}.txt", Test.Constants.RemoteHost1, Test.Constants.GWCode, name));

            var destination = new DelimitedTextDataFile(uri, DataFileMode.Write);

            IDataFileExporter dfe = null;
            if (remote)
            {
                dfe = RemoteServiceHelper.CreateObject<IDataFileExporter>(Test.Constants.Localhost);
            }
            else
            {
                dfe = new DataFileExporter();
            }
            dfe.Source = source;
            dfe.Destination = destination;


            return dfe;
        }

        [TestMethod]
        public void ExportTest()
        {
            var dfe = GetDataFileExporter("DataFileExporterTest_ExportTest", false);

            dfe.Execute();

            Assert.IsTrue(File.Exists(dfe.Destination.Uri.PathAndQuery));
            File.Delete(dfe.Destination.Uri.PathAndQuery);
        }

        [TestMethod]
        public void RemoteExportTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var dfe = GetDataFileExporter("DataFileExporterTest_RemoteExportTest", true);

                dfe.Execute();

                var d = dfe.Destination;
                var p = dfe.Destination.Uri;

                Assert.IsTrue(File.Exists(dfe.Destination.Uri.PathAndQuery));
                File.Delete(dfe.Destination.Uri.PathAndQuery);
            }
        }
    }
}
