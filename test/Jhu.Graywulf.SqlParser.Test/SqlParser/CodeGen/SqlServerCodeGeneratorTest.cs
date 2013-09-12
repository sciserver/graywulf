using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser.SqlCodeGen;

namespace Jhu.Graywulf.SqlCodeGen.Test
{
    [TestClass]
    public class SqlServerCodeGeneratorTest
    {
        private SchemaManager CreateSchemaManager()
        {
            var sm = new SqlServerSchemaManager();
            var ds = new SqlServerDataset("Test", Jhu.Graywulf.Test.Constants.TestConnectionString);

            sm.Datasets[ds.Name] = ds;

            return sm;
        }

        private SelectStatement CreateSelect(string query)
        {
            var p = new SqlParser.SqlParser();
            var select = (SelectStatement)p.Execute(new SelectStatement(), query);

            SqlNameResolver nr = new SqlNameResolver();
            nr.DefaultTableDatasetName = "Test";
            nr.DefaultTableSchemaName = "dbo";
            nr.SchemaManager = CreateSchemaManager();
            nr.Execute(select);

            return select;
        }

        private string GenerateCode(string query, bool resolveAliases, bool resolveNames, bool substituteStars)
        {
            var ss = CreateSelect(query);
            var w = new StringWriter();

            var cg = new SqlServerCodeGenerator();
            cg.ResolveNames = resolveNames;
            cg.Execute(w, ss);

            return w.ToString();
        }

        private string[] GenerateMostRestrictiveTableQueryTestHelper(string sql, bool includePrimaryKey, int top)
        {
            var cg = new SqlServerCodeGenerator();
            cg.ResolveNames = true;

            var ss = CreateSelect(sql);

            var res = new List<string>();

            foreach (var qs in ss.EnumerateQuerySpecifications())
            {
                // TODO: use qs.SourceTableReferences
                foreach (var tr in qs.EnumerateSourceTableReferences(true))
                {
                    res.Add(cg.GenerateMostRestrictiveTableQuery(tr, includePrimaryKey, top));
                }
            }

            return res.ToArray();
        }

        [TestMethod]
        public void WithoutResolvedNamesTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID AND Book.ID = 6
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Author.ID = 3";

            Assert.AreEqual(sql, GenerateCode(sql, false, false, false));
        }

        [TestMethod]
        public void WithResolvedNamesTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID AND Book.ID = 6
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Author.ID = 3";

            Assert.AreEqual(
@"SELECT [Graywulf_Test].[dbo].[Book].[Title] AS [Title], [Graywulf_Test].[dbo].[Author].[Name] AS [Name]
FROM [Graywulf_Test].[dbo].[Book]
INNER JOIN [Graywulf_Test].[dbo].[BookAuthor] ON [Graywulf_Test].[dbo].[BookAuthor].[BookID] = [Graywulf_Test].[dbo].[Book].[ID] AND [Graywulf_Test].[dbo].[Book].[ID] = 6
INNER JOIN [Graywulf_Test].[dbo].[Author] ON [Graywulf_Test].[dbo].[Author].[ID] = [Graywulf_Test].[dbo].[BookAuthor].[AuthorID]
WHERE [Graywulf_Test].[dbo].[Author].[ID] = 3",
                GenerateCode(sql, false, true, false));
        }

        [TestMethod]
        public void MultipleTableOccuranceTest()
        {
            var sql =
@"SELECT b1.Title, b2.Title
FROM Book b1, Book b2
WHERE b1.ID = 1 AND b2.ID = 2";

            var res = GenerateCode(sql, false, true, false);

            Assert.AreEqual(
@"SELECT [b1].[Title] AS [b1_Title], [b2].[Title] AS [b2_Title]
FROM [Graywulf_Test].[dbo].[Book] [b1], [Graywulf_Test].[dbo].[Book] [b2]
WHERE [b1].[ID] = 1 AND [b2].[ID] = 2", res);

        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_SimpleTest()
        {
            var sql =
@"SELECT b.Title
FROM Book b
WHERE b.ID = 1";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, false, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("SELECT [b].[ID], [b].[Title] FROM [Graywulf_Test].[dbo].[Book] AS [b] WHERE [b].[ID] = 1", res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_MultipleAliasesTest()
        {
            var sql =
@"SELECT a.Title, b.ID
FROM Book a CROSS JOIN Book b
WHERE b.ID = 1 AND a.ID IN (3, 4)";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, false, 0);

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("SELECT [a].[ID], [a].[Title] FROM [Graywulf_Test].[dbo].[Book] AS [a] WHERE [a].[ID] IN (3, 4)", res[0]);
            Assert.AreEqual("SELECT [b].[ID] FROM [Graywulf_Test].[dbo].[Book] AS [b] WHERE [b].[ID] = 1", res[1]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_UnionTest()
        {
            var sql =
@"SELECT Title, ID
FROM Book
WHERE ID IN (2, 3)
UNION
SELECT Title, ID + 1
FROM Book
WHERE ID = 1";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, false, 0);

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Book].[ID], [Graywulf_Test].[dbo].[Book].[Title] FROM [Graywulf_Test].[dbo].[Book] WHERE [Graywulf_Test].[dbo].[Book].[ID] IN (2, 3)", res[0]);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Book].[ID], [Graywulf_Test].[dbo].[Book].[Title] FROM [Graywulf_Test].[dbo].[Book] WHERE [Graywulf_Test].[dbo].[Book].[ID] = 1", res[1]);
        }

    }
}
