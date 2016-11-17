using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.Check
{
    public class UrlCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get { return CheckCategory.Url; }
        }

        public Uri Uri { get; set; }
        private HttpStatusCode ExpectedStatus;

        public UrlCheck(string uri)
        {
            InitializeMembers();

            this.Uri = new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        public UrlCheck(string uri, HttpStatusCode expectedStatus)
        {
            InitializeMembers();

            this.Uri = new Uri(uri, UriKind.RelativeOrAbsolute);
            this.ExpectedStatus = expectedStatus;
        }

        private void InitializeMembers()
        {
            this.Uri = null;
            this.ExpectedStatus = HttpStatusCode.OK;
        }

        public override void Execute(TextWriter output)
        {
            output.WriteLine(
                "Testing URL {0} expecting HTTP status {1}",
                Uri,
                (int)ExpectedStatus);

            try
            {
                var req = HttpWebRequest.Create(Uri);
                var res = req.GetResponse();

                var status = ((HttpWebResponse)res).StatusCode;
                if (status != ExpectedStatus)
                {
                    throw new Exception(String.Format("Unexpected HTTP status code {0}", (int)status));
                }
            }
            catch (WebException ex)
            {
                var status = ((HttpWebResponse)ex.Response).StatusCode;
                if (status != ExpectedStatus)
                {
                    throw;
                }
            }

            output.WriteLine("Page retrieved successfully.");
        }
    }
}
