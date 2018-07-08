﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class SubqueryTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleSubqueryTest()
        {
            var sql = "SELECT Name FROM (SELECT Name FROM Author) a";
            var qs = Parse<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
            Assert.IsTrue(qs.ResolvedSourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
        }

        [TestMethod]
        public void SubqueryWithTableAliasTest()
        {
            var sql = "SELECT a.Name FROM (SELECT Name FROM Author) a";
            var qs = Parse<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
            Assert.IsTrue(qs.ResolvedSourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
        }

        [TestMethod]
        public void SimpleSubqueryWithCustomAliasTest()
        {
            var sql = "SELECT Name FROM (SELECT Name AS Name FROM Author) a";
            var qs = Parse<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void SimpleSubqueryWithMissingAliasTest()
        {
            var sql = "SELECT Name FROM (SELECT Name + Name FROM Author) a";
            var qs = Parse<QuerySpecification>(sql);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void SimpleSubqueryWithDuplicateAliasTest1()
        {
            var sql = "SELECT Name FROM (SELECT Name, Name FROM Author) a";
            var qs = Parse<QuerySpecification>(sql);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void SimpleSubqueryWithDuplicateAliasTest2()
        {
            var sql = "SELECT Name FROM (SELECT Name AS Name, Name AS Name FROM Author) a";
            var qs = Parse<QuerySpecification>(sql);
        }

        [TestMethod]
        public void SubqueryWithOrderByTest()
        {
            var sql = "SELECT Name FROM (SELECT TOP 10 Name FROM Author ORDER BY Name) a";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT TOP 10 [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[Name]) [a]", res);
        }

        [TestMethod]
        public void SubqueryWithColumnAliasTest()
        {
            var sql = "SELECT Name FROM (SELECT TOP 10 ID AS Name FROM Author ORDER BY Name) a";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT TOP 10 [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[Name]) [a]", res);
        }

        [TestMethod]
        public void SubqueryWithColumnAlias2Test()
        {
            var sql = "SELECT Col_1 AS TestCol FROM (SELECT 1 AS Col_1 FROM Author) a";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Col_1] AS [TestCol] FROM (SELECT 1 AS [Col_1] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
        }

        [TestMethod]
        public void RecursiveSubqueriesTest()
        {
            var sql = "SELECT Name FROM (SELECT Name FROM (SELECT * FROM Author) q) a";
            var qs = Parse<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT [q].[Name] FROM (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]) [q]) [a]", res);
            Assert.AreEqual(1, qs.ResolvedSourceTableReferences.Count);
            Assert.IsTrue(qs.ResolvedSourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
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

            var qs = Parse<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(2, qs.ResolvedSourceTableReferences.Count);
            Assert.IsTrue(qs.ResolvedSourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
            Assert.IsTrue(qs.ResolvedSourceTableReferences["b"].TableContext.HasFlag(TableContext.Subquery));
        }

        [TestMethod]
        public void SubSubQueriesTest()
        {
            var sql = @"SELECT a.Name FROM (SELECT * FROM (SELECT * FROM Author) b) a";

            var gt = @"SELECT [a].[Name] FROM (SELECT * FROM (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]) [b]) [a]";

            var qs = Parse<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(1, qs.ResolvedSourceTableReferences.Count);
            Assert.IsTrue(qs.ResolvedSourceTableReferences["a"].TableContext.HasFlag(TableContext.Subquery));
        }

        // Add SELECT * tests, function, sunquery in where etc.
    }
}
