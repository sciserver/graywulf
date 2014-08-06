using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.SimpleRestClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Web.Api.V1
{
    [TestClass]
    public class JobsServiceTest : ApiTestBase
    {
        private IJobsService client;

        protected IJobsService Client
        {
            get
            {
                if (client == null)
                {
                    client = CreateClient<IJobsService>(new Uri("http://localhost/gwui/api/v1/jobs.svc"));
                }

                return client;
            }
        }

        [TestMethod]
        public void ListJobsTest()
        {
            AuthenticateUser();

            var jobs = Client.ListJobs("all", "all");
        }
    }
}
