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
    public class CopyTableTest : ApiTestBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();

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

            StopLogger();
        }

        protected virtual void CopyTableHelper(string sourceDataset, string sourceTable, string destinationDataset, string destinationTable)
        {
            DropUserDatabaseTable(destinationTable);

            using (SchedulerTester.Instance.GetToken())
            {
                PurgeTestJobs();

                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    using (var session = new RestClientSession())
                    {
                        var client = CreateJobServiceClient(session);

                        var job = new CopyJob()
                        {
                            Source = new SourceTable()
                            {
                                Dataset = sourceDataset,
                                Table = sourceTable,
                            },
                            Destination = new DestinationTable()
                            {
                                Dataset = destinationDataset,
                                Table = destinationTable,
                            }
                        };

                        var request = new JobRequest()
                        {
                            CopyJob = job
                        };

                        var response = client.SubmitJob(JobQueue.Quick.ToString(), request);

                        // Try to get newly scheduled job
                        var nj = client.GetJob(response.CopyJob.Guid.ToString());
                        var guid = nj.CopyJob.Guid;

                        WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                        var ji = LoadJob(guid);
                        Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                    }
                }
            }
        }

        [TestMethod]
        public void SimpleCopyTest()
        {
            CopyTableHelper(Registry.Constants.UserDbName, "SampleData", Registry.Constants.UserDbName, GetTestUniqueName());
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceModel.CommunicationException))]
        public void InvalidDatasetTest()
        {
            // Copy from TEST dataset is restricted
            CopyTableHelper("TEST", "SampleData", Registry.Constants.UserDbName, GetTestUniqueName());
        }
    }
}
