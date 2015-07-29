using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Net.Mime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    [TestClass]
    public class DataServiceTest : ApiTestBase
    {
        protected IDataService CreateClient(RestClientSession session)
        {
            AuthenticateTestUser(session);
            var client = session.CreateClient<IDataService>(new Uri("http://localhost/gwui/api/v1/data.svc"));
            return client;
        }

        [TestMethod]
        public void DownloadTableTest()
        {
            // This test also demonstrates how to pass cookies between REST requests
            // and a standard HttpWebRequest

            using (var session = new RestClientSession())
            {
                AuthenticateTestUser(session);

                // We create a raw HTTP request here to stream data back.
                // The WCF interface apparently doesn't support this

                var uri = new Uri("http://localhost/gwui/api/v1/data.svc/MYDB/SampleData");

                var req = (HttpWebRequest)WebRequest.Create(uri);

                req.Accept = MediaTypeNames.Text.Plain;
                req.CookieContainer = session.Cookies;

                var res = (HttpWebResponse)req.GetResponse();

                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    var text = reader.ReadToEnd();
                }
            }
        }

        [TestMethod]
        public void UploadTableTest()
        {
            // This test also demonstrates how to call a WCF REST service from a standard
            // WCF client with custom headers. To access WebOperationContext from a client an
            // OperationContextScope needs to be created

            var path = GetTestFilePath(@"graywulf\test\files\csv_numbers.csv");

            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);

                // Try to drop the table before uploading a new one
                try
                {
                    client.DropTable("MYDB", "file_upload_test");
                }
                catch (Exception) { }

                using (var scope = new OperationContextScope((IContextChannel)client))
                {
                    WebOperationContext.Current.OutgoingRequest.ContentType = Jhu.Graywulf.Format.Constants.MimeTypeCsv;

                    using (var infile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        client.UploadTable("MYDB", "file_upload_test", infile);
                    }
                }

                // Drop the table that just has been created
                client.DropTable("MYDB", "file_upload_test");
            }
        }
    }
}
