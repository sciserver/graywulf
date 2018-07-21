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
    public class ExpressionPostfixTest
    {
        private string Execute(string code)
        {
            var exp = new SqlParser().Execute<Expression>(code);
            return new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Postfix);
        }

        [TestMethod]
        public void PrecedenceTest()
        {
            var res = Execute("a + b");
            Assert.AreEqual("a b + ", res);

            res = Execute("a + b + c");
            Assert.AreEqual("a b + c + ", res);

            res = Execute("a * b + c");
            Assert.AreEqual("a b * c + ", res);

            res = Execute("a + b * c");
            Assert.AreEqual("a b c * + ", res);
        }

        [TestMethod]
        public void SamePrecedenceTest()
        {
            var res = Execute("a + b - c");
            Assert.AreEqual("a b + c - ", res);

            res = Execute("a - b + c");
            Assert.AreEqual("a b - c + ", res);
        }

        [TestMethod]
        public void BracketsTest()
        {
            var res = Execute("(a) + (b)");
            Assert.AreEqual("a b + ", res);

            res = Execute("(a + b)");
            Assert.AreEqual("a b + ", res);

            res = Execute("(a + b) * c");
            Assert.AreEqual("a b + c * ", res);

            res = Execute("c * (a + b)");
            Assert.AreEqual("c a b + * ", res);

            res = Execute("(a + b) * (c + d)");
            Assert.AreEqual("a b + c d + * ", res);
        }

        [TestMethod]
        public void UnaryOperatorTest()
        {
            var res = Execute("-a");
            Assert.AreEqual("a - ", res);

            res = Execute("-a + -b");
            Assert.AreEqual("a - b - + ", res);

            res = Execute("-(a + b)");
            Assert.AreEqual("a b + - ", res);

            res = Execute("--a");
            Assert.AreEqual("a - - ", res);
        }

        [TestMethod]
        public void FunctionTest()
        {
            var res = Execute("f(a)");
            Assert.AreEqual("a f `1 ", res);

            res = Execute("f()");
            Assert.AreEqual("f `0 ", res);

            res = Execute("f(a, b)");
            Assert.AreEqual("a , b f `2 ", res);

            res = Execute("f(a, b) * g(c, d)");
            Assert.AreEqual("a , b f `2 c , d g `2 * ", res);

            res = Execute("f(a, g(b))");
            Assert.AreEqual("a , b g `1 f `2 ", res);

            res = Execute("f(a, g(b)) * (a + b)");
            Assert.AreEqual("a , b g `1 f `2 a b + * ", res);
        }


        [TestMethod]
        public void WindowedFunctionCallTest()
        {
            var res = Execute("ROW_NUMBER() OVER (ORDER BY x)");
            Assert.AreEqual("( orderby x ) over ROW_NUMBER `0 ", res);

            res = Execute("ROW_NUMBER() OVER (ORDER BY x, y)");
            Assert.AreEqual("( orderby x , y ) over ROW_NUMBER `0 ", res);

            res = Execute("ROW_NUMBER() OVER (ORDER BY x + b * c)");
            Assert.AreEqual("( orderby x b c * + ) over ROW_NUMBER `0 ", res);

            res = Execute("ROW_NUMBER() OVER (PARTITION BY z ORDER BY x)");
            Assert.AreEqual("( partitionby z orderby x ) over ROW_NUMBER `0 ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x)");
            Assert.AreEqual("4 ( orderby x ) over NTILE `1 ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x, y)");
            Assert.AreEqual("4 ( orderby x , y ) over NTILE `1 ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x + b * c)");
            Assert.AreEqual("4 ( orderby x b c * + ) over NTILE `1 ", res);

            res = Execute("NTILE(4) OVER (PARTITION BY z ORDER BY x)");
            Assert.AreEqual("4 ( partitionby z orderby x ) over NTILE `1 ", res);
        }

        [TestMethod]
        public void UdtStaticPropertyAccessTest()
        {
            var res = Execute("dbo.udt::statprop + b * c");
            Assert.AreEqual("dbo.udt ::statprop b c * + ", res);

            res = Execute("dbo.udt::statprop * b");
            Assert.AreEqual("dbo.udt ::statprop b * ", res);
        }

        [TestMethod]
        public void UdtStaticMethodCallTest()
        {
            var res = Execute("udt::statmethod(a)");
            Assert.AreEqual("a udt ::statmethod `1 ", res);

            res = Execute("udt::statmethod()");
            Assert.AreEqual("udt ::statmethod `0 ", res);

            res = Execute("udt::statmethod(a, b)");
            Assert.AreEqual("a , b udt ::statmethod `2 ", res);

            res = Execute("udt::statmethod1(a, b) * udt::statmethod2(c, d)");
            Assert.AreEqual("a , b udt ::statmethod1 `2 c , d udt ::statmethod2 `2 * ", res);

            res = Execute("udt::statmethod1(a, udt::statmethod2(b))");
            Assert.AreEqual("a , b udt ::statmethod2 `1 udt ::statmethod1 `2 ", res);

            res = Execute("fudt::statmethod1(a, udt::statmethod2(b)) * (a + b)");
            Assert.AreEqual("a , b udt ::statmethod2 `1 fudt ::statmethod1 `2 a b + * ", res);
        }

        [TestMethod]
        public void UdtPropertyAccessAndMethodCallTest()
        {
            var res = Execute("udtcol.method1()");
            Assert.AreEqual("udtcol .method1 `0 ", res);

            res = Execute("udtcol.method1(a, b)");
            Assert.AreEqual("udtcol a , b .method1 `2 ", res);

            res = Execute("udtcol.method1(a, b).prop1.method2(c, d)");
            Assert.AreEqual("udtcol a , b .method1 `2 .prop1 c , d .method2 `2 ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2()).prop2");
            Assert.AreEqual("udtcol a .prop1 , b .method2 `0 .method1 `2 .prop2 ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2(c)).method3()");
            Assert.AreEqual("udtcol a .prop1 , b c .method2 `1 .method1 `2 .method3 `0 ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2(c)).method3(d)");
            Assert.AreEqual("udtcol a .prop1 , b c .method2 `1 .method1 `2 d .method3 `1 ", res);
        }


        // TODO: tests with OVER and properties/methods
    }
}
