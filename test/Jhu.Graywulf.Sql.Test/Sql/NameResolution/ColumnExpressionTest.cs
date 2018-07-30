using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class ColumnExpressionTest : SqlNameResolverTestBase
    {
        // This tests can be executed only after name resolution because
        // the column reference of column expressions can only be created
        // after multi-part identifiers are resolved

        [TestMethod]
        public void SetUserVariableTest()
        {
            var ce = ParseAndResolveNames<ColumnExpression>("DECLARE @var int; SELECT @var = 1");
            Assert.AreEqual("@var", ce.AssignedVariable.Value);
            Assert.AreEqual("1", ce.Expression.Value);
        }

        [TestMethod]
        public void SetColumnAliasTest()
        {
            var ce = ParseAndResolveNames<ColumnExpression>("SELECT alias = Author.ID FROM Author");
            Assert.AreEqual("alias", ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("Author.ID", ce.Expression.Value);
        }

        [TestMethod]
        public void SimpleColumnWithoutAliasTest()
        {
            var ce = ParseAndResolveNames<ColumnExpression>("SELECT Author.ID FROM Author");
            Assert.AreEqual(null, ce.ColumnReference.ColumnAlias);
            Assert.AreEqual("Author.ID", ce.Expression.Value);
        }
    }
}
