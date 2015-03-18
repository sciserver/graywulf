using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Web.Services;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Api.V1
{
    /// <summary>
    /// Implements functions for end to end testing of import
    /// operations initiated from the web interface or the REST services.
    /// </summary>
    [TestClass]
    public class ExportTest : JobsServiceTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                PurgeTestJobs();
            }
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            using (RemoteServiceTester.Instance.GetExclusiveToken())
            {
                if (RemoteServiceTester.Instance.IsRunning)
                {
                    RemoteServiceTester.Instance.Stop();
                }
            }

            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                if (SchedulerTester.Instance.IsRunning)
                {
                    SchedulerTester.Instance.DrainStop();
                }

                PurgeTestJobs();
            }
        }

        protected virtual void ExportFileHelper(string table, string uri, string mimeType, string comments)
        {
            using (SchedulerTester.Instance.GetToken())
            {
                PurgeTestJobs();

                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    using (var session = new RestClientSession())
                    {
                        var client = CreateClient(session);

                        var job = new ExportJob()
                        {
                            Table = table,
                            Uri = new Uri(uri),
                            Comments = comments,
                            FileFormat = new FileFormat()
                            {
                                MimeType = mimeType,
                            }
                        };

                        var request = new JobRequest()
                        {
                            ExportJob = job
                        };

                        var response = client.SubmitJob("quick", request);
                        
                        // Try to get newly scheduled job
                        var nj = client.GetJob(response.ExportJob.Guid.ToString());
                        var guid = nj.ExportJob.Guid;

                        WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                        var ji = LoadJob(guid);
                        Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                    }
                }
            }
        }

        
    }
}
