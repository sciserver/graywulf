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
        public void ListQueuesTest()
        {
            AuthenticateUser();

            var queues = Client.ListQueues();
        }

        [TestMethod]
        public void ListJobsTest()
        {
            AuthenticateUser();

            var jobs = Client.ListJobs("all", "all", null, null);

            jobs = Client.ListJobs("quick", "query", "1", "5");
            jobs = Client.ListJobs("long", "export", "1", "5");

            jobs = Client.ListJobs("all", "all", "1", "5");
            Assert.AreEqual(5, jobs.Jobs.Length);
        }
    }
}
