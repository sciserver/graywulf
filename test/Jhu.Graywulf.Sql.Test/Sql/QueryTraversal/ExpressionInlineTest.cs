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
    public class ExpressionInlineTest
    {
        private string Execute(string code, ExpressionTraversalMethod expressionTraversal, ExpressionTraversalMethod logicalExpressionTraversal)
        {
            var exp = new SqlParser().Execute<LogicalExpression>(code);
            return new TestVisitorSink().Execute(exp, expressionTraversal, logicalExpressionTraversal);
        }

        [TestMethod]
        public void InlinedInfixExpressions()
        {
            var exp = "a + b = 1 OR c + d = 2 AND e + f * g < 0";

            var res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Postfix);
            Assert.AreEqual("a + b = 1 c + d = 2 e + f * g < 0 AND OR ", res);

            res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Prefix);
            Assert.AreEqual("OR a + b = 1 AND c + d = 2 e + f * g < 0 ", res);

            res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("a + b = 1 OR c + d = 2 AND e + f * g < 0 ", res);
        }

        [TestMethod]
        public void InlinedPrefixExpressions()
        {
            var exp = "a + b = 1 OR c + d = 2 AND e + f * g < 0";

            var res = Execute(exp, ExpressionTraversalMethod.Prefix, ExpressionTraversalMethod.Postfix);
            Assert.AreEqual("+ a b = 1 + c d = 2 + e * f g < 0 AND OR ", res);

            res = Execute(exp, ExpressionTraversalMethod.Prefix, ExpressionTraversalMethod.Prefix);
            Assert.AreEqual("OR + a b = 1 AND + c d = 2 + e * f g < 0 ", res);

            res = Execute(exp, ExpressionTraversalMethod.Prefix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("+ a b = 1 OR + c d = 2 AND + e * f g < 0 ", res);
        }

        [TestMethod]
        public void InlinedPostfixExpressions()
        {
            var exp = "a + b = 1 OR c + d = 2 AND e + f * g < 0";

            var res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Postfix);
            Assert.AreEqual("a b + = 1 c d + = 2 e f g * + < 0 AND OR ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Prefix);
            Assert.AreEqual("OR a b + = 1 AND c d + = 2 e f g * + < 0 ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("+ a b = 1 OR + c d = 2 AND + e * f g < 0 ", res);
        }

        // TODO: add expressions containing logical expressions once IIF is implemented
    }
}
