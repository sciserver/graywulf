using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class FunctionArgumentsTest
    {
        private ScalarFunctionCall ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<ScalarFunctionCall>(query);
        }

        [TestMethod]
        public void SingleArgumentTest()
        {
            var sql = "function(a)";
            var exp = ExpressionTestHelper(sql);
            var args = exp.EnumerateArguments().ToArray();
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual(1, args.Length);

            sql = "function ( a )";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void MultipleArgumentsTest()
        {
            var sql = "function(a,b,c,d)";
            var exp = ExpressionTestHelper(sql);
            var args = exp.EnumerateArguments().ToArray();
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual(4, args.Length);

            sql = "function ( a , b , c, d )";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void ArgumentExpressionsTest()
        {
            var sql = "function(a+b,b+1,(c),((d)))";
            var exp = ExpressionTestHelper(sql);

            sql = "function ( a + b , b + 1 , c.d().e.f.g() , ( ( d ) ) )";
            exp = ExpressionTestHelper(sql);
        }

    }
}
