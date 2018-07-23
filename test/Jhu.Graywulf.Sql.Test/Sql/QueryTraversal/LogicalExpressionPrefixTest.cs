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
    public class LogicalExpressionPrefixTest
    {
        private string Execute(string code)
        {
            var exp = new SqlParser().Execute<LogicalExpression>(code);
            return new TestVisitorSink().Execute(exp, ExpressionTraversalMethod.Prefix);
        }

        [TestMethod]
        public void OperatorPrecedenceTest()
        {
            var res = Execute("a = b");
            Assert.AreEqual("a = b ", res);

            res = Execute("a = b OR c < d");
            Assert.AreEqual("OR a = b c < d ", res);

            res = Execute("a = b OR c < d AND e > f");
            Assert.AreEqual("OR a = b AND c < d e > f ", res);

            res = Execute("a = b OR NOT c < d AND NOT e > f");
            Assert.AreEqual("OR a = b AND NOT c < d NOT e > f ", res);

            res = Execute("a = b AND c < d OR e > f");
            Assert.AreEqual("OR AND a = b c < d e > f ", res);

            res = Execute("(a = b OR c < d) AND e > f");
            Assert.AreEqual("AND OR a = b c < d e > f ", res);

            res = Execute("NOT (a = b OR c < d) AND e > f");
            Assert.AreEqual("AND NOT OR a = b c < d e > f ", res);
        }

        [TestMethod]
        public void PredicateTypesTest()
        {
            var res = Execute("a = b OR c < d");
            Assert.AreEqual("OR a = b c < d ", res);

            res = Execute("'a' LIKE 'b' OR a > b AND c BETWEEN d AND e");
            Assert.AreEqual("OR 'a' LIKE 'b' AND a > b c BETWEEN d AND e ", res);

            res = Execute("a = b OR c NOT BETWEEN e AND d");
            Assert.AreEqual("OR a = b c NOT BETWEEN e AND d ", res);

            res = Execute("a + b NOT LIKE 'c' OR d > e");
            Assert.AreEqual("OR a + b NOT LIKE 'c' d > e ", res);

            res = Execute("a IS NULL OR b IS NOT NULL");
            Assert.AreEqual("OR a IS NULL b IS NOT NULL ", res);

            res = Execute("a IN (1, 3) AND b NOT IN (4, 5)");
            Assert.AreEqual("AND a IN ( 1 , 3 ) b NOT IN ( 4 , 5 ) ", res);
        }
    }
}
