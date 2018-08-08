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
    public class ExecuteComplexQueriesTest : SqlQueryTestBase
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
            Assert.AreEqual(1, GetUserDatabaseTableCount(GetTestUniqueName()));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void DeclareSetVariableFromQueryTest()
        {
            var sql = @"
DECLARE @myvar float
SET @myvar = (SELECT AVG(ra) FROM SDSSDR7:SpecObj)
SELECT @myvar INTO [$into]
";

            RunQuery(sql);
            Assert.AreEqual(1, GetUserDatabaseTableCount(GetTestUniqueName()));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void IfStatementWithVariablesTest()
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

            Assert.IsTrue(IsUserDatabaseTableExisting(testName + "_1"));
            Assert.AreEqual(1, GetUserDatabaseTableCount(testName + "_1"));
            Assert.IsFalse(IsUserDatabaseTableExisting(testName + "_2"));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void IfStatementWithSubqueryTest()
        {
            var testName = GetTestUniqueName();

            var sql = @"
IF 5 < ANY (SELECT AVG(ra) FROM SDSSDR7:SpecObj)
BEGIN
    SELECT 1 INTO [$into]
END ELSE BEGIN
    SELECT 0 INTO [$into]
END
";

            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName));
            Assert.AreEqual(1, GetUserDatabaseTableCount(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void WhileStatementWithInsertTest1()
        {
            var testName = GetTestUniqueName();

            var sql = @"
DECLARE @t AS TABLE
(
    ra float, dec float
)

DECLARE @i int = 0
WHILE @i < 3 BEGIN

    INSERT @t
    SELECT TOP 10 ra, dec
    FROM SDSSDR7:SpecObj

    SET @i = @i + 1
END

SELECT * INTO [$into] FROM @t
";

            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName));
            Assert.AreEqual(30, GetUserDatabaseTableCount(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void WhileStatementWithInsertTest2()
        {
            var testName = GetTestUniqueName();

            var sql = @"
CREATE TABLE [$into]
(
    ra float, dec float
)

DECLARE @i int = 0
WHILE @i < 3 BEGIN

    INSERT [$into]
    SELECT TOP 10 ra, dec
    FROM SDSSDR7:SpecObj

    SET @i = @i + 1
END
";

            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName));
            Assert.AreEqual(30, GetUserDatabaseTableCount(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void InsertIntoNewTableTest()
        {
            var testName = GetTestUniqueName();

            var sql = @"
CREATE TABLE [$into]
(
    ra float, dec float
)

INSERT [$into]
VALUES
(10, 20), (30, 40)

INSERT [$into]
VALUES
(5, 10), (11, 12)
";

            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName));
            Assert.AreEqual(4, GetUserDatabaseTableCount(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void InsertIntoExistingTableTest1()
        {
            // TODO: insert some stuff that doesn't require remote data
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Query")]
        public void InsertIntoExistingTableTest2()
        {
            // TODO: Try to insert into existing table from a remote table
            // and make sure it fails at validation
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Query")]
        public void DropExistingTableTest()
        {
            // TODO: drop an existing mydb table
        }

        [TestMethod]
        [TestCategory("Query")]
        public void DropNewTableTest()
        {
            var testName = GetTestUniqueName();

            var sql = @"
CREATE TABLE [$into]
(
    ra float, dec float
)

INSERT [$into]
VALUES
(10, 20), (30, 40)

DROP TABLE [$into]
";

            RunQuery(sql);

            Assert.IsFalse(IsUserDatabaseTableExisting(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void UpdateNewTableTest1()
        {
            var testName = GetTestUniqueName();

            var sql = @"
CREATE TABLE [$into]
(
    ra float, dec float
)

INSERT [$into]
VALUES
(10, 20), (30, 40)

UPDATE [$into]
SET ra = ra - 5
";

            RunQuery(sql);

            Assert.IsTrue(IsUserDatabaseTableExisting(testName));
            Assert.AreEqual(2, GetUserDatabaseTableCount(testName));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void UpdateNewTableTest2()
        {
            // TODO: create and update table with remote data source
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Query")]
        public void UpdateExistingTableTest1()
        {
            // TODO: update existing local table with local data only
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Query")]
        public void UpdateExistingTableTest2()
        {
            // TODO: update existing local table with remote data -> should fail at validation
            Assert.Fail();
        }
    }
}
