using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Test.Jobs.Query
{
    [TestClass]
    public class SqlQueryTest : QueryTestBase
    {
        [TestMethod]
        public void SqlQuerySerializableTest()
        {
            var t = typeof(SqlQuery);

            var sc = new SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        /*
        [TestMethod]
        public void SimpleQueryTest()
        {
            // *** TODO: rewrite this to work with test setup instead of skyquery

            var sql = "SELECT TOP 10 specobjid INTO SqlQueryTest_SimpleQueryTest FROM SDSSDR7:SpecObjAll";

            ScheduleQueryJob(sql, QueueType.Long);

            StartScheduler();

            // Leave enough time for the scheduler to start
            Thread.Sleep(TimeSpan.FromSeconds(20));

            // Drain-stop
            StopScheduler();

            // TODO: add drop
        }*/
    }
}
