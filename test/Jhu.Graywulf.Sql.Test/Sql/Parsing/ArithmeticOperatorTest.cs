﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ArithmeticOperatorTest
    {
        private Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<Expression>(query);
        }

        [TestMethod]
        public void PlusTest()
        {
            var sql = "a+b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a+b", exp.Value);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Operand>().Value);
        }

        // *** TODO: write rest of tests

    }
}
