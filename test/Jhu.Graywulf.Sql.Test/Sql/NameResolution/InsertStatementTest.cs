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

            var ts = ds.EnumerateSourceTableReferences(false).ToArray();

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

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTableReferences(false).ToArray();
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
((SELECT 1 AS [Col_0]), (SELECT TOP 1 [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[ID]))";

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTableReferences(false).ToArray();
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

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
SELECT TOP 100 [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author]";

            var ds = Parse<InsertStatement>(sql);
            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTableReferences(false).ToArray();
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        // TODO: select with join
        // TOSO: select with order by
    }
}
