using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class InsertStatementTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void InsertWithValuesTest()
        {
            var sql = 
@"INSERT Author
VALUES (1, 'test')";

            var gt =
@"INSERT [Graywulf_Schema_Test].[dbo].[Author]
VALUES (1, 'test')";

            var ds = ParseAndResolveNames<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        [TestMethod]
        public void InsertWithColumnNamesAndValuesTest()
        {
            var sql =
@"INSERT Author
(ID, Name)
VALUES (1, 'test')";

            var gt =
@"INSERT [Graywulf_Schema_Test].[dbo].[Author]
([Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name])
VALUES (1, 'test')";

            var ds = ParseAndResolveNames<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cn = ds.ColumnList.EnumerateDescendants<ColumnIdentifier>().ToArray();
            Assert.AreEqual(2, cn.Length);
            Assert.AreEqual("ID", cn[0].ColumnReference.ColumnName);
            Assert.AreEqual("Name", cn[1].ColumnReference.ColumnName);
        }

        [TestMethod]
        public void InsertWithQualifiedColumnNamesTest()
        {
            var sql =
@"INSERT Author
(Author.ID, Author.Name)
VALUES (1, 'test')";

            var gt =
@"INSERT [Graywulf_Schema_Test].[dbo].[Author]
([Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name])
VALUES (1, 'test')";

            var ds = ParseAndResolveNames<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cn = ds.ColumnList.EnumerateDescendants<ColumnIdentifier>().ToArray();
            Assert.AreEqual(2, cn.Length);
            Assert.AreEqual("ID", cn[0].ColumnReference.ColumnName);
            Assert.AreEqual("Name", cn[1].ColumnReference.ColumnName);
        }

        [TestMethod]
        public void InsertWithColumnNamesAndSubQueryValuesTest()
        {
            var sql =
@"INSERT Author
(ID, Name)
VALUES
((SELECT 1), (SELECT TOP 1 Name FROM Author ORDER BY ID))";

            var gt =
@"INSERT [Graywulf_Schema_Test].[dbo].[Author]
([Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name])
VALUES
((SELECT 1), (SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[ID]))";

            var qs = ParseAndResolveNames(sql);
            var ds = qs.ParsingTree.FindDescendantRecursive<InsertStatement>();
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences;
            Assert.AreEqual(1, ts.Count);
            Assert.AreEqual("Author", ts["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ts["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);

            Assert.AreEqual(1, qs.SourceTableReferences.Count);
            Assert.AreEqual(2, qs.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Author"].Count);

            var cn = ds.ColumnList.EnumerateDescendants<ColumnIdentifier>().ToArray();
            Assert.AreEqual(2, cn.Length);
            Assert.AreEqual("ID", cn[0].ColumnReference.ColumnName);
            Assert.AreEqual("Name", cn[1].ColumnReference.ColumnName);
        }

        [TestMethod]
        public void InsertWithColumnNamesAndSubQueryValues2Test()
        {
            var sql =
@"INSERT Author
(ID, Name)
VALUES
((SELECT TOP 1 Title FROM Book ORDER BY ID), (SELECT TOP 1 Name FROM Author ORDER BY ID))";

            var gt =
@"INSERT [Graywulf_Schema_Test].[dbo].[Author]
([Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name])
VALUES
((SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Book].[Title] FROM [Graywulf_Schema_Test].[dbo].[Book] ORDER BY [Graywulf_Schema_Test].[dbo].[Book].[ID]), (SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[ID]))";

            var qs = ParseAndResolveNames(sql);
            var ds = qs.ParsingTree.FindDescendantRecursive<InsertStatement>();
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);


            var ts = ds.SourceTableReferences;
            Assert.AreEqual(1, ts.Count);
            Assert.AreEqual("Author", ts["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ts["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);

            Assert.AreEqual(2, qs.SourceTableReferences.Count);
            Assert.AreEqual(2, qs.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Author"].Count);
            Assert.AreEqual(1, qs.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Book"].Count);


            var cn = ds.ColumnList.EnumerateDescendants<ColumnIdentifier>().ToArray();
            Assert.AreEqual(2, cn.Length);
            Assert.AreEqual("ID", cn[0].ColumnReference.ColumnName);
            Assert.AreEqual("Name", cn[1].ColumnReference.ColumnName);
        }

        [TestMethod]
        public void InsertWithColumnNamesAndSubQueryValues3Test()
        {
            var sql =
@"INSERT Author
(ID, Name)
VALUES
((SELECT TOP 1 Title FROM Book ORDER BY ID) + (SELECT TOP 1 Name FROM Author ORDER BY ID), (SELECT TOP 1 Name FROM Author ORDER BY ID))";

            var gt =
@"INSERT [Graywulf_Schema_Test].[dbo].[Author]
([Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name])
VALUES
((SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Book].[Title] FROM [Graywulf_Schema_Test].[dbo].[Book] ORDER BY [Graywulf_Schema_Test].[dbo].[Book].[ID]) + (SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[ID]), (SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[ID]))";

            var qs = ParseAndResolveNames(sql);
            var ds = qs.ParsingTree.FindDescendantRecursive<InsertStatement>();
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences;
            Assert.AreEqual(1, ts.Count);
            Assert.AreEqual("Author", ts["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ts["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);

            Assert.AreEqual(2, qs.SourceTableReferences.Count);
            Assert.AreEqual(3, qs.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Author"].Count);
            Assert.AreEqual(1, qs.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Book"].Count);

            var cn = ds.ColumnList.EnumerateDescendants<ColumnIdentifier>().ToArray();
            Assert.AreEqual(2, cn.Length);
            Assert.AreEqual("ID", cn[0].ColumnReference.ColumnName);
            Assert.AreEqual("Name", cn[1].ColumnReference.ColumnName);
        }

        [TestMethod]
        public void InsertWithColumnNamesAndSubQueryValues4Test()
        {
            // This tests sub-subqueries

            var sql =
@"INSERT Author
(ID, Name)
VALUES
((SELECT TOP 1 Title FROM (SELECT * FROM Book) q ORDER BY ID), (SELECT TOP 1 Name FROM Author ORDER BY ID))";

            var gt =
@"INSERT [Graywulf_Schema_Test].[dbo].[Author]
([Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[Name])
VALUES
((SELECT TOP 1 [q].[Title] FROM (SELECT * FROM [Graywulf_Schema_Test].[dbo].[Book]) [q] ORDER BY [q].[ID]), (SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[ID]))";

            var qs = ParseAndResolveNames(sql);
            var ds = qs.ParsingTree.FindDescendantRecursive<InsertStatement>();
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences;
            Assert.AreEqual(1, ts.Count);
            Assert.AreEqual("Author", ts["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ts["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);

            Assert.AreEqual(2, qs.SourceTableReferences.Count);
            Assert.AreEqual(2, qs.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Author"].Count);
            Assert.AreEqual(1, qs.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Book"].Count);

            var cn = ds.ColumnList.EnumerateDescendants<ColumnIdentifier>().ToArray();
            Assert.AreEqual(2, cn.Length);
            Assert.AreEqual("ID", cn[0].ColumnReference.ColumnName);
            Assert.AreEqual("Name", cn[1].ColumnReference.ColumnName);
        }

        [TestMethod]
        public void InsertWithSelectTest()
        {
            var sql =
@"INSERT Author
SELECT TOP 100 * FROM Author";

            var gt =
@"INSERT [Graywulf_Schema_Test].[dbo].[Author]
SELECT TOP 100 * FROM [Graywulf_Schema_Test].[dbo].[Author]";

            var ds = ParseAndResolveNames<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void InsertColumnNotPartOfTargetTest()
        {
            var sql =
@"INSERT Author
(ID2, Name3)
DEFAULT VALUES";

            ParseAndResolveNames<InsertStatement>(sql);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void InsertColumnNotPartOfTargetTest2()
        {
            var sql =
@"WITH q AS (SELECT 1 AS ID2, '' AS Name3)
INSERT Author
(ID2, Name3)
DEFAULT VALUES";

            ParseAndResolveNames<InsertStatement>(sql);
        }

        [TestMethod]
        public void ValueFromCommonTableExpressionTest()
        {
            var sql =
@"WITH q AS (SELECT 1 AS ID2, '' AS Name3)
INSERT Author
(ID, NAME)
VALUES
((SELECT ID2 FROM q), (SELECT Name3 FROM q))";

            var gt =
@"WITH [q] AS (SELECT 1 AS [ID2], '' AS [Name3])
INSERT [Graywulf_Schema_Test].[dbo].[Author]
([Graywulf_Schema_Test].[dbo].[Author].[ID], [Graywulf_Schema_Test].[dbo].[Author].[NAME])
VALUES
((SELECT [q].[ID2] FROM [q]), (SELECT [q].[Name3] FROM [q]))";

            var exp = ParseAndResolveNames<InsertStatement>(sql);
            var res = GenerateCode(exp);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void InsertIntoVariableTest()
        {
            var sql =
@"DECLARE @t AS TABLE
(
    ID int,
    Data float
)

INSERT @t
(ID, Data)
VALUES
(0, 1.0)";

            var gt =
@"INSERT @t
([@t].[ID], [@t].[Data])
VALUES
(0, 1.0)";

            var ds = ParseAndResolveNames<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("@t", ts[0].VariableName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        // TODO: values part with subquery in expression
        // TODO: values part with sum of queries in expression
        // TODO: select with join
        // TOSO: select with order by
    }
}
