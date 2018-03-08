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
    public sealed class ExportTest : ApiTestBase
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
        
        [TestMethod]
        public void SimpleExportTest()
        {
            var path = GetAbsoluteTestUniqueFileUri("output", ".zip");
            ExportFileHelper(Registry.Constants.UserDbName, "SampleData", path.ToString(), "text/csv", "ExportToSciDriveCsvTest");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceModel.CommunicationException))]
        public void InvalidDatasetTest()
        {
            // Export from TEST dataset is restricted
            var path = GetAbsoluteTestUniqueFileUri("output", ".zip");
            ExportFileHelper("TEST", "SampleData", path.ToString(), "text/csv", "ExportToSciDriveCsvTest");
        }
    }
}
