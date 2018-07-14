using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class DeleteStatementTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleDeleteTest()
        {
            var sql = "DELETE Author";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("DELETE [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
        }

        [TestMethod]
        public void DeleteWithWhereTest()
        {
            var sql = "DELETE Author WHERE ID = 1";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("DELETE [Graywulf_Schema_Test].[dbo].[Author] WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = 1", res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
        }

        [TestMethod]
        public void DeleteWithFromTest()
        {
            var sql = 
@"DELETE Author
FROM Author";

            var gt =
@"DELETE [Graywulf_Schema_Test].[dbo].[Author]
FROM [Graywulf_Schema_Test].[dbo].[Author]";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
        }

        [TestMethod]
        public void DeleteWithJoinTest()
        {
            var sql =
@"DELETE Author
FROM Author
INNER JOIN Book ON Book.ID = Book.ID";

            var gt =
@"DELETE [Graywulf_Schema_Test].[dbo].[Author]
FROM [Graywulf_Schema_Test].[dbo].[Author]
INNER JOIN [Graywulf_Schema_Test].[dbo].[Book] ON [Graywulf_Schema_Test].[dbo].[Book].[ID] = [Graywulf_Schema_Test].[dbo].[Book].[ID]";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(3, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
        }

        [TestMethod]
        public void DeleteWithSubqueryTest()
        {
            var sql =
@"DELETE Author
WHERE ID IN (SELECT ID FROM Book)";

            var gt =
@"DELETE [Graywulf_Schema_Test].[dbo].[Author]
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] IN (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
            Assert.AreEqual("Book", ts[1].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[1].TableReference.Alias);
        }
    }
}
