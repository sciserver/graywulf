using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryTraversal;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    [TestClass]
    public class ExpressionTreeBuilderTest
    {
        private LogicalExpression GetSearchCondition(string sql)
        {
            var exp = new SqlParser().Execute<LogicalExpression>(sql);
            return exp;
        }

        private string VisitTestHelper(string sql)
        {
            var exp = GetSearchCondition(sql);
            var tb = new ExpressionTreeBuilder();
            var tree = tb.Execute(exp);
            return tree.GetParsingTree().Value;
        }

        [TestMethod]
        public void SinglePredicateTest()
        {
            var sql = "a = b";
            Assert.AreEqual("a = b", VisitTestHelper(sql));

            sql = "NOT a = b";
            Assert.AreEqual("NOT a = b", VisitTestHelper(sql));
        }

        [TestMethod]
        public void PrecedenceTest()
        {
            var sql = "a = b OR c < d AND e > f";
            Assert.AreEqual("a = b OR c < d AND e > f", VisitTestHelper(sql));

            sql = "NOT a = b AND c > d OR e < F";
            Assert.AreEqual("NOT a = b AND c > d OR e < F", VisitTestHelper(sql));
        }

        [TestMethod]
        public void BracketsTest()
        {
            var sql = "a = b OR c < d AND e > f";
            Assert.AreEqual(sql, VisitTestHelper(sql));

            sql = "(a = b OR c < d) AND e > f";
            Assert.AreEqual(sql, VisitTestHelper(sql));

            sql = "NOT (a = b OR c > d) OR e < F";
            Assert.AreEqual(sql, VisitTestHelper(sql));

            sql = "NOT (a = b AND c > d) OR e < F";
            Assert.AreEqual(sql, VisitTestHelper(sql));

            sql = "NOT ((a = b OR c > d) AND e < F)";
            Assert.AreEqual(sql, VisitTestHelper(sql));
        }
    }
}
