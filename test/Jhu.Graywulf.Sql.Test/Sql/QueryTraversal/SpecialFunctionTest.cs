using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    [TestClass]
    public class SpecialFunctionTest
    {
        private string Execute(string code, ExpressionTraversalMethod expressionTraversal, ExpressionTraversalMethod logicalExpressionTraversal)
        {
            var exp = new SqlParser().Execute<Expression>(code);
            return new TestVisitorSink().Execute(exp, expressionTraversal, logicalExpressionTraversal);
        }

        [TestMethod]
        public void SpecialFunctionReshuffleTest()
        {
            var exp = "2 + 3 * CONVERT(nvarchar(50), a + b)";

            var res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 + 3 * CONVERT ( nvarchar(50) , a + b ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 3 nvarchar(50) , a b + CONVERT `2 * + ", res);

            res = Execute(exp, ExpressionTraversalMethod.Prefix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("+ 2 * 3 CONVERT `2 nvarchar(50) , + a b ", res);
        }

        [TestMethod]
        public void InlinedLogicalExpressionTest()
        {
            var exp = "IIF(a < b OR c > d AND e != f, a + b * c, d - e / f)";

            var res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("IIF ( a < b OR c > d AND e != f , a + b * c , d - e / f ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Postfix);
            Assert.AreEqual("IIF ( a < b c > d e != f AND OR , a + b * c , d - e / f ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Prefix);
            Assert.AreEqual("IIF ( OR a < b AND c > d e != f , a + b * c , d - e / f ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Postfix);
            Assert.AreEqual("a < b c > d e != f AND OR , a b c * + , d e f / - IIF `3 ", res);
        }
    }
}
