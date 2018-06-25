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

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].ParentTableReference);
        }

        [TestMethod]
        public void SimpleQueryWithJoinTest()
        {
            var sql = "SELECT Name FROM Author INNER JOIN Book ON Author.ID = 5";

            var qs = Parse<QuerySpecification>(sql);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.From | ColumnContext.PrimaryKey, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].ParentTableReference);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author] INNER JOIN [Graywulf_Schema_Test].[dbo].[Book] ON [Graywulf_Schema_Test].[dbo].[Author].[ID] = 5", res);
        }

        [TestMethod]
        public void SimpleQueryWithWhereTest()
        {
            var sql = "SELECT Name FROM Author WHERE ID = 1";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author] WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = 1", res);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.Where | ColumnContext.PrimaryKey, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].ParentTableReference);
        }

        [TestMethod]
        public void SimpleQueryWithGroupByTest()
        {
            var sql = "SELECT ID, MAX(Name) FROM Author GROUP BY ID";

            var qs = Parse<QuerySpecification>(sql);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.GroupBy | ColumnContext.PrimaryKey, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual("ID", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].ParentTableReference);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], MAX([Graywulf_Schema_Test].[dbo].[Author].[Name]) AS [Col_1] FROM [Graywulf_Schema_Test].[dbo].[Author] GROUP BY [Graywulf_Schema_Test].[dbo].[Author].[ID]", res);
        }

        [TestMethod]
        public void SimpleQueryWithHavingTest()
        {
            var sql = "SELECT ID, MAX(Name) FROM Author GROUP BY ID HAVING MAX(Name) > 5";

            var qs = Parse<QuerySpecification>(sql);

            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.GroupBy | ColumnContext.PrimaryKey, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.Having, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual("ID", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].ParentTableReference);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], MAX([Graywulf_Schema_Test].[dbo].[Author].[Name]) AS [Col_1] FROM [Graywulf_Schema_Test].[dbo].[Author] GROUP BY [Graywulf_Schema_Test].[dbo].[Author].[ID] HAVING MAX([Graywulf_Schema_Test].[dbo].[Author].[Name]) > 5", res);
        }

        [TestMethod]
        public void SimpleTableAliasQueryTest()
        {
            var sql = "SELECT Name FROM Author a";

            var qs = Parse<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] AS [a_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
        }

        [TestMethod]
        public void SimpleSelectWithTableNameQueryTest()
        {
            var sql = "SELECT Author.Name FROM Author";

            var qs = Parse<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void TwoTableAliasQueryTest()
        {
            var sql = "SELECT a.Name, b.Name FROM Author a, Author b";

            var qs = Parse<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] AS [a_Name], [b].[Name] AS [b_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a], [Graywulf_Schema_Test].[dbo].[Author] [b]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
            Assert.AreEqual("Author", ts[1].DatabaseObjectName);
            Assert.AreEqual("b", ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual(cs[0].ParentTableReference, ts[0]);
            Assert.AreEqual(cs[1].ParentTableReference, ts[1]);
            Assert.AreNotEqual(ts[0], ts[1]);
        }

        [TestMethod]
        public void TwoTableAliasQueryTest2()
        {
            var sql = "SELECT a.ID, b.ID FROM Author a, Book b";

            var qs = Parse<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [b].[ID] AS [b_ID] FROM [Graywulf_Schema_Test].[dbo].[Author] [a], [Graywulf_Schema_Test].[dbo].[Book] [b]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
            Assert.AreEqual("Book", ts[1].DatabaseObjectName);
            Assert.AreEqual("b", ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual(cs[0].ParentTableReference, ts[0]);
            Assert.AreEqual(cs[1].ParentTableReference, ts[1]);
        }

        [TestMethod]
        public void TwoTableReferenceTest()
        {
            var sql = "SELECT Author.ID, Book.ID FROM Author, Book";

            var qs = Parse<QuerySpecification>(sql);
            var ts = qs.ResolvedSourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Schema_Test].[dbo].[Book].[ID] AS [ID_0] FROM [Graywulf_Schema_Test].[dbo].[Author], [Graywulf_Schema_Test].[dbo].[Book]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
            Assert.AreEqual("Book", ts[1].DatabaseObjectName);
            Assert.AreEqual(null, ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual(cs[0].ParentTableReference, ts[0]);
            Assert.AreEqual(cs[1].ParentTableReference, ts[1]);

        }

        [TestMethod]
        public void AmbigousColumnNameTest()
        {
            var sql = "SELECT Name FROM Author a, Author b";

            try
            {
                var qs = Parse<QuerySpecification>(sql);
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
                var qs = Parse<QuerySpecification>(sql);
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
                var qs = Parse<QuerySpecification>(sql);
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
                var qs = Parse<QuerySpecification>(sql);
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
                var qs = Parse<QuerySpecification>(sql);
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
                var qs = Parse<QuerySpecification>(sql);
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

            var qs = Parse<QuerySpecification>(sql);

            Assert.AreEqual(2, qs.ResultsTableReference.ColumnReferences.Count);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void AliasStarQueryTest()
        {
            var sql = "SELECT * FROM Author AS a";

            var qs = Parse<QuerySpecification>(sql);

            Assert.AreEqual(2, qs.ResultsTableReference.ColumnReferences.Count);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a]", res);
        }

        [TestMethod]
        public void TwoTableStarQueryTest()
        {
            var sql = "SELECT * FROM Author CROSS JOIN Book";

            var qs = Parse<QuerySpecification>(sql);

            Assert.AreEqual(5, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("[TEST]:[Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID]", qs.ResultsTableReference.ColumnReferences[0].ToString());
            Assert.AreEqual("[TEST]:[Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name]", qs.ResultsTableReference.ColumnReferences[1].ToString());
            Assert.AreEqual("[TEST]:[Graywulf_Schema_Test].[dbo].[Book].[ID] AS [ID_0]", qs.ResultsTableReference.ColumnReferences[2].ToString());
            Assert.AreEqual("[TEST]:[Graywulf_Schema_Test].[dbo].[Book].[Title] AS [Title]", qs.ResultsTableReference.ColumnReferences[3].ToString());
            Assert.AreEqual("[TEST]:[Graywulf_Schema_Test].[dbo].[Book].[Year] AS [Year]", qs.ResultsTableReference.ColumnReferences[4].ToString());

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name], [Graywulf_Schema_Test].[dbo].[Book].[ID] AS [ID_0], [Graywulf_Schema_Test].[dbo].[Book].[Title] AS [Title], [Graywulf_Schema_Test].[dbo].[Book].[Year] AS [Year] FROM [Graywulf_Schema_Test].[dbo].[Author] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Book]", res);
        }

        [TestMethod]
        public void AliasTwoTableStarQueryTest()
        {
            var sql = "SELECT * FROM Author AS a CROSS JOIN Book AS b";

            var qs = Parse<QuerySpecification>(sql);

            Assert.AreEqual(5, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("[a].[ID] AS [a_ID]", qs.ResultsTableReference.ColumnReferences[0].ToString());
            Assert.AreEqual("[a].[Name] AS [a_Name]", qs.ResultsTableReference.ColumnReferences[1].ToString());
            Assert.AreEqual("[b].[ID] AS [b_ID]", qs.ResultsTableReference.ColumnReferences[2].ToString());
            Assert.AreEqual("[b].[Title] AS [b_Title]", qs.ResultsTableReference.ColumnReferences[3].ToString());
            Assert.AreEqual("[b].[Year] AS [b_Year]", qs.ResultsTableReference.ColumnReferences[4].ToString());

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [b].[ID] AS [b_ID], [b].[Title] AS [b_Title], [b].[Year] AS [b_Year] FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Book] AS [b]", res);
        }

        [TestMethod]
        public void SameTableNameTest()
        {
            var sql = "SELECT * FROM dbo.Author a CROSS JOIN test.Author b";

            var qs = Parse<QuerySpecification>(sql);

            Assert.AreEqual(4, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("[a].[ID] AS [a_ID]", qs.ResultsTableReference.ColumnReferences[0].ToString());
            Assert.AreEqual("[a].[Name] AS [a_Name]", qs.ResultsTableReference.ColumnReferences[1].ToString());
            Assert.AreEqual("[b].[ID] AS [b_ID]", qs.ResultsTableReference.ColumnReferences[2].ToString());
            Assert.AreEqual("[b].[Name] AS [b_Name]", qs.ResultsTableReference.ColumnReferences[3].ToString());

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [b].[ID] AS [b_ID], [b].[Name] AS [b_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[test].[Author] [b]", res);
        }

        [TestMethod]
        public void SimpleOrderByTest()
        {
            var sql = "SELECT ID FROM Author ORDER BY Name";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void SelectStarSubqueryTest()
        {
            var sql = "SELECT Name FROM (SELECT * FROM Author) a";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] AS [a_Name] FROM (SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]", res);
        }

        [TestMethod]
        public void SelectStarMultipleSubqueryTest()
        {
            var sql =
@"SELECT a.Name, b.Name
FROM (SELECT * FROM Author) a
CROSS JOIN (SELECT * FROM Author) b";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual(
@"SELECT [a].[Name] AS [a_Name], [b].[Name] AS [b_Name]
FROM (SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [a]
CROSS JOIN (SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]) [b]", res);
        }

        [TestMethod]
        public void MultipleSelectStarTest()
        {
            var sql = "SELECT a.*, b.* FROM Author a CROSS JOIN Author b";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [b].[ID] AS [b_ID], [b].[Name] AS [b_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Author] [b]", res);
        }

        [TestMethod]
        public void ColumnAliasTest()
        {
            var sql = "SELECT ID a FROM Author";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [a] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void RepeatedColumnAliasTest()
        {
            var sql = "SELECT ID a, ID b FROM Author";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [a], [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [b] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void RepeatedColumnAliasTest2()
        {
            var sql = "SELECT ID, ID FROM Author";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID_0] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest()
        {
            var sql = "SELECT a.* FROM Author a";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest2()
        {
            var sql = "SELECT a.*, b.ID AS q FROM Author a CROSS JOIN Author b";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [b].[ID] AS [q] FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Author] [b]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest3()
        {
            var sql = "SELECT a.*, a.* FROM Author a";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [a].[ID] AS [a_ID_0], [a].[Name] AS [a_Name_0] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);
        }

        [TestMethod]
        public void MultipleSelectStarTest99()
        {
            var sql = "SELECT a.ID q, b.* FROM Author a CROSS JOIN Author b";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [q], [b].[ID] AS [b_ID], [b].[Name] AS [b_Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Schema_Test].[dbo].[Author] [b]", res);
        }

        #region Function tests

        [TestMethod]
        public void SystemFunctionTest()
        {
            var sql = "SELECT SIN(1)";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT SIN(1) AS [Col_0]", res);
        }

        [TestMethod]
        public void ScalarFunctionTest()
        {
            var sql = "SELECT dbo.TestScalarFunction(1)";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[TestScalarFunction](1) AS [Col_0]", res);
        }

        [TestMethod]
        public void ScalarFunctionInSubqueryTest()
        {
            var sql = "SELECT * FROM (SELECT dbo.TestScalarFunction(1) t) a";

            var qs = Parse<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[t] AS [a_t] FROM (SELECT [Graywulf_Schema_Test].[dbo].[TestScalarFunction](1) AS [t]) [a]", res);
        }

        [TestMethod]
        public void TableValuedFunctionTest()
        {
            var sql = "SELECT * FROM dbo.TestTableValuedFunction(0) AS f";

            var qs = Parse<QuerySpecification>(sql);

            var tables = qs.EnumerateSourceTableReferences(false).ToArray();

            Assert.AreEqual(1, tables.Length);
            Assert.IsTrue(tables[0].TableContext.HasFlag(TableContext.UserDefinedFunction));
            Assert.AreEqual("f", tables[0].Alias);
            Assert.AreEqual("[f]", tables[0].ToString());

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [f].[b] AS [f_b], [f].[a] AS [f_a] FROM [Graywulf_Schema_Test].[dbo].[TestTableValuedFunction](0) AS [f]", res);
        }

        #endregion

    }
}
