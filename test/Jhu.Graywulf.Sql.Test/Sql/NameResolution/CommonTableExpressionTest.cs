using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class CommonTableExpressionTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleCteTest()
        {
            var sql =
@"WITH a AS
(
    SELECT * FROM Author
)
SELECT Name FROM a";

            var gt =
@"WITH [a] AS
(
    SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]
)
SELECT [a].[Name] FROM [a]";

            var ss = ParseAndResolveNames<SelectStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var ct = ss.CommonTableExpression.EnumerateCommonTableSpecifications().ToArray();
            var ts = ct[0].Subquery.QueryExpression.EnumerateSourceTableReferences(false).ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = ss.QueryExpression.EnumerateQuerySpecifications().FirstOrDefault().ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        [TestMethod]
        public void SimpleCteWithTableAliasTest()
        {
            var sql =
@"WITH a AS
(
    SELECT * FROM Author
)
SELECT a.Name FROM a";

            var gt =
@"WITH [a] AS
(
    SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]
)
SELECT [a].[Name] FROM [a]";

            var ss = ParseAndResolveNames<SelectStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var ct = ss.CommonTableExpression.EnumerateCommonTableSpecifications().ToArray();
            var ts = ct[0].Subquery.QueryExpression.EnumerateSourceTableReferences(false).ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = ss.QueryExpression.EnumerateQuerySpecifications().FirstOrDefault().ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        [TestMethod]
        public void MultipleCteTest()
        {
            var sql =
@"WITH
    a AS (SELECT * FROM Author),
    b AS (SELECT * FROM Book)
SELECT Name, Title FROM a INNER JOIN b ON a.ID = b.ID";

            var gt =
@"WITH
    [a] AS (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]),
    [b] AS (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Book])
SELECT [a].[Name], [b].[Title] FROM [a] INNER JOIN [b] ON [a].[ID] = [b].[ID]";

            var ss = ParseAndResolveNames<SelectStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void RecursiveCteTest()
        {
            var sql =
@"WITH a AS
(
    SELECT * FROM Author
    UNION ALL
    SELECT * FROM a
)
SELECT Name FROM a";

            var gt =
@"WITH [a] AS
(
    SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]
    UNION ALL
    SELECT * FROM [a]
)
SELECT [a].[Name] FROM [a]";

            var ss = ParseAndResolveNames<SelectStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void RankingFunctionInCteTest()
        {
            var sql =
@"WITH q AS
(
    SELECT ROW_NUMBER() OVER (PARTITION BY FLOOR(ID) ORDER BY ID) AS rn, ID, Name
    FROM Author a
)
SELECT *
FROM q
WHERE rn <= 10";

            var gt =
@"WITH [q] AS
(
    SELECT ROW_NUMBER() OVER (PARTITION BY FLOOR([a].[ID]) ORDER BY [a].[ID]) AS [rn], [a].[ID], [a].[Name]
    FROM [Graywulf_Schema_Test].[dbo].[Author] [a]
)
SELECT *
FROM [q]
WHERE [q].[rn] <= 10";

            var ss = ParseAndResolveNames<SelectStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }
    }
}
