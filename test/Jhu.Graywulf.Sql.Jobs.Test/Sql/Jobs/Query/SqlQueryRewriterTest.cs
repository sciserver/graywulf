using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Jobs.Query;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryGeneration.SqlServer;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Sql.Schema;


namespace Jhu.Graywulf.Sql.Jobs.Query
{
    [TestClass]
    public class SqlQueryRewriterTest : SqlQueryTestBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            StopLogger();
        }
       
        #region Remove extra tokens

        [TestMethod]
        [TestCategory("Parsing")]
        public void LeaveIntact()
        {
            var sql = "SELECT * FROM Table1";
            var gt = "SELECT * FROM Table1";

            RewriteQueryHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RewriteIntoClause()
        {
            var sql = "SELECT * INTO Table2 FROM Table1 ORDER BY column1";
            var gt =
@"BEGIN
PRINT '<graywulf><destinationName>Table2</destinationName></graywulf>';
SELECT *  FROM Table1 
END";

            RewriteQueryHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RewriteIntoClause2()
        {
            var sql = "SELECT * INTO dbo.Table2 FROM Table1 ORDER BY column1";
            var gt =
@"BEGIN
PRINT '<graywulf><destinationSchema>dbo</destinationSchema><destinationName>Table2</destinationName></graywulf>';
SELECT *  FROM Table1 
END";

            RewriteQueryHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveOrderByClause()
        {
            var sql = "SELECT * FROM Table1 ORDER BY column1";
            var gt = "SELECT * FROM Table1 ";

            RewriteQueryHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveOrderByClauseFromUnion()
        {
            var sql = "SELECT * FROM Table1 UNION SELECT * FROM Table2 ORDER BY column1";
            var gt = "SELECT * FROM Table1 UNION SELECT * FROM Table2 ";

            RewriteQueryHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveTablePartitionClause()
        {
            var sql = "SELECT * FROM Table2 PARTITION BY id";
            var gt = "SELECT * FROM Table2 ";

            RewriteQueryHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveAllExtraTokens()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id ORDER BY column1";
            var gt = "SELECT * FROM Table1  ";

            RewriteQueryHelper(sql, gt);
        }

        #endregion
        #region Append partitioning

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningFrom()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id";
            var gt = @"SELECT * FROM Table1 
WHERE @__partKeyMin <= id";

            RewritePartitioningTestHelper(sql, gt, true, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningFromWithWhere()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id WHERE x < 5";
            var gt = "SELECT * FROM Table1  WHERE (@__partKeyMin <= id) AND (x < 5)";

            RewritePartitioningTestHelper(sql, gt, true, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningTo()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id";
            var gt = @"SELECT * FROM Table1 
WHERE id < @__partKeyMax";

            RewritePartitioningTestHelper(sql, gt, false, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBoth()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id";
            var gt = @"SELECT * FROM Table1 
WHERE @__partKeyMin <= id AND id < @__partKeyMax";

            RewritePartitioningTestHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithWhere()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id WHERE x < 5";
            var gt = @"SELECT * FROM Table1  WHERE (@__partKeyMin <= id AND id < @__partKeyMax) AND (x < 5)";

            RewritePartitioningTestHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithJoin()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id CROSS JOIN Table2";
            var gt = @"SELECT * FROM Table1  CROSS JOIN Table2
WHERE @__partKeyMin <= id AND id < @__partKeyMax";

            RewritePartitioningTestHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithWhereAndJoin()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id CROSS JOIN Table2 WHERE x < 5";
            var gt = "SELECT * FROM Table1  CROSS JOIN Table2 WHERE (@__partKeyMin <= id AND id < @__partKeyMax) AND (x < 5)";

            RewritePartitioningTestHelper(sql, gt, true, true);
        }

        #endregion
        #region Star column substitution

        [TestMethod]
        public void SubstituteStarTest()
        {
            var sql = "SELECT * FROM Author";
            var gt = "SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]";

            SubstituteStarsTestHelper(sql, gt);
        }

        #endregion
        #region Assign column aliases

        [TestMethod]
        public void AssignColumnAliasesTest()
        {
            var sql = "SELECT * FROM Author a";
            var gt = "SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]";

            AssignColumnAliasesTestHelper(sql, gt);
        }
        
        #endregion
    }
}
