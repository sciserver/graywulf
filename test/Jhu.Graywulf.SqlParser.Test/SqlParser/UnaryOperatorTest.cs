using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser.Test
{
    [TestClass]
    public class UnaryOperatorTest
    {
        private Jhu.Graywulf.SqlParser.Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.Expression)p.Execute(new Jhu.Graywulf.SqlParser.Expression(), query);
        }

        [TestMethod]
        public void MinusSignTest()
        {
            var sql = "-1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("-", exp.FindDescendantRecursive<Minus>().ToString());
            Assert.AreEqual("1", exp.FindDescendantRecursive<Number>().ToString());
        }

        [TestMethod]
        public void PlusSignTest()
        {
            var sql = "+1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().ToString());
            Assert.AreEqual("1", exp.FindDescendantRecursive<Number>().ToString());
        }

        [TestMethod]
        public void BitwiseNotTest()
        {
            var sql = "~1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("~", exp.FindDescendantRecursive<BitwiseNot>().ToString());
            Assert.AreEqual("1", exp.FindDescendantRecursive<Number>().ToString());
        }

        [TestMethod]
        public void WhitespaceTest()
        {
            var sql = "+ 1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("+ 1", exp.ToString());
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().ToString());
            Assert.AreEqual("1", exp.FindDescendantRecursive<Number>().ToString());
        }
    
    }
}
