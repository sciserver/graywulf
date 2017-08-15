using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    [TestClass]
    public class FullSchedulerTest : SchedulerTestBase
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
            TestInitialize(true, 1);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void StartStopWithLoadLayoutTest()
        {
            Thread.Sleep(new TimeSpan(0, 0, 20));
        }
    }
}
