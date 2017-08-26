using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlCodeGen;

namespace Jhu.Graywulf.SqlCodeGen.Test
{
    [TestClass]
    public class MySqlCodeGeneratorTest
    {
        // TODO: rewrite these to use SQL Server for name resolution but generate
        // SQL for MySQL

        private SchemaManager CreateSchemaManager()
        {
            var sm = new SqlServerSchemaManager();
            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);

            sm.Datasets[ds.Name] = ds;

            return sm;
        }

        private SelectStatement CreateSelect(string query)
        {
            var p = new SqlParser.SqlParser();
            var select = (SelectStatement)p.Execute(new SelectStatement(), query);

            SqlNameResolver nr = new SqlNameResolver();
            nr.DefaultTableDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName;
            nr.DefaultFunctionDatasetName = Jhu.Graywulf.Test.Constants.CodeDatasetName;
            nr.SchemaManager = CreateSchemaManager();
            nr.Execute(select);

            return select;
        }

        private string GenerateCode(string query, bool resolved)
        {
            var ss = CreateSelect(query);
            var w = new StringWriter();

            var cg = new MySql.MySqlCodeGenerator();
            cg.ResolveNames = resolved;
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
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID AND Book.ID = 6
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Author.ID = 3",
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
@"SELECT `Graywulf_Schema_Test`.`Book`.`Title` AS `Title`, `Graywulf_Schema_Test`.`Author`.`Name` AS `Name`
FROM `Graywulf_Schema_Test`.`Book`
INNER JOIN `Graywulf_Schema_Test`.`BookAuthor` ON `Graywulf_Schema_Test`.`BookAuthor`.`BookID` = `Graywulf_Schema_Test`.`Book`.`ID` AND `Graywulf_Schema_Test`.`Book`.`ID` = 6
INNER JOIN `Graywulf_Schema_Test`.`Author` ON `Graywulf_Schema_Test`.`Author`.`ID` = `Graywulf_Schema_Test`.`BookAuthor`.`AuthorID`
WHERE `Graywulf_Schema_Test`.`Author`.`ID` = 3",
                GenerateCode(sql, true));
        }

        [TestMethod]
        public void MultipleTableOccuranceTest()
        {
            var sql =
@"SELECT b1.Title, b2.Title
FROM Book b1, Book b2";

            var res = GenerateCode(sql, true);

            // *** TODO

            Assert.AreEqual(
@"SELECT `b1`.`Title` AS `b1_Title`, `b2`.`Title` AS `b2_Title`
FROM `Graywulf_Schema_Test`.`Book` `b1`, `Graywulf_Schema_Test`.`Book` `b2`", res);

        }

        private string[] GenerateMostRestrictiveTableQueryTestHelper(string sql, ColumnContext columnContext, int top)
        {
            var cg = new MySql.MySqlCodeGenerator();
            cg.ResolveNames = true;

            var ss = CreateSelect(sql);

            var res = new List<string>();

            foreach (var qs in ss.QueryExpression.EnumerateQuerySpecifications())
            {
                // TODO: use qs.SourceTableReferences
                foreach (var tr in qs.EnumerateSourceTableReferences(true))
                {
                    res.Add(cg.GenerateMostRestrictiveTableQuery(qs, tr, columnContext, top));
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

            var gt =
@"SELECT 
`b`.`ID`, `b`.`Title`
 FROM `Book`AS `b` 
WHERE `b`.`ID` = 1
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_MultipleAliasesTest()
        {
            var sql =
@"SELECT a.Title, b.ID
FROM Book a CROSS JOIN Book b
WHERE b.ID = 1 AND a.ID IN (3, 4)";

            var gta =
@"SELECT 
`a`.`ID`, `a`.`Title`
 FROM `Book`AS `a` 
WHERE `a`.`ID` IN (3, 4)
";

            var gtb =
@"SELECT 
`b`.`ID`
 FROM `Book`AS `b` 
WHERE `b`.`ID` = 1
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual(gta, res[0]);
            Assert.AreEqual(gtb, res[1]);
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

            var gta =
@"SELECT 
`ID`, `Title`
 FROM `Book`
WHERE `Graywulf_Schema_Test`.`Book`.`ID` IN (2, 3)
";

            var gtb =
@"SELECT 
`ID`, `Title`
 FROM `Book`
WHERE `Graywulf_Schema_Test`.`Book`.`ID` = 1
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(2, res.Length);
            Assert.AreEqual(gta, res[0]);
            Assert.AreEqual(gtb, res[1]);
        }

    }
}
