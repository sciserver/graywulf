using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.QueryRendering;
using Jhu.Graywulf.Sql.QueryRendering.SqlServer;

namespace Jhu.Graywulf.Sql.Extensions.QueryRewriting
{
    [TestClass]
    public class GraywulfSqlQueryRewriterTest : Parsing.ParsingTestBase
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

        protected GraywulfSqlQueryRewriter CreateQueryRewriter(bool partitioningKeyMin, bool partitioningKeyMax)
        {
            return new GraywulfSqlQueryRewriter()
            {
                PartitioningKeyMin = partitioningKeyMin ? (object)0 : null,
                PartitioningKeyMax = partitioningKeyMax ? (object)1 : null,
                Options = new GraywulfSqlQueryRewriterOptions()
                {
                    AppendPartitioningCondition = true,
                    AssignColumnAliases = true,
                    RemoveOrderBy = true,
                    RemovePartitioningClause = true,
                    SubstituteStars = true,
                }
            };
        }

        protected SqlServerQueryRenderer CreateQueryRenderer()
        {
            return new SqlServerQueryRenderer()
            {
                Options = new QueryRendering.QueryRendererOptions()
                {
                    ColumnNameRendering = NameRendering.Original,
                    TableNameRendering = NameRendering.Original,
        }
            };
        }

        protected virtual void RewriteQueryHelper(string sql, string gt)
        {
            var qrw = CreateQueryRewriter(false, false);
            qrw.Options.SubstituteStars = false;

            var qr = CreateQueryRenderer();

            var parsingTree = Parse(sql);
            qrw.Execute(parsingTree);
            var res = qr.Execute(parsingTree);

            Assert.AreEqual(gt, res);
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
        #region System variables

        [TestMethod]
        public void PartitionVariablesTest()
        {
            var sql = @"SELECT @@PARTCOUNT, @@PARTID";
            var gt = @"SELECT @__partCount, @__partId";

            RewriteQueryHelper(sql, gt);
        }

        #endregion
        #region Star column substitution

        protected void SubstituteStarsTestHelper(string sql, string gt)
        {
            var qd = ParseAndResolveNames(sql);

            var qrw = CreateQueryRewriter(false, false);
            qrw.Options.AssignColumnAliases = false;

            var qr = CreateQueryRenderer();
            qr.Options.ColumnNameRendering = NameRendering.FullyQualified;
            qr.Options.ColumnAliasRendering = AliasRendering.Always;
            qr.Options.TableNameRendering = NameRendering.FullyQualified;
            qr.Options.TableAliasRendering = AliasRendering.Always;

            qrw.Execute(qd.ParsingTree);
            var res = qr.Execute(qd.ParsingTree);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void SubstituteStarTest()
        {
            var sql = "SELECT * FROM Author";
            var gt = "SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]";

            SubstituteStarsTestHelper(sql, gt);
        }

        [TestMethod]
        public void SubstituteStarWithAliasTest()
        {
            var sql = "SELECT a.* FROM Author a";
            var gt = "SELECT [a].[ID], [a].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]";

            SubstituteStarsTestHelper(sql, gt);
        }

        [TestMethod]
        public void SubstituteStarWithCreateTableTest()
        {
            var sql = 
@"CREATE TABLE test
(
    ID int PRIMARY KEY,
    Data nvarchar(50)
)

SELECT a.* FROM test a";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID]  PRIMARY KEY,
    [Data] 
)

SELECT [a].[Data], [a].[ID] FROM [Graywulf_Schema_Test].[dbo].[test] [a]";

            SubstituteStarsTestHelper(sql, gt);
        }

        [TestMethod]
        public void SubstituteStarWithDeclareTableTest()
        {
            var sql =
@"DECLARE @test AS TABLE
(
    ID int PRIMARY KEY,
    Data nvarchar(50)
)

SELECT a.* FROM @test a";

            var gt =
@"DECLARE @test AS TABLE
(
    [ID]  PRIMARY KEY,
    [Data] 
)

SELECT [a].[ID], [a].[Data] FROM @test [a]";

            SubstituteStarsTestHelper(sql, gt);
        }

        #endregion
        #region Assign column aliases

        protected void AssignColumnAliasesTestHelper(string sql, string gt)
        {
            var qd = ParseAndResolveNames(sql);
            var qrw = CreateQueryRewriter(false, false);

            var qr = CreateQueryRenderer();
            qr.Options.ColumnNameRendering = NameRendering.FullyQualified;
            qr.Options.ColumnAliasRendering = AliasRendering.Always;
            qr.Options.TableNameRendering = NameRendering.FullyQualified;
            qr.Options.TableAliasRendering = AliasRendering.Always;

            qrw.Execute(qd.ParsingTree);
            var res = qr.Execute(qd.ParsingTree);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void AssignColumnAliasesTest()
        {
            var sql = "SELECT * FROM Author a";
            var gt = "SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]";

            AssignColumnAliasesTestHelper(sql, gt);
        }

        [TestMethod]
        public void AssignColumnAliasesSubqueryTest()
        {
            var sql = "SELECT * FROM (SELECT * FROM Author a) q";
            var gt = "SELECT [q].[ID] AS [q_ID], [q].[Name] AS [q_Name] FROM (SELECT [a].[ID], [a].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]) [q]";

            AssignColumnAliasesTestHelper(sql, gt);
        }

        [TestMethod]
        public void AssignColumnAliaseCteTest()
        {
            var sql =
@"WITH q AS (SELECT * FROM Author a)
SELECT * FROM q";

            var gt =
@"WITH [q] AS (SELECT [a].[ID], [a].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a])
SELECT [q].[ID] AS [q_ID], [q].[Name] AS [q_Name] FROM [q]";
            
            AssignColumnAliasesTestHelper(sql, gt);
        }

        #endregion
        #region Append partitioning

        protected void RewritePartitioningTestHelper(string sql, string gt, bool partitioningKeyMin, bool partitioningKeyMax)
        {
            var qrw = CreateQueryRewriter(partitioningKeyMin, partitioningKeyMax);
            qrw.Options.SubstituteStars = false;
            qrw.Options.AssignColumnAliases = false;

            var qr = CreateQueryRenderer();
            qr.Options.TableNameRendering = NameRendering.Original;
            qr.Options.ColumnNameRendering = NameRendering.Original;

            var parsingTree = Parse(sql);
            qrw.Execute(parsingTree);
            var res = qr.Execute(parsingTree);

            Assert.AreEqual(gt, res);
        }

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
    }
}
