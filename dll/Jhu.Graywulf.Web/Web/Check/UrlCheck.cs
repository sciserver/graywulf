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
        public string Url;
        public HttpStatusCode ExpectedStatus;

        public UrlCheck(string url)
        {
            InitializeMembers();

            this.Url = url;
        }

        public UrlCheck(string url, HttpStatusCode expectedStatus)
        {
            InitializeMembers();

            this.Url = url;
            this.ExpectedStatus = expectedStatus;
        }

        private void InitializeMembers()
        {
            this.Url = "";
            this.ExpectedStatus = HttpStatusCode.OK;
        }

        public override void Execute(PageBase page)
        {
            var absurl = Util.UrlFormatter.ToAbsoluteUrl(Url);

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
