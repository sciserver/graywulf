using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryTraversal;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class SubqueryTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleSubqueryTest()
        {
            var sql = "SELECT Name FROM (SELECT Name FROM Author) a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
            Assert.IsTrue(qs.SourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
        }

        [TestMethod]
        public void SubqueryWithTableAliasTest()
        {
            var sql = "SELECT a.Name FROM (SELECT Name FROM Author) a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
            Assert.IsTrue(qs.SourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
        }

        [TestMethod]
        public void SimpleSubqueryWithCustomAliasTest()
        {
            var sql = "SELECT Name FROM (SELECT Name AS Name FROM Author) a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void SimpleSubqueryWithMissingAliasTest()
        {
            var sql = "SELECT Name FROM (SELECT Name + Name FROM Author) a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void SimpleSubqueryWithDuplicateAliasTest1()
        {
            var sql = "SELECT Name FROM (SELECT Name, Name FROM Author) a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void SimpleSubqueryWithDuplicateAliasTest2()
        {
            var sql = "SELECT Name FROM (SELECT Name AS Name, Name AS Name FROM Author) a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
        }

        [TestMethod]
        public void SubqueryWithOrderByTest()
        {
            var sql = "SELECT Name FROM (SELECT TOP 10 Name FROM Author ORDER BY Name) a";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT TOP 10 [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[Name]) [a]", res);
        }

        [TestMethod]
        public void SubqueryWithColumnAliasTest()
        {
            var sql = "SELECT Name FROM (SELECT TOP 10 ID AS Name FROM Author ORDER BY Name) a";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT TOP 10 [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[Name]) [a]", res);
        }

        [TestMethod]
        public void SubqueryWithColumnAlias2Test()
        {
            var sql = "SELECT Col_1 AS TestCol FROM (SELECT 1 AS Col_1 FROM Author) a";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Col_1] AS [TestCol] FROM (SELECT 1 AS [Col_1] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
        }

        [TestMethod]
        public void RecursiveSubqueriesTest()
        {
            var sql = "SELECT Name FROM (SELECT Name FROM (SELECT * FROM Author) q) a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT [q].[Name] FROM (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]) [q]) [a]", res);
            Assert.AreEqual(1, qs.SourceTableReferences.Count);
            Assert.IsTrue(qs.SourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
        }

        [TestMethod]
        public void MultipleSubqueriesTest1()
        {
            var sql =
@"SELECT a.Name, b.Title
FROM (SELECT * FROM Author) a
INNER JOIN (SELECT * FROM Book) b
    ON a.ID = b.ID";

            var gt =
@"SELECT [a].[Name], [b].[Title]
FROM (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]
INNER JOIN (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Book]) [b]
    ON [a].[ID] = [b].[ID]";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(2, qs.SourceTableReferences.Count);
            Assert.IsTrue(qs.SourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
            Assert.IsTrue(qs.SourceTableReferences["b"].TableContext.HasFlag(TableContext.Subquery));
        }

        [TestMethod]
        public void SubSubQueriesTest()
        {
            var sql = @"SELECT a.Name FROM (SELECT * FROM (SELECT * FROM Author) b) a";
            var gt = @"SELECT [a].[Name] FROM (SELECT * FROM (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]) [b]) [a]";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(1, qs.SourceTableReferences.Count);
            Assert.IsTrue(qs.SourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
        }

        #region Subqueries in expressions

        [TestMethod]
        public void SubqueryInSelectListTest()
        {
            var sql = "SELECT (SELECT TOP 1 ID FROM Author)";
            var gt = "SELECT (SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Author].[ID] FROM [Graywulf_Schema_Test].[dbo].[Author])";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void SubqueryInPredicateTest()
        {
            var sql = "SELECT * FROM Author WHERE ID = (SELECT 1)";
            var gt = "SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = (SELECT 1)";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        #endregion

        // Add SELECT * tests, function, subquery in where etc.


    }
}
