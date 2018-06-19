using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class UpdateStatementTest : SqlNameResolverTestBase
    {
        // TODO: 
        // -- update table variable
        // -- update CTE?
        // -- UPDATE a SET ... FROM table AS a

        [TestMethod]
        public void SimplUpdateTest()
        {
            var sql = "UPDATE Author SET Name = 'new_name'";

            var ds = Parse<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("UPDATE [Graywulf_Schema_Test].[dbo].[Author] SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name'", res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
        }

        [TestMethod]
        public void UpdateWithFromTest()
        {
            var sql = 
@"UPDATE Author
SET Name = 'new_name'
FROM Author";

            var gt =
@"UPDATE [Graywulf_Schema_Test].[dbo].[Author]
SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name'
FROM [Graywulf_Schema_Test].[dbo].[Author]";

            var ds = Parse<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
        }

        [TestMethod]
        public void UpdateWithTableAliasTest()
        {
            var sql =
@"UPDATE a
SET Name = 'new_name'
FROM Author AS a";

            var gt =
@"UPDATE [a]
SET [a].[Name] = 'new_name'
FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a]";

            var ds = Parse<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual("a", ts[0].TableReference.Alias);
        }

        [TestMethod]
        public void UpdateWithNoTableAliasTest()
        {
            var sql =
@"UPDATE Author
SET Name = 'new_name'
FROM Author AS a";

            var gt =
@"UPDATE [Graywulf_Schema_Test].[dbo].[Author]
SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name'
FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a]";

            var ds = Parse<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();
            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
            Assert.AreEqual("Author", ts[1].TableReference.DatabaseObjectName);
            Assert.AreEqual("a", ts[1].TableReference.Alias);
        }

        /*
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

            var ds = Parse<DeleteStatement>(sql);

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
WHERE [Graywulf_Schema_Test].[dbo].[Book].[ID] IN (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ds = Parse<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.EnumerateSourceTables(false).ToArray();

            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[0].TableReference.Alias);
            Assert.AreEqual("Book", ts[1].TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ts[1].TableReference.Alias);
        }
        */
    }
}
