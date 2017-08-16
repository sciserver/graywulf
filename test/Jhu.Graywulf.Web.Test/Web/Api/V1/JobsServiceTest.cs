using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    [TestClass]
    public class JobsServiceTest : ApiTestBase
    {

        protected IJobsService CreateClient(RestClientSession session)
        {
            AuthenticateTestUser(session);
            var uri = String.Format("http://{0}{1}/api/v1/jobs.svc", Environment.MachineName, Jhu.Graywulf.Test.AppSettings.WebUIPath);
            var client = session.CreateClient<IJobsService>(new Uri(uri));
            return client;
        }


        [TestMethod]
        public void ListQueuesTest()
        {
            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);
                var queues = client.ListQueues();
                Assert.AreEqual(2, queues.Queues.Length);   // Quick and long
            }
        }

        [TestMethod]
        public void GetQueueTest()
        {
            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);
                var queue = client.GetQueue("long");
                queue = client.GetQueue("quick");
            }
        }

        [TestMethod]
        public void ListJobsTest()
        {
            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);
                var jobs = client.ListJobs("all", "all", null, null);

                jobs = client.ListJobs("quick", "query", "1", "5");
                jobs = client.ListJobs("long", "export", "1", "5");

                jobs = client.ListJobs("all", "all", "1", "5");
                Assert.IsTrue(5 >= jobs.Jobs.Length);
            }
        }

        [TestMethod]
        public void GetJobTest()
        {
            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);
                // Get some jobs
                var jobs = client.ListJobs("all", "all", null, null);

                // Pick the first one
                var job = client.GetJob(jobs.Jobs[0].GetValue().Guid.ToString());
            }
        }

        [TestMethod]
        public void SubmitQueryJobTest()
        {
            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);

                var request = new JobRequest()
                {
                    QueryJob = new QueryJob()
                    {
                        Query = "SELECT * FROM TEST:SampleData",
                        Comments = "test comments",
                    }
                };

                var response = client.SubmitJob(JobQueue.Quick.ToString(), request);

                // Try to get newly scheduled job
                var nj = client.GetJob(response.QueryJob.Guid.ToString());


                // Now create another job depending on this one

                request = new JobRequest()
                {
                    QueryJob = new QueryJob()
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
                    }
                };

                response = client.SubmitJob(JobQueue.Quick.ToString(), request);

                var nj2 = client.GetJob(response.QueryJob.Guid.ToString());

                Assert.IsTrue(nj2.QueryJob.Dependencies.Length > 0);
            }
        }

        [TestMethod]
        public void SubmitExportJobTest()
        {
            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);

                var job = new ExportJob()
                {
                    Uri = new Uri("http://test/test.zip"),
                    FileFormat = new FileFormat()
                    {
                        MimeType = "text/csv",
                    },
                    Source = new SourceTable()
                    {
                        Dataset = Registry.Constants.UserDbName,
                        Table = "SampleData",
                    },
                    Comments = "test comments",
                };

                var request = new JobRequest()
                {
                    ExportJob = job
                };

                var response = client.SubmitJob(JobQueue.Quick.ToString(), request);

                // Try to get newly scheduled job
                var nj = client.GetJob(response.ExportJob.Guid.ToString());
            }
        }

        [TestMethod]
        public void SubmitImportJobTest()
        {
            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);

                var job = new ImportJob()
                {
                    Uri = new Uri("http://test/test.zip"),
                    Destination = new DestinationTable()
                    {
                        Dataset = Registry.Constants.UserDbName,
                        Table = "importtable",
                    },
                    Comments = "test comments",
                };

                var request = new JobRequest()
                {
                    ImportJob = job
                };

                var response = client.SubmitJob(JobQueue.Quick.ToString(), request);

                // Try to get newly scheduled job
                var nj = client.GetJob(response.ImportJob.Guid.ToString());
            }
        }

        [TestMethod]
        public void CancelJobTest()
        {
            using (var session = new RestClientSession())
            {
                var client = CreateClient(session);

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

                var response = client.SubmitJob(JobQueue.Quick.ToString(), request);

                // Try to get newly scheduled job
                var nj = client.GetJob(response.QueryJob.Guid.ToString());

                // Now cancel it
                var nj2 = client.CancelJob(response.QueryJob.Guid.ToString());

                Assert.AreEqual(JobStatus.Canceled, nj2.QueryJob.Status);
            }
        }


    }
}
