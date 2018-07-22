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
    public class LogicalExpressionPostfixTest
    {
        private string Execute(string code)
        {
            var exp = new SqlParser().Execute<LogicalExpression>(code);
            return new TestVisitorSink().Execute(exp, ExpressionTraversalMethod.Postfix);
        }

        [TestMethod]
        public void OperatorPrecedenceTest()
        {
            var res = Execute("a = b");
            Assert.AreEqual("a = b ", res);

            res = Execute("a = b OR c < d");
            Assert.AreEqual("a = b c < d OR ", res);

            res = Execute("a = b OR c < d AND e > f");
            Assert.AreEqual("a = b c < d e > f AND OR ", res);

            res = Execute("a = b OR NOT c < d AND NOT e > f");
            Assert.AreEqual("a = b c < d NOT e > f NOT AND OR ", res);

            res = Execute("a = b AND c < d OR e > f");
            Assert.AreEqual("a = b c < d AND e > f OR ", res);

            res = Execute("(a = b OR c < d) AND e > f");
            Assert.AreEqual("a = b c < d OR e > f AND ", res);

            res = Execute("NOT (a = b OR c < d) AND e > f");
            Assert.AreEqual("a = b c < d OR NOT e > f AND ", res);
        }

        [TestMethod]
        public void PredicateTypesTest()
        {
            var res = Execute("a = b OR c < d");
            Assert.AreEqual("a = b c < d OR ", res);

            res = Execute("'a' LIKE 'b' OR a > b AND c BETWEEN d AND e");
            Assert.AreEqual("'a' LIKE 'b' a > b c BETWEEN d AND e AND OR ", res);

            res = Execute("a = b OR c NOT BETWEEN e AND d");
            Assert.AreEqual("a = b c NOT BETWEEN e AND d OR ", res);

            res = Execute("a + b NOT LIKE 'c' OR d > e");
            Assert.AreEqual("a + b NOT LIKE 'c' d > e OR ", res);

            res = Execute("a IS NULL OR b IS NOT NULL");
            Assert.AreEqual("a IS NULL b IS NOT NULL OR ", res);

            res = Execute("a IN (1, 3) AND b NOT IN (4, 5)");
            Assert.AreEqual("a IN ( 1 , 3 ) b NOT IN ( 4 , 5 ) AND ", res);
        }
    }
}
