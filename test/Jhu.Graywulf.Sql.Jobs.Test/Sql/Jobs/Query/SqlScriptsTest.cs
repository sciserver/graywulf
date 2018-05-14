using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Jobs.Query;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    [TestClass]
    public class SqlScriptsTest : SqlQueryTestBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
            InitializeJobTests();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            CleanupJobTests();
            StopLogger();
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SystemVariablesTest()
        {
            var sql = @"
SELECT @@VERSION
INTO [$into]
";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void PartitionVariablesTest()
        {
            var sql = @"
SELECT @@PARTCOUNT, @@PARTID
INTO [$into]
";

            RunQuery(sql);
        }
    }
}
