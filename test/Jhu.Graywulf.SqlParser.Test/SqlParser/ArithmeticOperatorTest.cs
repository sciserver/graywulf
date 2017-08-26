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
    public class ArithmeticOperatorTest
    {
        private Jhu.Graywulf.SqlParser.Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.Expression)p.Execute(new Jhu.Graywulf.SqlParser.Expression(), query);
        }

        [TestMethod]
        public void PlusTest()
        {
            var sql = "a+b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a+b", exp.Value);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnName>().Value);
        }

        // *** TODO: write rest of tests

    }
}
