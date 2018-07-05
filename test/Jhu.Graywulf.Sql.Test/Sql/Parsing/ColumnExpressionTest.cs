using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ColumnExpressionTest : ParsingTestBase
    {
        private ColumnExpression Parse(string query)
        {
            var p = new SqlParser();
            var cd = p.Execute<ColumnExpression>(query);
            return cd;
        }

        [TestMethod]
        public void SetUserVariableTest()
        {
            var ce = Parse("@var = 1");
            Assert.AreEqual("@var", ce.AssignedVariable.Value);
            Assert.AreEqual("1", ce.Expression.Value);
        }

        [TestMethod]
        public void SetColumnAliasTest()
        {
            var ce = Parse("alias = tab.col");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);
        }

        [TestMethod]
        public void SimpleColumnWithoutAliasTest()
        {
            var ce = Parse("tab.col");
            Assert.AreEqual(null, ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);
        }

        [TestMethod]
        public void SimpleColumnAliasTest()
        {
            var ce = Parse("tab.col alias");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);

            ce = Parse("tab.col[alias]");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);

            ce = Parse("tab.col AS alias");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);

            ce = Parse("tab.col AS [alias]");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);

            ce = Parse("tab.[col]AS[alias]");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.[col]", ce.Expression.Value);
        }

        [TestMethod]
        public void CreateSingleStarTest()
        {
            var ce = ColumnExpression.CreateStar();
            Assert.AreEqual("*", ce.Value);
            Assert.IsTrue(ce.ColumnReference.IsStar);

            // TODO: this is fishy, this doesn't supposed to have a table reference
            // because it is the exported column
            Assert.IsTrue(ce.ColumnReference.ParentTableReference.IsUndefined);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CreateTableStarTest()
        {
            var cg = CreateCodeGenerator();
            var tr = new NameResolution.TableReference()
            {
                TableName = "test"
            };
            var ce = ColumnExpression.CreateStar(tr);
            Assert.AreEqual("[test].*", cg.Execute(ce));
            Assert.IsTrue(ce.ColumnReference.IsStar);

            // TODO: this is fishy, this doesn't supposed to have a table reference
            // because it is the exported column
            Assert.AreEqual("test", ce.ColumnReference.ParentTableReference.TableName);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CreateFromExpressionTest()
        {
            var exp = new SqlParser().Execute<Expression>("a + b");
            var ce = ColumnExpression.Create(exp, "alias");
            Assert.AreEqual("a + b AS alias", ce.Value);
        }
    }
}
