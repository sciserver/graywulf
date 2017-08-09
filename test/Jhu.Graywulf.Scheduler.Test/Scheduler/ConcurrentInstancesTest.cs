using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    [TestClass]
    public class ConcurrentInstancesTest : SchedulerTestBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ClassInitialize();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            ClassCleanUp();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestInitialize(false, 2);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void StartStopMultipleInstancesTest()
        {
            Thread.Sleep(new TimeSpan(0, 0, 20));
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Slow")]
        public void ManySimpleJobsSequentialFeederTest()
        {
            var guids = new Guid[20];

            for (int i = 0; i < guids.Length; i++)
            {
                guids[i] = ScheduleTestJob(new TimeSpan(0, 0, 10), JobType.AtomicDelay, QueueType.Quick, new TimeSpan(0, 2, 0));
            }

            WaitJobComplete(guids[guids.Length - 1], TimeSpan.FromSeconds(5));
            SchedulerTester.Instance.DrainStop();

            for (int i = 0; i < guids.Length; i++)
            {
                var job = LoadJob(guids[i]);

                Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
                Assert.AreEqual("OK", (string)job.Parameters["Result"].Value);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Slow")]
        public void ManySimpleJobsParallelFeederTest()
        {
            Parallel.For(0, 20, i =>
            {
                var guid = ScheduleTestJob(new TimeSpan(0, 0, 1), JobType.AtomicDelay, QueueType.Quick, new TimeSpan(0, 2, 0));

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
                Assert.AreEqual("OK", (string)job.Parameters["Result"].Value);
            });
        }
    }
}
