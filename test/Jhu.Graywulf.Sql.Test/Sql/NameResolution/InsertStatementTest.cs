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

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
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

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);

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

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);

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

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(3, ts.Length);
            // Target
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
            // Source 1
            Assert.AreEqual("Book", ts[1].TableReference.DatabaseObjectName);
            // Source 2
            Assert.AreEqual("Author", ts[2].TableReference.DatabaseObjectName);

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

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(4, ts.Length);
            // Target
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
            // Source 1
            Assert.AreEqual("Book", ts[1].TableReference.DatabaseObjectName);
            // Source 2
            Assert.AreEqual("Author", ts[2].TableReference.DatabaseObjectName);

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

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(4, ts.Length);
            // Target
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
            // Source subquery
            Assert.IsTrue(ts[1].TableReference.TableContext.HasFlag(TableContext.Subquery));
            // Source 1
            Assert.AreEqual("Book", ts[2].TableReference.DatabaseObjectName);
            // Source 2
            Assert.AreEqual("Author", ts[3].TableReference.DatabaseObjectName);

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

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
        }

        // TODO: values part with subquery in expression
        // TODO: values part with sum of queries in expression
        // TODO: select with join
        // TOSO: select with order by
    }
}
