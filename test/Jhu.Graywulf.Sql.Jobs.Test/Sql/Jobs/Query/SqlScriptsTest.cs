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

        [TestMethod]
        [TestCategory("Query")]
        public void DeclareSetAndSelectVariableTest()
        {
            var sql = @"
DECLARE @myvar float = 20
SET @myvar = 15
SELECT @myvar = @myvar - 5
SELECT @myvar INTO [$into]
";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void FlowControlTest()
        {
            var testName = GetTestUniqueName();
            DropUserDatabaseTable(testName + "_1");
            DropUserDatabaseTable(testName + "_2");

            var sql = @"
DECLARE @myvar int = 2
IF @myvar < 5
BEGIN
    SELECT @myvar INTO [$into]_1
END
IF @myvar > 5
BEGIN
    SELECT @myvar INTO [$into]_2
END
";

            RunQuery(sql);
        }
    }
}
