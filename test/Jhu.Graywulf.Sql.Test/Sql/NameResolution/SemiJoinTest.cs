using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class SemiJoinTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void InSemiJoinInWhereTest()
        {
            #region Simple semijoin constructs

            var sql =
@"SELECT Name 
FROM Author
WHERE ID IN (SELECT ID FROM Book)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] IN (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void InSemiJoinInJoinOnTest()
        {
            var sql =
@"SELECT Name 
FROM Author
INNER JOIN Book
ON Author.ID IN (SELECT 1)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
INNER JOIN [Graywulf_Schema_Test].[dbo].[Book]
ON [Graywulf_Schema_Test].[dbo].[Author].[ID] IN (SELECT 1)";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void AnySemiJoinInWhereTest()
        {
            var sql =
@"SELECT Name 
FROM Author
WHERE ID = ANY (SELECT ID FROM Book)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = ANY (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void AnySemiJoinInJoinOnTest()
        {
            var sql =
@"SELECT Name 
FROM Author
INNER JOIN Book
ON Author.ID = ANY (SELECT 1)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
INNER JOIN [Graywulf_Schema_Test].[dbo].[Book]
ON [Graywulf_Schema_Test].[dbo].[Author].[ID] = ANY (SELECT 1)";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void AllSemiJoinInWhereTest()
        {
            var sql =
@"SELECT Name 
FROM Author
WHERE ID = ALL (SELECT ID FROM Book)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = ALL (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void SomeSemiJoinInWhereTest()
        {
            var sql =
@"SELECT Name 
FROM Author
WHERE ID = SOME (SELECT ID FROM Book)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = SOME (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void ExistsSemiJoinInWhereTest()
        {
            var sql =
@"SELECT Name 
FROM Author
WHERE EXISTS (SELECT ID FROM Book)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE EXISTS (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void ExistsSemiJoinInJoinOnTest()
        {
            var sql =
@"SELECT Name 
FROM Author
INNER JOIN Book ON EXISTS (SELECT ID FROM Book)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
INNER JOIN [Graywulf_Schema_Test].[dbo].[Book] ON EXISTS (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void ExactSemiJoinInWhereTest()
        {
            var sql =
@"SELECT Name 
FROM Author
WHERE ID < (SELECT ID FROM Book)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] < (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);

            sql =
@"SELECT Name 
FROM Author
WHERE (SELECT ID FROM Book) > ID";

            gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book]) > [Graywulf_Schema_Test].[dbo].[Author].[ID]";

            ss = ParseAndResolveNames<SelectStatement>(sql);
            qs = ss.QueryExpression.FirstQuerySpecification;
            res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void ExactSemiJoinInJoinOnTest()
        {
            var sql =
@"SELECT Name 
FROM Author
INNER JOIN Book
ON Author.ID < (SELECT 1)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
INNER JOIN [Graywulf_Schema_Test].[dbo].[Book]
ON [Graywulf_Schema_Test].[dbo].[Author].[ID] < (SELECT 1)";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);

            sql =
@"SELECT Name 
FROM Author
INNER JOIN Book
ON (SELECT 1) < Author.ID";

            gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
INNER JOIN [Graywulf_Schema_Test].[dbo].[Book]
ON (SELECT 1) < [Graywulf_Schema_Test].[dbo].[Author].[ID]";

            ss = ParseAndResolveNames<SelectStatement>(sql);
            qs = ss.QueryExpression.FirstQuerySpecification;
            res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        #endregion
        #region Subquery column name visibility

        [TestMethod]
        public void ReferenceTableOutsideInWhereTest1()
        {
            // Here the ID column in the subquery is resolved against the local source tables first

            var sql =
@"SELECT Name 
FROM Author a
WHERE EXISTS 
    (SELECT ID
     FROM Book
     WHERE ID = a.ID)";

            var gt =
@"SELECT [a].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author] [a]
WHERE EXISTS 
    (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID]
     FROM [Graywulf_Schema_Test].[dbo].[Book]
     WHERE [Graywulf_Schema_Test].[dbo].[Book].[ID] = [a].[ID])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void ReferenceTableOutsideInWhereTest2()
        {
            var sql =
@"SELECT Name 
FROM Author
WHERE EXISTS 
    (SELECT a.ID
     FROM Author a, Book b
     WHERE a.ID = Author.ID)";

            var gt =
@"SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] 
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE EXISTS 
    (SELECT [a].[ID]
     FROM [Graywulf_Schema_Test].[dbo].[Author] [a], [Graywulf_Schema_Test].[dbo].[Book] [b]
     WHERE [a].[ID] = [Graywulf_Schema_Test].[dbo].[Author].[ID])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void ReferenceTableOutsideInSelectTest()
        {
            // Here the ID column in the subquery is resolved against the local source tables first

            var sql =
@"SELECT (SELECT ID
     FROM Book
     WHERE ID = a.ID)
FROM Author a";

            var gt =
@"SELECT (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID]
     FROM [Graywulf_Schema_Test].[dbo].[Book]
     WHERE [Graywulf_Schema_Test].[dbo].[Book].[ID] = [a].[ID])
FROM [Graywulf_Schema_Test].[dbo].[Author] [a]";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void ReferenceTableOutsideInJoinOnTest()
        {
            // Here the ID column in the subquery is resolved against the local source tables first

            var sql =
@"SELECT Name
FROM Author a
INNER JOIN Book b ON EXISTS
	(SELECT ID FROM Author
	 WHERE ID = a.ID)";

            var gt =
@"SELECT [a].[Name]
FROM [Graywulf_Schema_Test].[dbo].[Author] [a]
INNER JOIN [Graywulf_Schema_Test].[dbo].[Book] [b] ON EXISTS
	(SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] FROM [Graywulf_Schema_Test].[dbo].[Author]
	 WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = [a].[ID])";

            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var qs = ss.QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual(gt, res);
        }

        #endregion
    }
}
