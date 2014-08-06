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
            Assert.AreEqual(2, queues.Queues.Length);
        }

        [TestMethod]
        public void GetQueueTest()
        {
            AuthenticateUser();

            var queue = Client.GetQueue("long");
            queue = Client.GetQueue("quick");
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

        [TestMethod]
        public void GetJobTest()
        {
            AuthenticateUser();

            // Get some jobs
            var jobs = Client.ListJobs("all", "all", null, null);

            // Pick the first one
            var job = Client.GetJob(jobs.Jobs[0].GetValue().Guid.ToString());
        }

        [TestMethod]
        public void SubmitQueryJobTest()
        {
            AuthenticateUser();

            var job = new QueryJob()
            {
                Query = "SELECT * FROM TEST:SampleData",
                Comments = "test comments",
            };

            var request = new JobRequest()
            {
                QueryJob = job
            };

            var response = Client.SubmitJob("quick", request);

            // Try to get newly scheduled job
            var nj = Client.GetJob(response.QueryJob.Guid.ToString());


            // Now create another job depending on this one

            job = new QueryJob()
            {
                Query = "SELECT * FROM TEST:SampleData -- JOB 2",
                Comments = "test comments",
                Dependencies = new JobDependency[]
                {
                    new JobDependency()
                    {
                        Condition = JobDependencyCondition.Completed,
                        PredecessorJobGuid = nj.QueryJob.Guid
                    }
                }
            };

            request = new JobRequest()
            {
                QueryJob = job
            };

            response = Client.SubmitJob("quick", request);

            var nj2 = Client.GetJob(response.QueryJob.Guid.ToString());

            Assert.IsTrue(nj2.QueryJob.Dependencies.Length > 0);
        }

        [TestMethod]
        public void CancelJobTest()
        {
            AuthenticateUser();

            // Create a simple job first

            var job = new QueryJob()
            {
                Query = "SELECT * FROM TEST:SampleData",
                Comments = "test comments",
            };

            var request = new JobRequest()
            {
                QueryJob = job
            };

            var response = Client.SubmitJob("quick", request);

            // Try to get newly scheduled job
            var nj = Client.GetJob(response.QueryJob.Guid.ToString());

            // Now cancel it
            var nj2 = Client.CancelJob(response.QueryJob.Guid.ToString());

            Assert.AreEqual(JobStatus.Canceled, nj2.QueryJob.Status);
        }
    }
}
