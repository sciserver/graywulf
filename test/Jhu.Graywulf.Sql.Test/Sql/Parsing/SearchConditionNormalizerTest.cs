using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.LogicalExpressions;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class SearchConditionNormalizerTest
    {
        private SchemaManager CreateSchemaManager()
        {
            var sm = new SqlServerSchemaManager();
            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);

            sm.Datasets[ds.Name] = ds;

            return sm;
        }

        private SelectStatement CreateSelect(string query)
        {
            SqlParser p = new SqlParser();
            var script = p.Execute<StatementBlock>(query);

            SqlNameResolver nr = new SqlNameResolver();
            nr.DefaultTableDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName;
            nr.DefaultFunctionDatasetName = Jhu.Graywulf.Test.Constants.CodeDatasetName;
            nr.SchemaManager = CreateSchemaManager();
            nr.Execute(script);

            return script.FindDescendantRecursive<SelectStatement>();
        }

        private string[] GetWhereClauses(string query)
        {
            var cn = new LogicalExpressions.SearchConditionNormalizer();

            var select = CreateSelect(query);
            var res = new List<string>();

            foreach (var qs in select.QueryExpression.EnumerateQuerySpecifications())
            {
                cn.CollectConditions(qs);

                // TODO use qs.SourceTableReferences ???
                foreach (var tr in qs.EnumerateSourceTableReferences(true))
                {
                    var where = cn.GenerateWherePredicatesSpecificToTable(tr);

                    if (where != null)
                    {
                        var cg = new SqlServerCodeGenerator();

                        cg.TableNameRendering = CodeGeneration.NameRendering.FullyQualified;
                        cg.ColumnNameRendering = CodeGeneration.NameRendering.IdentifierOnly;
                        cg.FunctionNameRendering = CodeGeneration.NameRendering.FullyQualified;

                        var sw = new StringWriter();
                        cg.Execute(sw, where);

                        res.Add(sw.ToString());
                    }
                    else
                    {
                        res.Add("");
                    }
                }
            }

            return res.ToArray();
        }

        // ---

        private IEnumerable<LogicalExpressions.Expression> EnumerateCnfTermsTestHelper(string query)
        {
            var select = CreateSelect(query);

            var scn = new LogicalExpressions.SearchConditionNormalizer();
            scn.CollectConditions(select.QueryExpression);

            var conditions = typeof(LogicalExpressions.SearchConditionNormalizer).GetField("conditions", BindingFlags.Instance | BindingFlags.NonPublic);
            var enumterms = typeof(LogicalExpressions.SearchConditionNormalizer).GetMethod("EnumerateCnfTerms", BindingFlags.Static | BindingFlags.NonPublic);

            return (IEnumerable<LogicalExpressions.Expression>)enumterms.Invoke(null, new object[] { ((List<LogicalExpressions.Expression>)conditions.GetValue(scn)).First() });
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

        private IEnumerable<LogicalExpressions.Expression> EnumerateCnfTermsSpecificToTableTestHelper(string query)
        {
            var select = CreateSelect(query);

            var scn = new LogicalExpressions.SearchConditionNormalizer();
            scn.CollectConditions(select.QueryExpression);

            var conditions = typeof(LogicalExpressions.SearchConditionNormalizer).GetField("conditions", BindingFlags.Instance | BindingFlags.NonPublic);
            var enumterms = typeof(LogicalExpressions.SearchConditionNormalizer).GetMethod("EnumerateCnfTermsSpecificToTable", BindingFlags.Static | BindingFlags.NonPublic);

            return (IEnumerable<LogicalExpressions.Expression>)enumterms.Invoke(null, new object[] { ((List<LogicalExpressions.Expression>)conditions.GetValue(scn)).FirstOrDefault(), select.QueryExpression.EnumerateSourceTableReferences(false).First(), null });
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

        private IEnumerable<LogicalExpressions.Expression> EnumerateCnfTermPredicatesTestHelper(string query)
        {
            var terms = EnumerateCnfTermsTestHelper(query);

            var enumpreds = typeof(LogicalExpressions.SearchConditionNormalizer).GetMethod("EnumerateCnfTermPredicates", BindingFlags.Static | BindingFlags.NonPublic);

            return (IEnumerable<LogicalExpressions.Expression>)enumpreds.Invoke(null, new object[] { terms.First() });
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
            Assert.AreEqual("([ID] = 6)", res[0]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_SingleTableMultiplePredicatesTest()
        {
            var sql = "SELECT ID FROM Book WHERE ID = 6 AND Title='x'";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 1);
            Assert.AreEqual("([ID] = 6 AND [Title]='x')", res[0]);
        }

        [TestMethod]
        public void GenerateWhereClauseSpecificToTable_SingleTableMultiplePredicates2Test()
        {
            var sql = "SELECT ID FROM Book WHERE ID = 6 AND 2 = 3";
            var res = GetWhereClauses(sql);

            Assert.IsTrue(res.Length == 1);
            Assert.AreEqual("([ID] = 6 AND 2 = 3)", res[0]);
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
            Assert.AreEqual("([ID] = 6)", res[0]);
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
            Assert.AreEqual("([ID] = 6)", res[0]);
            Assert.AreEqual("", res[1]);
            Assert.AreEqual("([ID] = 3)", res[2]);
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
            Assert.AreEqual("([ID] = 6)", res[0]);
            Assert.AreEqual("", res[1]);
            Assert.AreEqual("([ID] = 3)", res[2]);
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
            Assert.AreEqual("([ID] IN (3, 6) AND [ID] = 6)", res[0]);
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
            Assert.AreEqual("([ID] = 3)", res[0]);
            Assert.AreEqual("", res[1]);
        }
    }
}
