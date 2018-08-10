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
    public class AssignServerInstanceTest : SqlQueryTestBase
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
        public void SelectSingleServerNoTableReferenceTest()
        {
            var testName = GetTestUniqueName();
            var sql = "SELECT 1";

            DropUserDatabaseTable(testName + "_0");
            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName + "_0"));
            Assert.AreEqual(1, GetUserDatabaseTableCount(testName + "_0"));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SelectIntoSingleServerTest()
        {
            var testName = GetTestUniqueName();
            var sql = "SELECT 1 INTO [$into]";

            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName));
            Assert.AreEqual(1, GetUserDatabaseTableCount(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SelectIntoSameServerTest()
        {
            var testName = GetTestUniqueName();
            var sql = "SELECT TOP 10 ra, dec INTO [$into] FROM MYDB:MyCatalog";

            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName));
            Assert.AreEqual(10, GetUserDatabaseTableCount(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SelectIntoOtherServerTest()
        {
            var testName = GetTestUniqueName();
            var sql = "SELECT TOP 10 ra, dec INTO [$into] FROM SDSSDR7:SpecObj ";

            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName));
            Assert.AreEqual(10, GetUserDatabaseTableCount(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void ClrFunctionFromCodeDbTest()
        {
            var testName = GetTestUniqueName();
            var sql = "SELECT region.Area(region.CircleEq(10, 10, 1))";

            DropUserDatabaseTable(testName + "_0");
            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName + "_0"));
            Assert.AreEqual(1, GetUserDatabaseTableCount(testName + "_0"));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SystemFunctionTest()
        {
            var testName = GetTestUniqueName();
            var sql = "SELECT SIN(2)";

            DropUserDatabaseTable(testName + "_0");
            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName + "_0"));
            Assert.AreEqual(1, GetUserDatabaseTableCount(testName + "_0"));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void ClrDataTypeFromCodeDbTest()
        {
            var sql = "DECLARE @r dbo.Region";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SystemDataTypeTest()
        {
            var sql = "DECLARE @r int";

            RunQuery(sql);
        }

        /*[TestMethod]
        [TestCategory("Query")]
        public void ClrAggregateFunctionFromCodeDbTest()
        {
            var sql =
@"SELECT region.UnionEvery(region.CircleEq(ra, dec, 1))
INTO [$into]
FROM TEST:SDSSDR7PhotoObjAll a
";

            RunQuery(sql);
        }*/
    }
}
