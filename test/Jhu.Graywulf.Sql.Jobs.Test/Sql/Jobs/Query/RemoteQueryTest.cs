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
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.CodeGeneration;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    [TestClass]
    public class RemoteQueryTest : SqlQueryTestBase
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

        #region Most restrictive remote query generation

        private string[] GenerateMostRestrictiveTableQueryTestHelper(string sql, ColumnContext columnContext, int top)
        {
            var query = CreateQuery(sql);
            query.GeneratePartitions();
            var res = query.Partitions[0].GenerateRemoteSourceTableQueries(columnContext, top);

            return res.Values.ToArray();
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_SimpleTest()
        {
            var sql =
@"SELECT c.RA
FROM MYDB:MyCatalog c
WHERE c.objID = 1";

            var gt =
@"SELECT 
[ObjID], [Ra]
FROM [MYDB_1768162722].[mydb].[MyCatalog] 
WHERE ([ObjID] = 1)
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_PrimaryKeyTest()
        {
            var sql =
@"SELECT c.RA
FROM MYDB:MyCatalog c
WHERE c.Dec = 1";

            var gt =
@"SELECT 
[ObjID], [Ra], [Dec]
FROM [MYDB_1768162722].[mydb].[MyCatalog] 
WHERE ([Dec] = 1)
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default | ColumnContext.PrimaryKey, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_MultipleAliasesTest()
        {
            var sql =
@"SELECT a.RA, b.Dec
FROM MYDB:MyCatalog a CROSS JOIN MYDB.MyCatalog b
WHERE b.objID = 1 AND a.objID IN (3, 4)";

            var gt =
@"SELECT 
[ObjID], [Ra], [Dec]
FROM [MYDB_1768162722].[mydb].[MyCatalog] 
WHERE ([ObjID] = 1) OR ([ObjID] IN (3, 4))
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_MultipleSelectsTest()
        {
            var sql =
@"SELECT a.RA FROM MYDB:MyCatalog a
WHERE a.objID IN (3, 4)

SELECT b.Dec FROM MYDB:MyCatalog b
WHERE b.Dec = 10";

            var gt =
@"SELECT 
[ObjID], [Ra], [Dec]
FROM [MYDB_1768162722].[mydb].[MyCatalog] 
WHERE ([Dec] = 10) OR ([ObjID] IN (3, 4))
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_MultipleSelectsNoAliasTest()
        {
            var sql =
@"SELECT RA FROM MYDB:MyCatalog
WHERE objID IN (3, 4)

SELECT Dec FROM MYDB:MyCatalog
WHERE Dec = 10";

            var gt =
@"SELECT 
[ObjID], [Ra], [Dec]
FROM [MYDB_1768162722].[mydb].[MyCatalog] 
WHERE ([Dec] = 10) OR ([ObjID] IN (3, 4))
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_UnionTest()
        {
            var sql =
@"SELECT RA, Dec
FROM MYDB:MyCatalog
WHERE objID IN (2, 3)
UNION
SELECT RA, Dec + 1
FROM MYDB:MyCatalog
WHERE objID = 1";

            var gt =
@"SELECT 
[ObjID], [Ra], [Dec]
FROM [MYDB_1768162722].[mydb].[MyCatalog] 
WHERE ([ObjID] = 1) OR ([ObjID] IN (2, 3))
";
            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        #endregion
    }
}
