using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Check
{
    public class UrlCheck : CheckRoutineBase
    {
        public Uri uri;
        public HttpStatusCode ExpectedStatus;

        public UrlCheck(string url)
        {
            InitializeMembers();

            this.uri = new Uri(url, UriKind.RelativeOrAbsolute);
        }

        public UrlCheck(string url, HttpStatusCode expectedStatus)
        {
            InitializeMembers();

            this.uri = new Uri(url, UriKind.RelativeOrAbsolute);
            this.ExpectedStatus = expectedStatus;
        }

        private void InitializeMembers()
        {
            this.uri = null;
            this.ExpectedStatus = HttpStatusCode.OK;
        }

        public override void Execute(PageBase page)
        {
            var absurl = Util.UriConverter.Combine(page.Request.Url, uri).ToString();

            page.Response.Output.WriteLine(
                "Testing URL {0} expecting HTTP status {1}",
                absurl,
                (int)ExpectedStatus);

            try
            {
                var req = HttpWebRequest.Create(absurl);
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

            page.Response.Output.WriteLine("Page retrieved successfully.");
        }
    }
}
