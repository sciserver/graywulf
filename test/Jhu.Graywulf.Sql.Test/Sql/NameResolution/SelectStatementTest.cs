using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class SelectStatementTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleQueryTest()
        {
            var sql = "SELECT Name FROM Author";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        [TestMethod]
        public void SimpleQueryWithJoinTest()
        {
            var sql = "SELECT Name FROM Author INNER JOIN Book ON Author.ID = 5";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.From | ColumnContext.PrimaryKey, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] INNER JOIN [Graywulf_Schema_Test].[dbo].[Book] ON [Graywulf_Schema_Test].[dbo].[Author].[ID] = 5", res);
        }

        [TestMethod]
        public void SimpleQueryWithWhereTest()
        {
            var sql = "SELECT Name FROM Author WHERE ID = 1";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = 1", res);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.Where | ColumnContext.PrimaryKey, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        [TestMethod]
        public void SimpleQueryWithGroupByTest()
        {
            var sql = "SELECT ID, MAX(Name) FROM Author GROUP BY ID";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.GroupBy | ColumnContext.PrimaryKey, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual("ID", cs[0].ColumnName);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], MAX([Graywulf_Schema_Test].[dbo].[Author].[Name]) FROM [Graywulf_Schema_Test].[dbo].[Author] GROUP BY [Graywulf_Schema_Test].[dbo].[Author].[ID]", res);
        }

        [TestMethod]
        public void SimpleQueryWithHavingTest()
        {
            var sql = "SELECT ID, MAX(Name) FROM Author GROUP BY ID HAVING MAX(Name) > 5";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.GroupBy | ColumnContext.PrimaryKey, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.Having, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual("ID", cs[0].ColumnName);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], MAX([Graywulf_Schema_Test].[dbo].[Author].[Name]) FROM [Graywulf_Schema_Test].[dbo].[Author] GROUP BY [Graywulf_Schema_Test].[dbo].[Author].[ID] HAVING MAX([Graywulf_Schema_Test].[dbo].[Author].[Name]) > 5", res);
        }

        [TestMethod]
        public void SimpleTableAliasQueryTest()
        {
            var sql = "SELECT Name FROM Author a";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
        }

        [TestMethod]
        public void SimpleSelectWithTableNameQueryTest()
        {
            var sql = "SELECT Author.Name FROM Author";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void TwoTableAliasQueryTest()
        {
            var sql = "SELECT a.Name, b.Name FROM Author a, Author b";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name], [b].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a], [Graywulf_Schema_Test].[dbo].[Author] [b]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
            Assert.AreEqual("Author", ts[1].DatabaseObjectName);
            Assert.AreEqual("b", ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
        }

        [TestMethod]
        public void TwoTableAliasQueryTest2()
        {
            var sql = "SELECT a.ID, b.ID FROM Author a, Book b";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID], [b].[ID] FROM [Graywulf_Schema_Test].[dbo].[Author] [a], [Graywulf_Schema_Test].[dbo].[Book] [b]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
            Assert.AreEqual("Book", ts[1].DatabaseObjectName);
            Assert.AreEqual("b", ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
        }

        [TestMethod]
        public void TwoTableReferenceTest()
        {
            var sql = "SELECT Author.ID, Book.ID FROM Author, Book";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Author], [Graywulf_Schema_Test].[dbo].[Book]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
            Assert.AreEqual("Book", ts[1].DatabaseObjectName);
            Assert.AreEqual(null, ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);

        }

        [TestMethod]
        public void AmbigousColumnNameTest()
        {
            var sql = "SELECT Name FROM Author a, Author b";

            try
            {
                var qs = ParseAndResolveNames<QuerySpecification>(sql);
                Assert.Fail();
            }
            catch (NameResolverException)
            {
            }
        }

        [TestMethod]
        public void AmbigousColumnNameTest2()
        {
            var sql = "SELECT ID FROM Author a, Book b";

            try
            {
                var qs = ParseAndResolveNames<QuerySpecification>(sql);
                Assert.Fail();
            }
            catch (NameResolverException)
            {
            }
        }

        [TestMethod]
        public void AmbigousTableNameTest()
        {
            var sql = "SELECT Name FROM Author, Author";

            try
            {
                var qs = ParseAndResolveNames<QuerySpecification>(sql);
                Assert.Fail();
            }
            catch (NameResolverException)
            {
            }
        }

        [TestMethod]
        public void AmbigousTableNameTest2()
        {
            var sql = "SELECT Name FROM Test:Author, Author";

            try
            {
                var qs = ParseAndResolveNames<QuerySpecification>(sql);
                Assert.Fail();
            }
            catch (NameResolverException)
            {
            }
        }

        [TestMethod]
        public void AmbigousTableNameTest3()
        {
            var sql = "SELECT Name FROM dbo.Author, Author";

            try
            {
                var qs = ParseAndResolveNames<QuerySpecification>(sql);
                Assert.Fail();
            }
            catch (NameResolverException)
            {
            }
        }

        [TestMethod]
        public void AmbigousTableNameTest4()
        {
            var sql = "SELECT Name FROM dbo.Author, test.Author";

            try
            {
                var qs = ParseAndResolveNames<QuerySpecification>(sql);
                Assert.Fail();
            }
            catch (NameResolverException)
            {
            }
        }

        [TestMethod]
        public void SimpleStarQueryTest()
        {
            var sql = "SELECT * FROM Author";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            Assert.AreEqual(2, qs.ResultsTableReference.ColumnReferences.Count);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void AliasStarQueryTest()
        {
            var sql = "SELECT * FROM Author AS a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);
            Assert.AreEqual(2, qs.ResultsTableReference.ColumnReferences.Count);           
            Assert.AreEqual("SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a]", res);

            sql = "SELECT a.* FROM Author AS a";
            qs = ParseAndResolveNames<QuerySpecification>(sql);
            res = GenerateCode(qs);
            Assert.AreEqual(2, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("SELECT [a].* FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a]", res);
        }

        [TestMethod]
        public void TwoTableStarQueryTest()
        {
            var sql = "SELECT * FROM Author CROSS JOIN Book";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            Assert.AreEqual(5, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[0].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[1].ColumnName);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[2].ColumnName);
            Assert.AreEqual("Title", qs.ResultsTableReference.ColumnReferences[3].ColumnName);
            Assert.AreEqual("Year", qs.ResultsTableReference.ColumnReferences[4].ColumnName);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Book]", res);
        }

        [TestMethod]
        public void AliasTwoTableStarQueryTest()
        {
            var sql = "SELECT * FROM Author AS a CROSS JOIN Book AS b";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Book] AS [b]", res);

            Assert.AreEqual(5, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[0].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[1].ColumnName);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[2].ColumnName);
            Assert.AreEqual("Title", qs.ResultsTableReference.ColumnReferences[3].ColumnName);
            Assert.AreEqual("Year", qs.ResultsTableReference.ColumnReferences[4].ColumnName);
        }

        [TestMethod]
        public void AliasTwoTableStarQueryTest2()
        {
            var sql = "SELECT a.*, b.* FROM Author AS a CROSS JOIN Book AS b";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [a].*, [b].* FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Book] AS [b]", res);

            Assert.AreEqual(5, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[0].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[1].ColumnName);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[2].ColumnName);
            Assert.AreEqual("Title", qs.ResultsTableReference.ColumnReferences[3].ColumnName);
            Assert.AreEqual("Year", qs.ResultsTableReference.ColumnReferences[4].ColumnName);

        }

        [TestMethod]
        public void SameTableNameTest()
        {
            var sql = "SELECT * FROM dbo.Author a CROSS JOIN test.Author b";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(4, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[0].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[1].ColumnName);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[2].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[3].ColumnName);
            
            Assert.AreEqual("SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[test].[Author] [b]", res);
        }

        [TestMethod]
        public void SameTableNameTest2()
        {
            var sql = "SELECT a.*, b.* FROM dbo.Author a CROSS JOIN test.Author b";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(4, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[0].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[1].ColumnName);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[2].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[3].ColumnName);

            Assert.AreEqual("SELECT [a].*, [b].* FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[test].[Author] [b]", res);
        }

        [TestMethod]
        public void SameTableNameTest4()
        {
            var sql = "SELECT b.*, a.* FROM dbo.Author a CROSS JOIN test.Author b";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var res = GenerateCode(qs);

            Assert.AreEqual(4, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[0].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[1].ColumnName);
            Assert.AreEqual("ID", qs.ResultsTableReference.ColumnReferences[2].ColumnName);
            Assert.AreEqual("Name", qs.ResultsTableReference.ColumnReferences[3].ColumnName);
            

            Assert.AreEqual("SELECT [b].*, [a].* FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[test].[Author] [b]", res);
        }

        [TestMethod]
        public void SimpleOrderByTest()
        {
            var sql = "SELECT ID FROM Author ORDER BY Name";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void SelectStarSubqueryTest()
        {
            var sql = "SELECT Name FROM (SELECT * FROM Author) a";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] FROM (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
        }

        [TestMethod]
        public void SelectStarMultipleSubqueryTest()
        {
            var sql =
@"SELECT a.Name, b.Name
FROM (SELECT * FROM Author) a
CROSS JOIN (SELECT * FROM Author) b";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual(
@"SELECT [a].[Name], [b].[Name]
FROM (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]
CROSS JOIN (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author]) [b]", res);
        }

        [TestMethod]
        public void MultipleSelectStarTest()
        {
            var sql = "SELECT a.*, b.* FROM Author a CROSS JOIN Author b";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].*, [b].* FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Author] [b]", res);
        }

        [TestMethod]
        public void ColumnAliasTest()
        {
            var sql = "SELECT ID a FROM Author";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [a] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void RepeatedColumnAliasTest()
        {
            var sql = "SELECT ID a, ID b FROM Author";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [a], [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [b] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void RepeatedColumnAliasTest2()
        {
            var sql = "SELECT ID, ID FROM Author";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[ID] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest()
        {
            var sql = "SELECT a.* FROM Author a";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].* FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest2()
        {
            var sql = "SELECT a.*, b.ID AS q FROM Author a CROSS JOIN Author b";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].*, [b].[ID] AS [q] FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Author] [b]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest3()
        {
            var sql = "SELECT a.*, a.* FROM Author a";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].*, [a].* FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);
        }

        [TestMethod]
        public void MultipleSelectStarTest99()
        {
            var sql = "SELECT a.ID q, b.* FROM Author a CROSS JOIN Author b";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [q], [b].* FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Author] [b]", res);
        }

        #region Function tests

        [TestMethod]
        public void SystemFunctionTest()
        {
            var sql = "SELECT SIN(1)";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT SIN(1)", res);
        }

        [TestMethod]
        public void ScalarFunctionTest()
        {
            var sql = "SELECT dbo.TestScalarFunction(1)";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[TestScalarFunction](1)", res);
        }

        [TestMethod]
        public void ScalarFunctionInSubqueryTest()
        {
            var sql = "SELECT * FROM (SELECT dbo.TestScalarFunction(1) t) a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);
            var sq = qs.FindDescendantRecursive<Subquery>().QueryExpression.FirstQuerySpecification;
            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT * FROM (SELECT [Graywulf_Schema_Test].[dbo].[TestScalarFunction](1) AS [t]) [a]", res);

            Assert.AreEqual(1, sq.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("t", sq.ResultsTableReference.ColumnReferences[0].ColumnName);
            Assert.AreEqual("a", sq.ResultsTableReference.ColumnReferences[0].TableReference.Alias);
            Assert.AreEqual(1, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("t", qs.ResultsTableReference.ColumnReferences[0].ColumnName);
        }

        [TestMethod]
        public void TableValuedFunctionTest()
        {
            var sql = "SELECT * FROM dbo.TestTableValuedFunction(0) AS f";

            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var tables = qs.EnumerateSourceTableReferences(false).ToArray();

            Assert.AreEqual(1, tables.Length);
            Assert.IsTrue(tables[0].TableContext.HasFlag(TableContext.UserDefinedFunction));
            Assert.AreEqual("f", tables[0].Alias);
            Assert.AreEqual("[f]", tables[0].ToString());

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT * FROM [Graywulf_Schema_Test].[dbo].[TestTableValuedFunction](0) AS [f]", res);
        }

        #endregion

    }
}
