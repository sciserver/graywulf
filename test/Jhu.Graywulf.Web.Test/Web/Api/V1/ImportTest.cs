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
    public class ImportTest : JobsServiceTest
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

        protected virtual void ImportFileHelper(string uri, bool generateIdentityColumn)
        {
            ImportFileHelper(uri, Registry.Constants.UserDbName, null, generateIdentityColumn);
        }

        protected virtual void ImportFileHelper(string uri, string dataset, string table, bool generateIdentityColumn)
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

                        var job = new ImportJob()
                        {
                            Uri = new Uri(uri),
                            Comments = GetTestUniqueName(),
                            Destination = new DestinationTable()
                            {
                                Dataset = dataset,
                                Table = table,
                            },
                            /*Options = new ImportOptions()
                            {
                                GenerateIdentityColumn = generateIdentityColumn
                            }*/
                        };

                        var request = new JobRequest()
                        {
                            ImportJob = job
                        };

                        var response = client.SubmitJob(JobQueue.Quick.ToString(), request);
                        
                        // Try to get newly scheduled job
                        var nj = client.GetJob(response.ImportJob.Guid.ToString());
                        var guid = nj.ImportJob.Guid;

                        WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                        var ji = LoadJob(guid);
                        Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                    }
                }
            }
        }

        [TestMethod]
        public void ImportFileFromHttpTest()
        {
            ImportFileHelper("http://localhost/graywulf_io_test/csv_numbers.csv", false);
            ImportFileHelper("http://localhost/graywulf_io_test/csv_numbers.csv", true);
        }

        // TODO: add FTP

        [TestMethod]
        public void ImportFileWithTableNameTest()
        {
            ImportFileHelper("http://localhost/graywulf_io_test/csv_numbers.csv", Registry.Constants.UserDbName, "csv_numbers", false);
        }

        [TestMethod]
        public void ImportCompressedTest()
        {
            ImportFileHelper("http://localhost/graywulf_io_test/csv_numbers.csv.gz", false);
        }

        [TestMethod]
        public void ImportArchiveTest()
        {
            ImportFileHelper("http://localhost/graywulf_io_test/csv_numbers.zip", false);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceModel.CommunicationException))]
        public void InvalidDatasetTest()
        {
            // Import into the TEST dataset is restricted
            ImportFileHelper("http://localhost/graywulf_io_test/csv_numbers.zip", "Test", "InvalidDatasetTest", false);
        }
    }
}
