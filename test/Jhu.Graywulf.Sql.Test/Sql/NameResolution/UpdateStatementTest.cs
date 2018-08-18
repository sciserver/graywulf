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
        // -- update CTE?
        // -- UPDATE a SET ... FROM table AS a

        [TestMethod]
        public void SimpleUpdateTest()
        {
            var sql = "UPDATE Author SET Name = 'new_name'";

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("UPDATE [Graywulf_Schema_Test].[dbo].[Author] SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name'", res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        [TestMethod]
        public void UpdateWithQualifiedColumnNameTest()
        {
            var sql = "UPDATE Author SET Author.Name = 'new_name'";

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("UPDATE [Graywulf_Schema_Test].[dbo].[Author] SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name'", res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        [TestMethod]
        public void UpdateWithAliasedColumnNameTest()
        {
            var sql = "UPDATE a SET a.Name = 'new_name' FROM Author a";

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("UPDATE [a] SET [a].[Name] = 'new_name' FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);
        }

        [TestMethod]
        public void UpdateWithWhereTest()
        {
            var sql = "UPDATE Author SET Name = 'new_name' WHERE ID = 1";

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual("UPDATE [Graywulf_Schema_Test].[dbo].[Author] SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name' WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = 1", res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
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

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["Author"].DatabaseObjectName);
            Assert.AreEqual(null, ds.SourceTableReferences["Author"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual(null, ds.TargetTable.TableReference.Alias);
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

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["a"].DatabaseObjectName);
            Assert.AreEqual("a", ds.SourceTableReferences["a"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
            Assert.AreEqual("a", ds.TargetTable.TableReference.Alias);
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

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["a"].DatabaseObjectName);
            Assert.AreEqual("a", ds.SourceTableReferences["a"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
        }

        [TestMethod]
        public void UpdateWithNoTableAliasTest2()
        {
            var sql =
@"UPDATE Author
SET Author.Name = 'new_name'
FROM Author AS a";

            var gt =
@"UPDATE [Graywulf_Schema_Test].[dbo].[Author]
SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name'
FROM [Graywulf_Schema_Test].[dbo].[Author] AS [a]";

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            Assert.AreEqual(1, ds.SourceTableReferences.Count);
            Assert.AreEqual("Author", ds.SourceTableReferences["a"].DatabaseObjectName);
            Assert.AreEqual("a", ds.SourceTableReferences["a"].Alias);

            Assert.AreEqual("Author", ds.TargetTable.TableReference.DatabaseObjectName);
        }

        [TestMethod]
        public void UpdateWithJoinTest()
        {
            var sql =
@"UPDATE Author
SET Author.Name = 'new_name'
FROM Author
INNER JOIN Book ON Book.ID = Book.ID";

            var gt =
@"UPDATE [Graywulf_Schema_Test].[dbo].[Author]
SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name'
FROM [Graywulf_Schema_Test].[dbo].[Author]
INNER JOIN [Graywulf_Schema_Test].[dbo].[Book] ON [Graywulf_Schema_Test].[dbo].[Book].[ID] = [Graywulf_Schema_Test].[dbo].[Book].[ID]";

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences.Values.ToArray();
            Assert.AreEqual(2, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        [TestMethod]
        public void UpdateWithSubqueryTest()
        {
            var sql =
@"UPDATE Author
SET Author.Name = 'new_name'
WHERE ID IN (SELECT ID FROM Book)";

            var gt =
@"UPDATE [Graywulf_Schema_Test].[dbo].[Author]
SET [Graywulf_Schema_Test].[dbo].[Author].[Name] = 'new_name'
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] IN (SELECT [Graywulf_Schema_Test].[dbo].[Book].[ID] FROM [Graywulf_Schema_Test].[dbo].[Book])";

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences.Values.ToArray();

            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);
        }

        [TestMethod]
        public void UpdateTableVariableTest()
        {
            var sql =
@"DECLARE @t AS TABLE
(
    ID int,
    Data float
)

UPDATE @t
SET Data = 1
WHERE ID = 2";

            var gt =
@"UPDATE @t
SET [@t].[Data] = 1
WHERE [@t].[ID] = 2";

            var ds = ParseAndResolveNames<UpdateStatement>(sql);

            var res = GenerateCode(ds);
            Assert.AreEqual(gt, res);

            var ts = ds.SourceTableReferences.Values.ToArray();

            Assert.AreEqual(1, ts.Length);
            Assert.AreEqual("@t", ts[0].VariableName);
            Assert.AreEqual(null, ts[0].Alias);
        }
    }
}
