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
            Assert.AreEqual(", a f ", res);

            res = Execute("f()");
            Assert.AreEqual("f ", res);

            res = Execute("f(a, b)");
            Assert.AreEqual(", a , b f ", res);

            res = Execute("f(a, b) * g(c, d)");
            Assert.AreEqual(", a , b f , c , d g * ", res);

            res = Execute("f(a, g(b))");
            Assert.AreEqual(", a , , b g f ", res);

            res = Execute("f(a, g(b)) * (a + b)");
            Assert.AreEqual(", a , , b g f a b + * ", res);
        }


        [TestMethod]
        public void WindowedFunctionCallTest()
        {
            var res = Execute("ROW_NUMBER() OVER (ORDER BY x)");
            Assert.AreEqual("( orderby , x ) over ROW_NUMBER ", res);

            res = Execute("ROW_NUMBER() OVER (ORDER BY x, y)");
            Assert.AreEqual("( orderby , x , y ) over ROW_NUMBER ", res);

            res = Execute("ROW_NUMBER() OVER (ORDER BY x + b * c)");
            Assert.AreEqual("( orderby , x b c * + ) over ROW_NUMBER ", res);

            res = Execute("ROW_NUMBER() OVER (PARTITION BY z ORDER BY x)");
            Assert.AreEqual("( partitionby , z orderby , x ) over ROW_NUMBER ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x)");
            Assert.AreEqual(", 4 ( orderby , x ) over NTILE ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x, y)");
            Assert.AreEqual(", 4 ( orderby , x , y ) over NTILE ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x + b * c)");
            Assert.AreEqual(", 4 ( orderby , x b c * + ) over NTILE ", res);

            res = Execute("NTILE(4) OVER (PARTITION BY z ORDER BY x)");
            Assert.AreEqual(", 4 ( partitionby , z orderby , x ) over NTILE ", res);
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
            Assert.AreEqual(", a udt ::statmethod ", res);

            res = Execute("udt::statmethod()");
            Assert.AreEqual("udt ::statmethod ", res);

            res = Execute("udt::statmethod(a, b)");
            Assert.AreEqual(", a , b udt ::statmethod ", res);

            res = Execute("udt::statmethod1(a, b) * udt::statmethod2(c, d)");
            Assert.AreEqual(", a , b udt ::statmethod1 , c , d udt ::statmethod2 * ", res);

            res = Execute("udt::statmethod1(a, udt::statmethod2(b))");
            Assert.AreEqual(", a , , b udt ::statmethod2 udt ::statmethod1 ", res);

            res = Execute("fudt::statmethod1(a, udt::statmethod2(b)) * (a + b)");
            Assert.AreEqual(", a , , b udt ::statmethod2 fudt ::statmethod1 a b + * ", res);
        }

        [TestMethod]
        public void UdtPropertyAccessAndMethodCallTest()
        {
            // TODO: how to deal with resolved multi-part identifiers?
            // the resolves should maybe rewrite the parsing tree?

            var res = Execute("udtcol.method1()");
            Assert.AreEqual("", res);

        }

    }
}
