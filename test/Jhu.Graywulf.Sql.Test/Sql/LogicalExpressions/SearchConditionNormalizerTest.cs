using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    [TestClass]
    public class SearchConditionNormalizerTest : LogicalExpressionsTestBase
    {
        private IEnumerable<LogicalExpressions.ExpressionTreeNode> EnumerateCnfTermsTestHelper(string query)
        {
            var select = CreateSelect(query);

            var scn = new LogicalExpressions.SearchConditionNormalizer();
            scn.CollectConditions(select.QueryExpression);

            var conditions = typeof(LogicalExpressions.SearchConditionNormalizer).GetField("conditions", BindingFlags.Instance | BindingFlags.NonPublic);
            var enumterms = typeof(LogicalExpressions.SearchConditionNormalizer).GetMethod("EnumerateCnfTerms", BindingFlags.Static | BindingFlags.NonPublic);

            return (IEnumerable<LogicalExpressions.ExpressionTreeNode>)enumterms.Invoke(null, new object[] { ((List<LogicalExpressions.ExpressionTreeNode>)conditions.GetValue(scn)).First() });
        }

        // ---

        [TestMethod]
        public void SimpleWherePredicateTest()
        {
            var sql = "SELECT * FROM Author WHERE Name = 'test'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Author.Name = 'test')", w[0].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Author.Name = 'test')", w[0].Value);
        }

        [TestMethod]
        public void SubqueryWherePredicateTest1()
        {
            // Where clauses are always taken from the query which directly references the table.
            // Outer queries referencing the table via a subquery don't count.

            var sql = "SELECT * FROM (SELECT * FROM Author) a WHERE Name = 'test'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);
        }

        [TestMethod]
        public void SubqueryWherePredicateTest2()
        {
            var sql = "SELECT * FROM (SELECT * FROM Author WHERE Name = 'test') a";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Author.Name = 'test')", w[0].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Author.Name = 'test')", w[0].Value);
        }

        [TestMethod]
        public void SubqueryWherePredicateTest3()
        {
            var sql =
@"SELECT * 
FROM (SELECT * FROM Author WHERE Name = 'test') a
CROSS JOIN (SELECT * FROM Author WHERE Name = 'test2') b";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual("(Author.Name = 'test')", w[0].Value);
            Assert.AreEqual("(Author.Name = 'test2')", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Author.Name = 'test2') OR (Author.Name = 'test')", w[0].Value);
        }

        [TestMethod]
        public void MultipleSelectWherePredicateTest()
        {
            var sql =
@"SELECT * FROM Author WHERE Name = 'test1' OR Name = 'test3'
SELECT * FROM Author WHERE Name = 'test2' AND Name = 'test4'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual("(Author.Name = 'test1' OR Author.Name = 'test3')", w[0].Value);
            Assert.AreEqual("(Author.Name = 'test2' AND Author.Name = 'test4')", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Author.Name = 'test2' AND Author.Name = 'test4') OR (Author.Name = 'test1' OR Author.Name = 'test3')", w[0].Value);
        }

        [TestMethod]
        public void UnionWherePredicateTest()
        {
            var sql =
@"SELECT * FROM Author WHERE Name = 'test1' OR Name = 'test3'
UNION
SELECT * FROM Author WHERE Name = 'test2' AND Name = 'test4'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual("(Author.Name = 'test1' OR Author.Name = 'test3')", w[0].Value);
            Assert.AreEqual("(Author.Name = 'test2' AND Author.Name = 'test4')", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Author.Name = 'test2' AND Author.Name = 'test4') OR (Author.Name = 'test1' OR Author.Name = 'test3')", w[0].Value);
        }

        [TestMethod]
        public void CteWherePredicateTest1()
        {
            var sql =
@"
WITH a AS
(SELECT * FROM Author WHERE Name = 'test1' OR Name = 'test3'),
b AS
(SELECT * FROM Author WHERE Name = 'test2' AND Name = 'test4')
SELECT * FROM a CROSS JOIN b";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual("(Author.Name = 'test1' OR Author.Name = 'test3')", w[0].Value);
            Assert.AreEqual("(Author.Name = 'test2' AND Author.Name = 'test4')", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Author.Name = 'test2' AND Author.Name = 'test4') OR (Author.Name = 'test1' OR Author.Name = 'test3')", w[0].Value);
        }

        [TestMethod]
        public void CteWherePredicateTest2()
        {
            var sql =
@"
WITH a AS
(SELECT * FROM Author WHERE Name = 'test1' OR Name = 'test3'),
b AS
(SELECT * FROM Author WHERE Name = 'test2' AND Name = 'test4')
SELECT * FROM a CROSS JOIN b CROSS JOIN Author c
WHERE c.Name = 'test5'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(3, w.Count);
            Assert.AreEqual("(Author.Name = 'test1' OR Author.Name = 'test3')", w[0].Value);
            Assert.AreEqual("(Author.Name = 'test2' AND Author.Name = 'test4')", w[1].Value);
            Assert.AreEqual("(c.Name = 'test5')", w[2].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(c.Name = 'test5') OR (Author.Name = 'test2' AND Author.Name = 'test4') OR (Author.Name = 'test1' OR Author.Name = 'test3')", w[0].Value);
        }

        [TestMethod]
        public void NoPredicateTest1()
        {
            var sql = "SELECT name FROM Author";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);
        }

        [TestMethod]
        public void NoPredicateTest2()
        {
            var sql =
@"SELECT Title FROM Book
SELECT Title FROM Book WHERE Year > 2000";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual(null, w[0]);
            Assert.AreEqual("(Book.Year > 2000)", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);
        }

        [TestMethod]
        public void EnumerateCnfTermsTest()
        {
            string sql;

            sql = "SELECT ID FROM Book WHERE ID = 12";
            Assert.AreEqual(1, EnumerateCnfTermsTestHelper(sql).Count());

            sql = "SELECT ID FROM Book WHERE ID = 12 OR ID = 16";
            Assert.AreEqual(1, EnumerateCnfTermsTestHelper(sql).Count());

            sql = "SELECT ID FROM Book WHERE ID = 12 OR ID = 16 AND Title = ''";
            Assert.AreEqual(2, EnumerateCnfTermsTestHelper(sql).Count());

            sql = "SELECT * FROM Book, Author WHERE Book.ID = 12 OR Author.ID = 2";
            Assert.AreEqual(1, EnumerateCnfTermsTestHelper(sql).Count());
        }

        // ---

        private IEnumerable<LogicalExpressions.ExpressionTreeNode> EnumerateCnfTermsSpecificToTableTestHelper(string query)
        {
            var select = CreateSelect(query);

            var scn = new LogicalExpressions.SearchConditionNormalizer();
            scn.CollectConditions(select.QueryExpression);

            var conditions = typeof(LogicalExpressions.SearchConditionNormalizer).GetField("conditions", BindingFlags.Instance | BindingFlags.NonPublic);
            var enumterms = typeof(LogicalExpressions.SearchConditionNormalizer).GetMethod("EnumerateCnfTermsSpecificToTable", BindingFlags.Static | BindingFlags.NonPublic);

            return (IEnumerable<LogicalExpressions.ExpressionTreeNode>)enumterms.Invoke(null, new object[] { ((List<LogicalExpressions.ExpressionTreeNode>)conditions.GetValue(scn)).FirstOrDefault(), select.QueryExpression.FirstQuerySpecification.SourceTableReferences.Values.First(), null });
        }

        [TestMethod]
        public void EnumerateCnfTermsSpecificToTableTest()
        {
            string sql;

            sql = "SELECT * FROM Book WHERE ID = 1";
            Assert.AreEqual(1, EnumerateCnfTermsSpecificToTableTestHelper(sql).Count());

            sql = "SELECT * FROM Book WHERE ID = 1 OR ID = 2";
            Assert.AreEqual(1, EnumerateCnfTermsSpecificToTableTestHelper(sql).Count());

            sql = "SELECT * FROM Book WHERE ID = 1 OR ID = 2 AND Title = ''";
            Assert.AreEqual(2, EnumerateCnfTermsSpecificToTableTestHelper(sql).Count());

            sql = "SELECT * FROM Book, Author WHERE Book.ID = 1 OR Author.ID = 2";
            Assert.AreEqual(0, EnumerateCnfTermsSpecificToTableTestHelper(sql).Count());

            sql = "SELECT * FROM Book, Author WHERE Book.ID = 1 AND Author.ID = 2";
            Assert.AreEqual(1, EnumerateCnfTermsSpecificToTableTestHelper(sql).Count());
        }

        // ---

        private IEnumerable<LogicalExpressions.ExpressionTreeNode> EnumerateCnfTermPredicatesTestHelper(string query)
        {
            var terms = EnumerateCnfTermsTestHelper(query);

            var enumpreds = typeof(LogicalExpressions.SearchConditionNormalizer).GetMethod("EnumerateCnfTermPredicates", BindingFlags.Static | BindingFlags.NonPublic);

            return (IEnumerable<LogicalExpressions.ExpressionTreeNode>)enumpreds.Invoke(null, new object[] { terms.First() });
        }

        [TestMethod]
        public void EnumerateCnfTermPredicatesTest()
        {
            string sql;

            sql = "SELECT ID FROM Book WHERE ID = 12";
            Assert.AreEqual(1, EnumerateCnfTermPredicatesTestHelper(sql).Count());

            sql = "SELECT ID FROM Book WHERE ID = 12 OR ID = 16";
            Assert.AreEqual(2, EnumerateCnfTermPredicatesTestHelper(sql).Count());

            sql = "SELECT ID FROM Book WHERE ID = 12 OR ID = 16 AND Title = ''";
            Assert.AreEqual(2, EnumerateCnfTermPredicatesTestHelper(sql).Count());
        }

        // ---

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_NoPredicateTest()
        {
            var sql = "SELECT ID FROM Book";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 1);
            Assert.AreEqual("", res[0]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_SingleTableSinglePredicateTest()
        {
            // --- simplest test
            var sql = "SELECT ID FROM Book WHERE ID = 6";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 1);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Book].[ID] = 6)", res[0]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_SingleTableMultiplePredicatesTest()
        {
            var sql = "SELECT ID FROM Book WHERE ID = 6 AND Title='x'";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 1);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Book].[ID] = 6 AND [Graywulf_Schema_Test].[dbo].[Book].[Title]='x')", res[0]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_SingleTableMultiplePredicates2Test()
        {
            var sql = "SELECT ID FROM Book WHERE ID = 6 AND 2 = 3";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 1);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Book].[ID] = 6 AND 2 = 3)", res[0]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_JoinedTablesNoPredicatesTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 3);
            Assert.AreEqual("", res[0]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_JoinedTablesWherePredicateTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Book.ID = 6";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 3);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Book].[ID] = 6)", res[0]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_JoinedTablesWherePredicatesTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Book.ID = 6 AND Author.ID = 3";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 3);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Book].[ID] = 6)", res[0]);
            Assert.AreEqual("", res[1]);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Author].[ID] = 3)", res[2]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_JoinedTablesOnPredicatesTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID AND Book.ID = 6
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Author.ID = 3";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 3);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Book].[ID] = 6)", res[0]);
            Assert.AreEqual("", res[1]);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Author].[ID] = 3)", res[2]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_JoinedTablesOnPredicates2Test()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID AND Book.ID = 6
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Book.ID IN (3, 6)";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 3);
            Assert.AreEqual("([Graywulf_Schema_Test].[dbo].[Book].[ID] IN (3, 6) AND [Graywulf_Schema_Test].[dbo].[Book].[ID] = 6)", res[0]);
            Assert.AreEqual("", res[1]);
            Assert.AreEqual("", res[2]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_JoinedTablesOnPredicates3Test()
        {
            var sql =
@"SELECT a.Title, b.ID
FROM Book a
INNER JOIN Book b ON a.ID = b.ID
WHERE a.ID = 3";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 2);
            Assert.AreEqual("([a].[ID] = 3)", res[0]);
            Assert.AreEqual("", res[1]);
        }
    }
}
