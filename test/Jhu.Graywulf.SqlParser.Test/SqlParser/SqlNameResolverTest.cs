using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.SqlParser.Test
{
    [TestClass]
    public class SqlNameResolverTest
    {
        private SchemaManager CreateSchemaManager()
        {
            var sm = new SqlServerSchemaManager();
            var ds = new SqlServerDataset("Test", "Data Source=localhost;Initial Catalog=Graywulf_Test;Integrated Security=true");

            sm.Datasets[ds.Name] = ds;

            return sm;
        }

        private void ResolveNames(QuerySpecification qs)
        {
            var ss = qs.FindAscendant<SelectStatement>();

            var nr = new SqlNameResolver();
            nr.SchemaManager = CreateSchemaManager();
            nr.DefaultDatasetName = "Test";
            nr.DefaultSchemaName = "dbo";
            nr.Execute(ss);
        }

        private QuerySpecification Parse(string query)
        {
            var p = new SqlParser();
            var ss = (SelectStatement)p.Execute(query);
            var qs = (QuerySpecification)ss.EnumerateQuerySpecifications().First();
            
            ResolveNames(qs);

            return qs;
        }

        private string GenerateCode(QuerySpecification qs)
        {
            var cg = new SqlCodeGen.SqlServerCodeGenerator();
            cg.ResolveNames = true;

            var sw = new StringWriter();
            cg.Execute(sw, qs);

            return sw.ToString();
        }

        [TestMethod]
        public void SimpleQueryTest()
        {
            var sql = "SELECT Name FROM Author";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Test].[dbo].[Author]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].TableReference);
        }

        [TestMethod]
        public void SimpleQueryWithJoinTest()
        {
            var sql = "SELECT Name FROM Author INNER JOIN Book ON Author.ID = 5";

            var qs = Parse(sql);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.From, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].TableReference);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Test].[dbo].[Author] INNER JOIN [Graywulf_Test].[dbo].[Book] ON [Graywulf_Test].[dbo].[Author].[ID] = 5", res);
        }

        [TestMethod]
        public void SimpleQueryWithWhereTest()
        {
            var sql = "SELECT Name FROM Author WHERE ID = 1";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Test].[dbo].[Author] WHERE [Graywulf_Test].[dbo].[Author].[ID] = 1", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.Where, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].TableReference);
        }

        [TestMethod]
        public void SimpleQueryWithGroupByTest()
        {
            var sql = "SELECT ID, MAX(Name) FROM Author GROUP BY ID";

            var qs = Parse(sql);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.GroupBy, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual("ID", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].TableReference);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [ID], MAX([Graywulf_Test].[dbo].[Author].[Name]) AS [Col_1] FROM [Graywulf_Test].[dbo].[Author] GROUP BY [Graywulf_Test].[dbo].[Author].[ID]", res);
        }

        [TestMethod]
        public void SimpleQueryWithHavingTest()
        {
            var sql = "SELECT ID, MAX(Name) FROM Author GROUP BY ID HAVING MAX(Name) > 5";

            var qs = Parse(sql);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.GroupBy, ts[0].ColumnReferences[0].ColumnContext);
            Assert.AreEqual(ColumnContext.SelectList | ColumnContext.Having, ts[0].ColumnReferences[1].ColumnContext);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual("ID", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].TableReference);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [ID], MAX([Graywulf_Test].[dbo].[Author].[Name]) AS [Col_1] FROM [Graywulf_Test].[dbo].[Author] GROUP BY [Graywulf_Test].[dbo].[Author].[ID] HAVING MAX([Graywulf_Test].[dbo].[Author].[Name]) > 5", res);
        }

        [TestMethod]
        public void SimpleTableAliasQueryTest()
        {
            var sql = "SELECT Name FROM Author a";

            var qs = Parse(sql);
            var ts = qs.SourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] AS [a_Name] FROM [Graywulf_Test].[dbo].[Author] [a]", res);

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
        }

        [TestMethod]
        public void TwoTableAliasQueryTest()
        {
            var sql = "SELECT a.Name, b.Name FROM Author a, Author b";

            var qs = Parse(sql);
            var ts = qs.SourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] AS [a_Name], [b].[Name] AS [b_Name] FROM [Graywulf_Test].[dbo].[Author] [a], [Graywulf_Test].[dbo].[Author] [b]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
            Assert.AreEqual("Author", ts[1].DatabaseObjectName);
            Assert.AreEqual("b", ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual(cs[0].TableReference, ts[0]);
            Assert.AreEqual(cs[1].TableReference, ts[1]);
            Assert.AreNotEqual(ts[0], ts[1]);
        }

        [TestMethod]
        public void TwoTableAliasQueryTest2()
        {
            var sql = "SELECT a.ID, b.ID FROM Author a, Book b";

            var qs = Parse(sql);
            var ts = qs.SourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [b].[ID] AS [b_ID] FROM [Graywulf_Test].[dbo].[Author] [a], [Graywulf_Test].[dbo].[Book] [b]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
            Assert.AreEqual("Book", ts[1].DatabaseObjectName);
            Assert.AreEqual("b", ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual(cs[0].TableReference, ts[0]);
            Assert.AreEqual(cs[1].TableReference, ts[1]);
        }

        [TestMethod]
        public void TwoTableReferenceTest()
        {
            var sql = "SELECT Author.ID, Book.ID FROM Author, Book";

            var qs = Parse(sql);
            var ts = qs.SourceTableReferences.Values.ToArray();

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Test].[dbo].[Book].[ID] AS [ID_0] FROM [Graywulf_Test].[dbo].[Author], [Graywulf_Test].[dbo].[Book]", res);

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
            Assert.AreEqual("Book", ts[1].DatabaseObjectName);
            Assert.AreEqual(null, ts[1].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual(cs[0].TableReference, ts[0]);
            Assert.AreEqual(cs[1].TableReference, ts[1]);

        }

        [TestMethod]
        public void AmbigousColumnNameTest()
        {
            var sql = "SELECT Name FROM Author a, Author b";

            try
            {
                var qs = Parse(sql);
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
                var qs = Parse(sql);
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
                var qs = Parse(sql);
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
                var qs = Parse(sql);
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
                var qs = Parse(sql);
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
                var qs = Parse(sql);
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

            var qs = Parse(sql);

            Assert.AreEqual(2, qs.ResultsTableReference.ColumnReferences.Count);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void AliasStarQueryTest()
        {
            var sql = "SELECT * FROM Author AS a";

            var qs = Parse(sql);

            Assert.AreEqual(2, qs.ResultsTableReference.ColumnReferences.Count);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name] FROM [Graywulf_Test].[dbo].[Author] AS [a]", res);
        }

        [TestMethod]
        public void TwoTableStarQueryTest()
        {
            var sql = "SELECT * FROM Author CROSS JOIN Book";

            var qs = Parse(sql);

            Assert.AreEqual(5, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("Test:[dbo].[Author].[ID]", qs.ResultsTableReference.ColumnReferences[0].FullyQualifiedName);
            Assert.AreEqual("Test:[dbo].[Author].[Name]", qs.ResultsTableReference.ColumnReferences[1].FullyQualifiedName);
            Assert.AreEqual("Test:[dbo].[Book].[ID]", qs.ResultsTableReference.ColumnReferences[2].FullyQualifiedName);
            Assert.AreEqual("Test:[dbo].[Book].[Title]", qs.ResultsTableReference.ColumnReferences[3].FullyQualifiedName);
            Assert.AreEqual("Test:[dbo].[Book].[Year]", qs.ResultsTableReference.ColumnReferences[4].FullyQualifiedName);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Test].[dbo].[Author].[Name] AS [Name], [Graywulf_Test].[dbo].[Book].[ID] AS [ID_0], [Graywulf_Test].[dbo].[Book].[Title] AS [Title], [Graywulf_Test].[dbo].[Book].[Year] AS [Year] FROM [Graywulf_Test].[dbo].[Author] CROSS JOIN [Graywulf_Test].[dbo].[Book]", res);
        }

        [TestMethod]
        public void AliasTwoTableStarQueryTest()
        {
            var sql = "SELECT * FROM Author AS a CROSS JOIN Book AS b";

            var qs = Parse(sql);

            Assert.AreEqual(5, qs.ResultsTableReference.ColumnReferences.Count);
            Assert.AreEqual("[a].[ID]", qs.ResultsTableReference.ColumnReferences[0].FullyQualifiedName);
            Assert.AreEqual("[a].[Name]", qs.ResultsTableReference.ColumnReferences[1].FullyQualifiedName);
            Assert.AreEqual("[b].[ID]", qs.ResultsTableReference.ColumnReferences[2].FullyQualifiedName);
            Assert.AreEqual("[b].[Title]", qs.ResultsTableReference.ColumnReferences[3].FullyQualifiedName);
            Assert.AreEqual("[b].[Year]", qs.ResultsTableReference.ColumnReferences[4].FullyQualifiedName);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [b].[ID] AS [b_ID], [b].[Title] AS [b_Title], [b].[Year] AS [b_Year] FROM [Graywulf_Test].[dbo].[Author] AS [a] CROSS JOIN [Graywulf_Test].[dbo].[Book] AS [b]", res);
        }

        [TestMethod]
        public void SimpleSubqueryTest()
        {
            var sql = "SELECT Name FROM (SELECT Name FROM Author) a";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] AS [a_Name] FROM (SELECT [Graywulf_Test].[dbo].[Author].[Name] FROM [Graywulf_Test].[dbo].[Author]) [a]", res);
        }

        [TestMethod]
        public void SimpleOrderByTest()
        {
            var sql = "SELECT ID FROM Author ORDER BY Name";

            var qs = Parse(sql);

            var res = GenerateCode(qs);

            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [ID] FROM [Graywulf_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void SelectStarSubqueryTest()
        {
            var sql = "SELECT Name FROM (SELECT * FROM Author) a";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] AS [a_Name] FROM (SELECT [Graywulf_Test].[dbo].[Author].[ID], [Graywulf_Test].[dbo].[Author].[Name] FROM [Graywulf_Test].[dbo].[Author]) [a]", res);
        }

        [TestMethod]
        public void SelectStarMultipleSubqueryTest()
        {
            var sql = 
@"SELECT a.Name, b.Name
FROM (SELECT * FROM Author) a
CROSS JOIN (SELECT * FROM Author) b";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual(
@"SELECT [a].[Name] AS [a_Name], [b].[Name] AS [b_Name]
FROM (SELECT [Graywulf_Test].[dbo].[Author].[ID], [Graywulf_Test].[dbo].[Author].[Name] FROM [Graywulf_Test].[dbo].[Author]) [a]
CROSS JOIN (SELECT [Graywulf_Test].[dbo].[Author].[ID], [Graywulf_Test].[dbo].[Author].[Name] FROM [Graywulf_Test].[dbo].[Author]) [b]", res);
        }

        [TestMethod]
        public void MultipleSelectStarTest()
        {
            var sql = "SELECT a.*, b.* FROM Author a CROSS JOIN Author b";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [b].[ID] AS [b_ID], [b].[Name] AS [b_Name] FROM [Graywulf_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Test].[dbo].[Author] [b]", res);
        }

        [TestMethod]
        public void ColumnAliasTest()
        {
            var sql = "SELECT ID a FROM Author";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [a] FROM [Graywulf_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void RepeatedColumnAliasTest()
        {
            var sql = "SELECT ID a, ID b FROM Author";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [a], [Graywulf_Test].[dbo].[Author].[ID] AS [b] FROM [Graywulf_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void RepeatedColumnAliasTest2()
        {
            var sql = "SELECT ID, ID FROM Author";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Test].[dbo].[Author].[ID] AS [ID_0] FROM [Graywulf_Test].[dbo].[Author]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest()
        {
            var sql = "SELECT a.* FROM Author a";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name] FROM [Graywulf_Test].[dbo].[Author] [a]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest2()
        {
            var sql = "SELECT a.*, b.ID AS q FROM Author a CROSS JOIN Author b";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [b].[ID] AS [q] FROM [Graywulf_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Test].[dbo].[Author] [b]", res);
        }

        [TestMethod]
        public void SelectStarColumnAliasTest3()
        {
            var sql = "SELECT a.*, a.* FROM Author a";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [a_ID], [a].[Name] AS [a_Name], [a].[ID] AS [a_ID_0], [a].[Name] AS [a_Name_0] FROM [Graywulf_Test].[dbo].[Author] [a]", res);
        }

        [TestMethod]
        public void MultipleSelectStarTest99()
        {
            var sql = "SELECT a.ID q, b.* FROM Author a CROSS JOIN Author b";

            var qs = Parse(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[ID] AS [q], [b].[ID] AS [b_ID], [b].[Name] AS [b_Name] FROM [Graywulf_Test].[dbo].[Author] [a] CROSS JOIN [Graywulf_Test].[dbo].[Author] [b]", res);
        }
    }
}
