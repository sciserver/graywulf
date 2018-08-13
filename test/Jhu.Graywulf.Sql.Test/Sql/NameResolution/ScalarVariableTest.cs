using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryTraversal;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class ScalarVariableTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void DeclareSingleVariableTest()
        {
            var query = ParseAndResolveNames("DECLARE @var int");
            var d = query.ParsingTree.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(1, query.VariableReferences.Count);
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var"));
        }

        [TestMethod]
        public void DeclareSimpleUDTVariableTest()
        {
            var query = ParseAndResolveNames("DECLARE @var SimpleUDT");
            var d = query.ParsingTree.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(1, query.VariableReferences.Count);
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var"));

            query = ParseAndResolveNames("DECLARE @var dbo.SimpleUDT");
            d = query.ParsingTree.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(1, query.VariableReferences.Count);
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var"));
        }

        [TestMethod]
        public void DeclareClrUDTVariableTest()
        {
            var query = ParseAndResolveNames("DECLARE @var ClrUDT");
            var d = query.ParsingTree.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(1, query.VariableReferences.Count);
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var"));

            query = ParseAndResolveNames("DECLARE @var dbo.ClrUDT");
            d = query.ParsingTree.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(1, query.VariableReferences.Count);
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var"));
        }

        [TestMethod]
        public void DeclareMultipleVariableTest()
        {
            var query = ParseAndResolveNames("DECLARE @var1 int, @var2 float");
            var d = query.ParsingTree.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(2, query.VariableReferences.Count);
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var1"));
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var2"));

            query = ParseAndResolveNames(
@"DECLARE @var1 int
DECLARE @var2 float");
            d = query.ParsingTree.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(2, query.VariableReferences.Count);
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var1"));
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var2"));
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void DuplicateVariableTest()
        {
            var script = ParseAndResolveNames("DECLARE @var1 int, @var1 float");
        }

        private void ReferencedVariableHelper(string name, string sql)
        {
            var query = ParseAndResolveNames(sql);
            var s = query.ParsingTree.FindDescendantRecursive<SelectStatement>();
            var v = s.FindDescendantRecursive<UserVariable>();
            Assert.AreEqual(query.VariableReferences[name], v.VariableReference);
        }

        [TestMethod]
        public void SelectListReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT @var1");

            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT @var1 = 6");

            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT @var1 AS vv");
        }

        [TestMethod]
        public void WhereClauseReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT 1 WHERE @var1 = 5");
        }

        [TestMethod]
        public void GroupByReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT 1 GROUP BY @var1");
        }

        [TestMethod]
        public void HavingReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT 1 GROUP BY 2 HAVING AVG(@var1) = 2");
        }

        [TestMethod]
        public void OrderByReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT 1 ORDER BY @var1");
        }

        [TestMethod]
        public void SetNumberTest()
        {
            var sql =
@"DECLARE @var int
SET @var = 5";

            var ss = ParseAndResolveNames(sql);
        }

        [TestMethod]
        public void SetStringTest()
        {
            var sql =
@"DECLARE @var nvarchar(50)
SET @var = 'this is a text'";

            var ss = ParseAndResolveNames(sql);
        }

        [TestMethod]
        public void InitFromQueryTest()
        {
            var sql = @"DECLARE @var int = (SELECT TOP 1 ID FROM Author a)";
            var query = ParseAndResolveNames(sql);

            var sq = query.ParsingTree.FindDescendantRecursive<Subquery>();
            var qs = sq.QueryExpression.EnumerateQuerySpecifications().FirstOrDefault();
            Assert.AreEqual(1, qs.SourceTableReferences.Count);
            Assert.AreEqual("Author", qs.SourceTableReferences["a"].DatabaseObjectName);
            Assert.AreEqual(2, qs.SourceTableReferences["a"].ColumnReferences.Count);
            Assert.AreEqual(ColumnContext.Expression | ColumnContext.SelectList | ColumnContext.PrimaryKey, qs.SourceTableReferences["a"].ColumnReferences[0].Value.ColumnContext);
        }

        [TestMethod]
        public void InitFromQueryTest2()
        {
            var sql = @"DECLARE @var int = (SELECT TOP 1 ID FROM (SELECT * FROM Author) a)";
            var query = ParseAndResolveNames(sql);
            
            var sq = query.ParsingTree.FindDescendantRecursive<Subquery>();
            var qs = sq.QueryExpression.EnumerateQuerySpecifications().FirstOrDefault();
            Assert.AreEqual(1, qs.SourceTableReferences.Count);
            Assert.AreEqual("a", qs.SourceTableReferences["a"].Alias);
            Assert.AreEqual(2, qs.SourceTableReferences["a"].ColumnReferences.Count);
            Assert.AreEqual(ColumnContext.Expression | ColumnContext.SelectList | ColumnContext.PrimaryKey, qs.SourceTableReferences["a"].ColumnReferences[0].Value.ColumnContext);
        }

        [TestMethod]
        public void SetFromQueryTest()
        {
            var sql =
@"DECLARE @var int
SET @var = (SELECT 1)";

            var ss = ParseAndResolveNames(sql);
        }

        [TestMethod]
        public void SelectFromQueryTest()
        {
            var sql =
@"DECLARE @var int
SELECT @var = 1";

            var ss = ParseAndResolveNames(sql);
        }
    }
}
