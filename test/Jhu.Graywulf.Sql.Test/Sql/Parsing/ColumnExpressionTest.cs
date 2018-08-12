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
        [TestMethod]
        public void SetUserVariableTest()
        {
            var ce = Parse<ColumnExpression>("@var = 1");
            Assert.AreEqual("@var", ce.AssignedVariable.Value);
            Assert.AreEqual("1", ce.Expression.Value);
        }

        [TestMethod]
        public void SetColumnAliasTest()
        {
            var ce = Parse<ColumnExpression>("alias = tab.col");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);
        }

        [TestMethod]
        public void SimpleColumnWithoutAliasTest()
        {
            var ce = Parse<ColumnExpression>("tab.col");
            Assert.AreEqual(null, ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);
        }

        [TestMethod]
        public void SimpleColumnAliasTest()
        {
            var ce = Parse<ColumnExpression>("tab.col alias");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);

            ce = Parse<ColumnExpression>("tab.col[alias]");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);

            ce = Parse<ColumnExpression>("tab.col AS alias");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);

            ce = Parse<ColumnExpression>("tab.col AS [alias]");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.col", ce.Expression.Value);

            ce = Parse<ColumnExpression>("tab.[col]AS[alias]");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("tab.[col]", ce.Expression.Value);
        }

        [TestMethod]
        public void CreateSingleStarTest()
        {
            var ce = ColumnExpression.CreateStar();
            Assert.AreEqual("*", ce.Value);
            Assert.IsTrue(ce.ColumnReference.IsStar);
            Assert.IsTrue(ce.ColumnReference.TableReference.IsUndefined);
        }

        [TestMethod]
        public void CreateTableStarTest()
        {
            var cg = CreateQueryRenderer();
            var tr = new NameResolution.TableReference()
            {
                TableName = "test"
            };
            var ce = ColumnExpression.CreateStar(tr);
            Assert.AreEqual("[test].*", cg.Execute(ce));
            Assert.IsTrue(ce.ColumnReference.IsStar);
            Assert.AreEqual("test", ce.ColumnReference.TableReference.TableName);
        }

        [TestMethod]
        public void CreateFromExpressionTest()
        {
            var exp = new SqlParser().Execute<Expression>("a + b");
            var ce = ColumnExpression.Create(exp, "alias");
            Assert.AreEqual("a + b AS alias", ce.Value);
        }

        [TestMethod]
        public void IsSingleColumnTest()
        {
            var sql = "a+b";
            var ce = Parse<ColumnExpression>(sql);
            Assert.IsFalse(ce.Expression.IsSingleColumn);

            sql = "12";
            ce = Parse<ColumnExpression>(sql);
            Assert.IsFalse(ce.Expression.IsSingleColumn);

            sql = "@a";
            ce = Parse<ColumnExpression>(sql);
            Assert.IsFalse(ce.Expression.IsSingleColumn);

            sql = "udt::Method()";
            ce = Parse<ColumnExpression>(sql);
            Assert.IsFalse(ce.Expression.IsSingleColumn);
        }
    }
}
