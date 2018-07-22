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
    public class LogicalExpressionInfixTest
    {
        private string Execute(string code)
        {
            var exp = new SqlParser().Execute<LogicalExpression>(code);
            return new TestVisitorSink().Execute(exp, ExpressionTraversalMethod.Infix);
        }

        [TestMethod]
        public void OperatorsTest()
        {
            var res = Execute("a = b");
            Assert.AreEqual("a = b ", res);

            res = Execute("a = b OR c < d");
            Assert.AreEqual("a = b OR c < d ", res);

            res = Execute("a = b OR c < d AND e > f");
            Assert.AreEqual("a = b OR c < d AND e > f ", res);

            res = Execute("(a = b OR c < d) AND e > f");
            Assert.AreEqual("( a = b OR c < d ) AND e > f ", res);
        }
    }
}
