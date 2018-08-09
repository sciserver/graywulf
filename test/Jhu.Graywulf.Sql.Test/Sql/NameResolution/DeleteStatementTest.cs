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

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        [TestMethod]
        public void DeleteWithAliasTest()
        {
            var sql = "DELETE a FROM Author a";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("DELETE [a] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["a"].DatabaseObjectName);
            Assert.AreEqual("a", ds.SourceTableReferences["a"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual("a", ds.TargetTable.TableReference.Alias);
        }

        [TestMethod]
        public void DeleteWithWhereTest()
        {
            var sql = "DELETE Author WHERE ID = 1";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("DELETE [Graywulf_Schema_Test].[dbo].[Author] WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = 1", res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ds.SourceTableReferences["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);
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

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ds.SourceTableReferences["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);
        }

        [TestMethod]
        public void DeleteWithFromAndWhereTest()
        {
            var sql =
@"DELETE Author
FROM Author
WHERE ID = 2";

            var gt =
@"DELETE [Graywulf_Schema_Test].[dbo].[Author]
FROM [Graywulf_Schema_Test].[dbo].[Author]
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = 2";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ds.SourceTableReferences["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);
        }

        [TestMethod]
        public void DeleteWithFromAliasAndWhereTest()
        {
            var sql =
@"DELETE Author
FROM Author a
WHERE ID = 2";

            var gt =
@"DELETE [Graywulf_Schema_Test].[dbo].[Author]
FROM [Graywulf_Schema_Test].[dbo].[Author] [a]
WHERE [a].[ID] = 2";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["a"].DatabaseObjectName);
            Assert.AreEqual("a", ds.SourceTableReferences["a"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);
        }

        [TestMethod]
        public void DeleteWithFromAliasAndWhereTest2()
        {
            var sql =
@"DELETE a
FROM Author a
WHERE ID = 2";

            var gt =
@"DELETE [a]
FROM [Graywulf_Schema_Test].[dbo].[Author] [a]
WHERE [a].[ID] = 2";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["a"].DatabaseObjectName);
            Assert.AreEqual("a", ds.SourceTableReferences["a"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual("a", ds.TargetTable.TableReference.Alias);
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

            Assert.AreEqual(2, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ds.SourceTableReferences["Author"].Alias);
            Assert.AreEqual("Book", ds.SourceTableReferences["Book"].DatabaseObjectName);
            Assert.AreEqual(null, ds.SourceTableReferences["Book"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);
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

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ds.SourceTableReferences["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);
        }

        [TestMethod]
        public void DeleteTableVariableTest()
        {
            var sql =
@"DECLARE @t AS TABLE
(
    ID int,
    Data float
)

DELETE @t
WHERE ID = 2";

            var gt =
@"DELETE @t
WHERE [@t].[ID] = 2";

            var ds = ParseAndResolveNames<DeleteStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("@t", ds.SourceTableReferences["@t"].VariableName);
            Assert.AreEqual(null, ds.SourceTableReferences["@t"].Alias);

            Assert.AreEqual("@t", ds.TargetTable.TableReference.VariableName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);
        }
    }
}
