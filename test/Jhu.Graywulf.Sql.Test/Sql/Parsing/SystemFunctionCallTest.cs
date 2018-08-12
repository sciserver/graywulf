using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class SystemFunctionCallTest : ParsingTestBase
    {
        private SystemFunctionCall ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<SystemFunctionCall>(query);
        }

        [TestMethod]
        public void FunctionCallNoArgumentTest()
        {
            var sql = "function()";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FunctionName.Value);
        }

        [TestMethod]
        public void SimpleFunctionCallTest()
        {
            var sql = "function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FunctionName.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
        }

        [TestMethod]
        public void FunctionCallArgumentListTest()
        {
            var sql = "function(a,b,c)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FunctionName.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
            Assert.AreEqual(3, exp.EnumerateDescendantsRecursive<Argument>(null).Count());
        }

        [TestMethod]
        public void FunctionCallWhitespacesTest()
        {
            var sql = "function ( a , b , c )";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FunctionName.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
            Assert.AreEqual(3, exp.EnumerateDescendantsRecursive<Argument>(null).Count());
        }

        [TestMethod]
        public void CreateSystemFunctionCallTest()
        {
            var cg = CreateQueryRenderer();
            var fun = SystemFunctionCall.Create("SIN", Expression.CreateNumber("1.1"));
            Assert.AreEqual("SIN(1.1)", cg.Execute(fun));

            fun = SystemFunctionCall.Create("GETDATE", new Expression[0]);
            Assert.AreEqual("GETDATE()", cg.Execute(fun));
        }
    }
}
