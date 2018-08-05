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
            return new TestVisitorSink().Execute(exp, ExpressionTraversalMethod.Postfix);
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
        public void StarArgumentTest()
        {
            var res = Execute("COUNT(*)");
            Assert.AreEqual("* COUNT `1 ", res);

            res = Execute("COUNT(*) + 3");
            Assert.AreEqual("* COUNT `1 3 + ", res);
        }


        [TestMethod]
        public void WindowedFunctionCallTest()
        {
            var res = Execute("ROW_NUMBER() OVER (ORDER BY x)");
            Assert.AreEqual("over ( orderby x ) ROW_NUMBER `0 ", res);

            res = Execute("ROW_NUMBER() OVER (ORDER BY x, y)");
            Assert.AreEqual("over ( orderby x , y ) ROW_NUMBER `0 ", res);

            res = Execute("ROW_NUMBER() OVER (ORDER BY x + b * c)");
            Assert.AreEqual("over ( orderby x b c * + ) ROW_NUMBER `0 ", res);

            res = Execute("ROW_NUMBER() OVER (PARTITION BY z ORDER BY x)");
            Assert.AreEqual("over ( partitionby z orderby x ) ROW_NUMBER `0 ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x)");
            Assert.AreEqual("4 over ( orderby x ) NTILE `1 ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x, y)");
            Assert.AreEqual("4 over ( orderby x , y ) NTILE `1 ", res);

            res = Execute("NTILE(4) OVER (ORDER BY x + b * c)");
            Assert.AreEqual("4 over ( orderby x b c * + ) NTILE `1 ", res);

            res = Execute("NTILE(4) OVER (PARTITION BY z ORDER BY x)");
            Assert.AreEqual("4 over ( partitionby z orderby x ) NTILE `1 ", res);

            res = Execute("NTILE(4) OVER (PARTITION BY z + 1 ORDER BY x * y + z DESC, w ASC)");
            Assert.AreEqual("4 over ( partitionby z 1 + orderby x y * z + , w ) NTILE `1 ", res);
        }

        [TestMethod]
        public void UdtStaticPropertyAccessTest()
        {
            var res = Execute("dbo.udt::statprop + b * c");
            Assert.AreEqual("dbo.udt statprop :: b c * + ", res);

            res = Execute("dbo.udt::statprop * b");
            Assert.AreEqual("dbo.udt statprop :: b * ", res);

            res = Execute("dbo.udt::statprop + dbo.udt2::statprop2");
            Assert.AreEqual("dbo.udt statprop :: dbo.udt2 statprop2 :: + ", res);
        }

        [TestMethod]
        public void UdtStaticMethodCallTest()
        {
            var res = Execute("udt::statmethod(a)");
            Assert.AreEqual("udt a statmethod `1 :: ", res);

            res = Execute("udt::statmethod()");
            Assert.AreEqual("udt statmethod `0 :: ", res);

            res = Execute("udt::statmethod(a, b)");
            Assert.AreEqual("udt a , b statmethod `2 :: ", res);

            res = Execute("udt::statmethod1(a, b) * udt::statmethod2(c, d)");
            Assert.AreEqual("udt a , b statmethod1 `2 :: udt c , d statmethod2 `2 :: * ", res);

            res = Execute("udt::statmethod1(a, udt::statmethod2(b))");
            Assert.AreEqual("udt a , udt b statmethod2 `1 :: statmethod1 `2 :: ", res);

            res = Execute("udt::statmethod1(a, udt::statmethod2(b)) * (a + b)");
            Assert.AreEqual("udt a , udt b statmethod2 `1 :: statmethod1 `2 :: a b + * ", res);
        }

        [TestMethod]
        public void UdtPropertyAccessAndMethodCallTest()
        {
            var res = Execute("udtcol.method1()");
            Assert.AreEqual("udtcol method1 `0 . ", res);

            res = Execute("udtcol.method1(a, b)");
            Assert.AreEqual("udtcol a , b method1 `2 . ", res);

            res = Execute("udtcol.method1(a, b).prop1.method2(c, d)");
            Assert.AreEqual("udtcol a , b method1 `2 . prop1 . c , d method2 `2 . ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2()).prop2");
            Assert.AreEqual("udtcol a prop1 . , b method2 `0 . method1 `2 . prop2 . ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2(c)).method3()");
            Assert.AreEqual("udtcol a prop1 . , b c method2 `1 . method1 `2 . method3 `0 . ", res);

            res = Execute("udtcol.method1(a.prop1, b.method2(c)).method3(d)");
            Assert.AreEqual("udtcol a prop1 . , b c method2 `1 . method1 `2 . d method3 `1 . ", res);
        }

        [TestMethod]
        public void SubqueryTest()
        {
            var res = Execute("(SELECT a + b)");
            Assert.AreEqual("<subquery> ", res);

            res = Execute("2 * (SELECT a + b) + 3");
            Assert.AreEqual("2 <subquery> * 3 + ", res);

            res = Execute("(SELECT a + b).method(x * y) + z");
            Assert.AreEqual("<subquery> x y * method `1 . z + ", res);
        }

        [TestMethod]
        public void SimpleCaseTest()
        {
            var res = Execute("CASE x WHEN y THEN z END");
            Assert.AreEqual("CASE x WHEN y THEN z END ", res);

            res = Execute("CASE x + 1 WHEN y + 2 THEN z + 3 END");
            Assert.AreEqual("CASE x 1 + WHEN y 2 + THEN z 3 + END ", res);

            res = Execute("a + CASE x WHEN y THEN z END * b");
            Assert.AreEqual("a CASE x WHEN y THEN z END b * + ", res);

            res = Execute("a + CASE x WHEN y THEN z ELSE 0 END * b");
            Assert.AreEqual("a CASE x WHEN y THEN z ELSE 0 END b * + ", res);
        }

        [TestMethod]
        public void SearchedCaseTest()
        {
            var res = Execute("CASE WHEN 1 > y THEN z WHEN y > 2 THEN x END");
            Assert.AreEqual("CASE WHEN 1 > y THEN z WHEN y > 2 THEN x END ", res);

            res = Execute("CASE WHEN x + 1 < y + 2 AND z > 3 THEN z + 3 END");
            Assert.AreEqual("CASE WHEN x 1 + < y 2 + AND z > 3 THEN z 3 + END ", res);

            res = Execute("a + CASE WHEN x = y THEN z END * b");
            Assert.AreEqual("a CASE WHEN x = y THEN z END b * + ", res);

            res = Execute("a + CASE WHEN y = y THEN z ELSE 0 END * b");
            Assert.AreEqual("a CASE WHEN y = y THEN z ELSE 0 END b * + ", res);
        }
    }
}
