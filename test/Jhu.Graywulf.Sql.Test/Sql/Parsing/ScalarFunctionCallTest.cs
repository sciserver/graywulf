using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ScalarFunctionCallTest : ParsingTestBase
    {
        private ScalarFunctionCall ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<ScalarFunctionCall>(query);
        }

        [TestMethod]
        public void FunctionCallNoArgumentTest()
        {
            var sql = "function()";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.UdfIdentifier.Value);
        }

        [TestMethod]
        public void SimpleFunctionCallTest()
        {
            var sql = "function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.UdfIdentifier.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
        }

        [TestMethod]
        public void FunctionCallArgumentListTest()
        {
            var sql = "function(a,b,c)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.UdfIdentifier.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
            Assert.AreEqual(3, exp.EnumerateDescendantsRecursive<Argument>(null).Count());
        }

        [TestMethod]
        public void FunctionCallWhitespacesTest()
        {
            var sql = "function ( a , b , c )";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.UdfIdentifier.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Argument>().Value);
            Assert.AreEqual(3, exp.EnumerateDescendantsRecursive<Argument>(null).Count());
        }

        // TODO: add more test width UDF etc.

        [TestMethod]
        public void CreateSystemFunctionCallTest()
        {
            var cg = CreateCodeGenerator();
            var fun = ScalarFunctionCall.CreateSystem("SIN", Expression.CreateNumber("1.1"));
            Assert.AreEqual("SIN(1.1)", cg.Execute(fun));

            fun = ScalarFunctionCall.CreateSystem("GETDATE", null);
            Assert.AreEqual("GETDATE()", cg.Execute(fun));
        }

        [TestMethod]
        public void CreateUdfCallTest()
        {
            var cg = CreateCodeGenerator();
            var fr = new FunctionReference()
            {
                SchemaName = "dbo",
                FunctionName = "test",
                IsUserDefined = true
            };

            var fun = ScalarFunctionCall.Create(fr, Expression.CreateNumber("1.1"));
            Assert.AreEqual("[dbo].[test](1.1)", cg.Execute(fun));

            fun = ScalarFunctionCall.Create(fr, new[] { Expression.CreateNumber("1.1"), Expression.CreateString("'test'") });
            Assert.AreEqual("[dbo].[test](1.1, 'test')", cg.Execute(fun));

            fun = ScalarFunctionCall.Create(fr, null);
            Assert.AreEqual("[dbo].[test]()", cg.Execute(fun));
        }
    }
}
