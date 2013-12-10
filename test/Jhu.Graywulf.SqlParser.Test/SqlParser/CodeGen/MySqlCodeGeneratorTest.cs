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
    public class MySqlCodeGeneratorTest
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
            nr.DefaultFunctionDatasetName = "Code";
            nr.SchemaManager = CreateSchemaManager();
            nr.Execute(select);

            return select;
        }

        private string GenerateCode(string query, bool resolved)
        {
            var ss = CreateSelect(query);
            var w = new StringWriter();

            var cg = new MySqlCodeGenerator();
            cg.ResolveNames = resolved;
            cg.QuoteIdentifiers = true;
            cg.Execute(w, ss);

            return w.ToString();
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

            Assert.AreEqual(
@"SELECT `Title`, `Name`
FROM `Book`
INNER JOIN `BookAuthor` ON `BookAuthor`.`BookID` = `Book`.`ID` AND `Book`.`ID` = 6
INNER JOIN `Author` ON `Author`.`ID` = `BookAuthor`.`AuthorID`
WHERE `Author`.`ID` = 3",
                        GenerateCode(sql, false));
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
@"SELECT `Book`.`Title` AS `Title`, `Author`.`Name` AS `Name`
FROM `Book`
INNER JOIN `BookAuthor` ON `BookAuthor`.`BookID` = `Book`.`ID` AND `Book`.`ID` = 6
INNER JOIN `Author` ON `Author`.`ID` = `BookAuthor`.`AuthorID`
WHERE `Author`.`ID` = 3",
                GenerateCode(sql, true));
        }

        [TestMethod]
        public void MultipleTableOccuranceTest()
        {
            var sql =
@"SELECT b1.Title, b2.Title
FROM Book b1, Book b2";

            var res = GenerateCode(sql, true);

            Assert.AreEqual(
@"SELECT `b1`.`Title` AS `b1_Title`, `b2`.`Title` AS `b2_Title`
FROM `Book` `b1`, `Book` `b2`", res);

        }

        private string[] GenerateMostRestrictiveTableQueryTestHelper(string sql, bool includePrimaryKey, int top)
        {
            var cg = new MySqlCodeGenerator();
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
        public void GenerateMostRestrictiveTableQuery_SimpleTest()
        {
            var sql =
@"SELECT b.Title
FROM Book b
WHERE b.ID = 1";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, false, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("SELECT `ID`, `Title` FROM `Book` `b` WHERE `b`.`ID` = 1", res[0]);
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
            Assert.AreEqual("SELECT `ID`, `Title` FROM `Book` `a` WHERE `a`.`ID` IN (3, 4)", res[0]);
            Assert.AreEqual("SELECT `ID` FROM `Book` `b` WHERE `b`.`ID` = 1", res[1]);
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
            Assert.AreEqual("SELECT `ID`, `Title` FROM `Book` WHERE `Book`.`ID` IN (2, 3)", res[0]);
            Assert.AreEqual("SELECT `ID`, `Title` FROM `Book` WHERE `Book`.`ID` = 1", res[1]);
        }

    }
}
