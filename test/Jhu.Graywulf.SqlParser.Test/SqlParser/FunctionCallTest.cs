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
    public class FunctionCallTest
    {
        private Jhu.Graywulf.SqlParser.FunctionCall ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.FunctionCall)p.Execute(new Jhu.Graywulf.SqlParser.FunctionCall(), query);
        }

        [TestMethod]
        public void FunctionCallNoArgumentTest()
        {
            var sql = "function()";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
        }

        [TestMethod]
        public void SimpleFunctionCallTest()
        {
            var sql = "function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
        }

        [TestMethod]
        public void FunctionCallArgumentListTest()
        {
            var sql = "function(a,b,c)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
            Assert.AreEqual(3, exp.EnumerateDescendantsRecursive<Argument>(null).Count());
        }

        [TestMethod]
        public void FunctionCallWhitespacesTest()
        {
            var sql = "function ( a , b , c )";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
            Assert.AreEqual(3, exp.EnumerateDescendantsRecursive<Argument>(null).Count());
        }
    }
}
