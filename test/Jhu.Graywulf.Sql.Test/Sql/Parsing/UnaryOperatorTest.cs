using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class UnaryOperatorTest
    {
        private Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<Expression>(query);
        }

        [TestMethod]
        public void MinusSignTest()
        {
            var sql = "-1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("-", exp.FindDescendantRecursive<Minus>().Value);
            Assert.AreEqual("1", exp.FindDescendantRecursive<NumericConstant>().Value);
        }

        [TestMethod]
        public void PlusSignTest()
        {
            var sql = "+1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
            Assert.AreEqual("1", exp.FindDescendantRecursive<NumericConstant>().Value);
        }

        [TestMethod]
        public void BitwiseNotTest()
        {
            var sql = "~1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("~", exp.FindDescendantRecursive<BitwiseNot>().Value);
            Assert.AreEqual("1", exp.FindDescendantRecursive<NumericConstant>().Value);
        }

        [TestMethod]
        public void WhitespaceTest()
        {
            var sql = "+ 1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("+ 1", exp.Value);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
            Assert.AreEqual("1", exp.FindDescendantRecursive<NumericConstant>().Value);
        }
    
    }
}
