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
    public class ExpressionPrefixTest
    {
        private string Execute(string code)
        {
            var exp = new SqlParser().Execute<Expression>(code);
            return new TestVisitorSink().Execute(exp, ExpressionTraversalMethod.Prefix);
        }

        [TestMethod]
        public void PrecedenceTest()
        {
            var res = Execute("a + b");
            Assert.AreEqual("+ a b ", res);

            res = Execute("a + b + c");
            Assert.AreEqual("+ a + b c ", res);

            res = Execute("a * b + c");
            Assert.AreEqual("+ * a b c ", res);

            res = Execute("a + b * c");
            Assert.AreEqual("+ a * b c ", res);
        }

        [TestMethod]
        public void SamePrecedenceTest()
        {
            var res = Execute("a + b - c");
            Assert.AreEqual("+ a - b c ", res);

            res = Execute("a - b + c");
            Assert.AreEqual("- a + b c ", res);
        }

        [TestMethod]
        public void BracketsTest()
        {
            var res = Execute("(a) + (b)");
            Assert.AreEqual("+ a b ", res);

            res = Execute("(a + b)");
            Assert.AreEqual("+ a b ", res);

            res = Execute("(a + b) * c");
            Assert.AreEqual("* + a b c ", res);

            res = Execute("c * (a + b)");
            Assert.AreEqual("* c + a b ", res);

            res = Execute("(a + b) * (c + d)");
            Assert.AreEqual("* + a b + c d ", res);
        }

        [TestMethod]
        public void UnaryOperatorTest()
        {
            var res = Execute("-a");
            Assert.AreEqual("- a ", res);

            res = Execute("-a + -b");
            Assert.AreEqual("+ - a - b ", res);

            res = Execute("-(a + b)");
            Assert.AreEqual("- + a b ", res);

            res = Execute("--a");
            Assert.AreEqual("- - a ", res);
        }

        [TestMethod]
        public void FunctionTest()
        {
            var res = Execute("f(a)");
            Assert.AreEqual("f `1 a ", res);

            res = Execute("f()");
            Assert.AreEqual("f `0 ", res);

            res = Execute("f(a, b)");
            Assert.AreEqual("f `2 a , b ", res);

            res = Execute("f(a, b) * g(c, d)");
            Assert.AreEqual("* f `2 a , b g `2 c , d ", res);

            res = Execute("f(a, g(b))");
            Assert.AreEqual("f `2 a , g `1 b ", res);

            res = Execute("f(a, g(b)) * (a + b)");
            Assert.AreEqual("* f `2 a , g `1 b + a b ", res);
        }


        [TestMethod]
        public void WindowedFunctionCallTest()
        {
            var res = Execute("ROW_NUMBER() OVER (ORDER BY x)");
            Assert.AreEqual("ROW_NUMBER `0 over ( orderby x ) ", res);

            res = Execute("ROW_NUMBER() OVER (ORDER BY x, y)");
            Assert.AreEqual("ROW_NUMBER `0 over ( orderby x , y ) ", res);

            res = Execute("ROW_NUMBER() OVER (ORDER BY x + b * c)");
            Assert.AreEqual("ROW_NUMBER `0 over ( orderby + x * b c ) ", res);

            res = Execute("ROW_NUMBER() OVER (PARTITION BY z ORDER BY x)");
            Assert.AreEqual("ROW_NUMBER `0 over ( partitionby z orderby x ) ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x)");
            Assert.AreEqual("NTILE `1 over ( orderby x ) 4 ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x, y)");
            Assert.AreEqual("NTILE `1 over ( orderby x , y ) 4 ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x + b * c)");
            Assert.AreEqual("NTILE `1 over ( orderby + x * b c ) 4 ", res);

            res = Execute("NTILE(4) OVER (PARTITION BY z ORDER BY x)");
            Assert.AreEqual("NTILE `1 over ( partitionby z orderby x ) 4 ", res);

            res = Execute("NTILE(4) OVER (PARTITION BY z + 1 ORDER BY x * y + z DESC, w ASC)");
            Assert.AreEqual("NTILE `1 over ( partitionby + z 1 orderby + * x y z , w ) 4 ", res);
        }

        [TestMethod]
        public void UdtStaticPropertyAccessTest()
        {
            var res = Execute("dbo.udt::statprop + b * c");
            Assert.AreEqual("+ :: dbo.udt statprop * b c ", res);

            res = Execute("dbo.udt::statprop * b");
            Assert.AreEqual("* :: dbo.udt statprop b ", res);

            res = Execute("dbo.udt::statprop + dbo.udt2::statprop2");
            Assert.AreEqual("+ :: dbo.udt statprop :: dbo.udt2 statprop2 ", res);
        }

        [TestMethod]
        public void UdtStaticMethodCallTest()
        {
            var res = Execute("udt::statmethod(a)");
            Assert.AreEqual(":: udt statmethod `1 a ", res);

            res = Execute("udt::statmethod()");
            Assert.AreEqual(":: udt statmethod `0 ", res);

            res = Execute("udt::statmethod(a, b)");
            Assert.AreEqual(":: udt statmethod `2 a , b ", res);

            res = Execute("udt::statmethod1(a, b) * udt::statmethod2(c, d)");
            Assert.AreEqual("* :: udt statmethod1 `2 a , b :: udt statmethod2 `2 c , d ", res);

            res = Execute("udt::statmethod1(a, udt::statmethod2(b))");
            Assert.AreEqual(":: udt statmethod1 `2 a , :: udt statmethod2 `1 b ", res);

            res = Execute("udt::statmethod1(a, udt::statmethod2(b)) * (a + b)");
            Assert.AreEqual("* :: udt statmethod1 `2 a , :: udt statmethod2 `1 b + a b ", res);
        }

        [TestMethod]
        public void UdtPropertyAccessAndMethodCallTest()
        {
            var res = Execute("udtcol.method1()");
            Assert.AreEqual(". udtcol method1 `0 ", res);

            res = Execute("udtcol.method1(a, b)");
            Assert.AreEqual(". udtcol method1 `2 a , b ", res);

            res = Execute("udtcol.method1(a, b).prop1.method2(c, d)");
            Assert.AreEqual(". udtcol . method1 `2 a , b . prop1 method2 `2 c , d ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2()).prop2");
            Assert.AreEqual(". udtcol . method1 `2 . a prop1 , . b method2 `0 prop2 ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2(c)).method3()");
            Assert.AreEqual(". udtcol . method1 `2 . a prop1 , . b method2 `1 c method3 `0 ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2(c)).method3(d)");
            Assert.AreEqual(". udtcol . method1 `2 . a prop1 , . b method2 `1 c method3 `1 d ", res);
        }

        [TestMethod]
        public void SubqueryTest()
        {
            var res = Execute("(SELECT 1)");
            Assert.AreEqual("subquery ", res);

            res = Execute("(SELECT TOP 1 udtcol FROM tab1).method1().method2(b, c)");
            Assert.AreEqual(". subquery . method1 `0 method2 `2 b , c ", res);
        }

        [TestMethod]
        public void SimpleCaseTest()
        {
            var res = Execute("CASE x WHEN y THEN z END");
            Assert.AreEqual("case x y z ", res);

            res = Execute("CASE x + 1 WHEN y + 2 THEN z + 3 END");
            Assert.AreEqual("case + x 1 + y 2 + z 3 ", res);

            res = Execute("a + CASE x WHEN y THEN z END * b");
            Assert.AreEqual("+ a * case x y z b ", res);
        }
    }
}
