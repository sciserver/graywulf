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
    public class ExpressionInfixTest
    {
        private string Execute(string code)
        {
            var exp = new SqlParser().Execute<Expression>(code);
            return new TestVisitorSink().Execute(exp, ExpressionTraversalMethod.Infix);
        }

        [TestMethod]
        public void OperatorsTest()
        {
            var res = Execute("a + b * -c + d - (e + f) * g");
            Assert.AreEqual("a + b * - c + d - ( e + f ) * g ", res);

            res = Execute("1 + @b * -@@c + COUNT(*) - (e + f) * g");
            Assert.AreEqual("1 + @b * - @@c + COUNT(*) - ( e + f ) * g ", res);

            res = Execute("a ^ b % ~c | d & (e + f) * g");
            Assert.AreEqual("a ^ b % ~ c | d & ( e + f ) * g ", res);
        }

        [TestMethod]
        public void FunctionTest1()
        {
            var res = Execute("f2()");
            Assert.AreEqual("f2 ( ) ", res);

            res = Execute("f2(a)");
            Assert.AreEqual("f2 ( a ) ", res);

            res = Execute("f2(a, b, c)");
            Assert.AreEqual("f2 ( a , b , c ) ", res);
        }

        [TestMethod]
        public void FunctionTest2()
        {
            var res = Execute("a * f1(b) + f2(a + b * c, d, e)");
            Assert.AreEqual("a * f1 ( b ) + f2 ( a + b * c , d , e ) ", res);
        }

        [TestMethod]
        public void UdtStaticPropertyTest()
        {
            var res = Execute("dbo.udt::prop1");
            Assert.AreEqual("dbo.udt :: prop1 ", res);

            res = Execute("dbo.udt::prop1.prop2");
            Assert.AreEqual("dbo.udt :: prop1 . prop2 ", res);

            res = Execute("a + dbo.udt::prop1.prop2 * b");
            Assert.AreEqual("a + dbo.udt :: prop1 . prop2 * b ", res);
        }

        [TestMethod]
        public void UdtPropertyTest()
        {
            var res = Execute("@a.prop1");
            Assert.AreEqual("@a . prop1 ", res);

            res = Execute("@a.prop1.prop2");
            Assert.AreEqual("@a . prop1 . prop2 ", res);

            res = Execute("(a + b).prop1.prop2");
            Assert.AreEqual("( a + b ) . prop1 . prop2 ", res);

            res = Execute("(a + b).prop1.prop2.prop3");
            Assert.AreEqual("( a + b ) . prop1 . prop2 . prop3 ", res);

            res = Execute("(SELECT TOP 1 udtcol FROM tab1).prop1.prop2");
            Assert.AreEqual("subquery . prop1 . prop2 ", res);
        }

        [TestMethod]
        public void StaticUdtMethodTest()
        {
            var res = Execute("dbo.udt::method1()");
            Assert.AreEqual("dbo.udt :: method1 ( ) ", res);

            res = Execute("dbo.udt::method1(a, b)");
            Assert.AreEqual("dbo.udt :: method1 ( a , b ) ", res);

            res = Execute("a + dbo.udt::method1().method2(a) * b");
            Assert.AreEqual("a + dbo.udt :: method1 ( ) . method2 ( a ) * b ", res);
        }

        [TestMethod]
        public void UdtMethodTest()
        {
            var res = Execute("@a.method1()");
            Assert.AreEqual("@a . method1 ( ) ", res);

            res = Execute("@a.method1().method2(b, c)");
            Assert.AreEqual("@a . method1 ( ) . method2 ( b , c ) ", res);

            res = Execute("(a + b).method1().method2(b, c)");
            Assert.AreEqual("( a + b ) . method1 ( ) . method2 ( b , c ) ", res);

            res = Execute("(a + b).method1((c * d).method2(e, f))");
            Assert.AreEqual("( a + b ) . method1 ( ( c * d ) . method2 ( e , f ) ) ", res);
        }

        [TestMethod]
        public void UdtMembersTest()
        {
            var res = Execute("@a.prop1.method1(b, c).prop2.prop3.method2()");
            Assert.AreEqual("@a . prop1 . method1 ( b , c ) . prop2 . prop3 . method2 ( ) ", res);
        }

        [TestMethod]
        public void WindowedFunctionCallTest()
        {
            var res = Execute("a + ROW_NUMBER() OVER (PARTITION BY a ORDER BY c ASC, d DESC) + e");
            Assert.AreEqual("a + ROW_NUMBER ( ) over ( partitionby a orderby c , d ) + e ", res);

            res = Execute("a + NTILE(4) OVER (PARTITION BY a ORDER BY c ASC, d DESC) + e");
            Assert.AreEqual("a + NTILE ( 4 ) over ( partitionby a orderby c , d ) + e ", res);

            res = Execute("a + NTILE(4) OVER (PARTITION BY a * b ORDER BY c + 1 ASC, d DESC) + e");
            Assert.AreEqual("a + NTILE ( 4 ) over ( partitionby a * b orderby c + 1 , d ) + e ", res);
        }

        [TestMethod]
        public void SubqueryTest()
        {
            var res = Execute("(SELECT 1)");
            Assert.AreEqual("subquery ", res);

            res = Execute("(SELECT TOP 1 udtcol FROM tab1).method1().method2(b, c)");
            Assert.AreEqual("subquery . method1 ( ) . method2 ( b , c ) ", res);
        }

        [TestMethod]
        public void SimpleCaseTest()
        {
            var res = Execute("CASE x WHEN y THEN z END");
            Assert.AreEqual("case x y z ", res);

            res = Execute("CASE x + 1 WHEN y + 2 THEN z + 3 END");
            Assert.AreEqual("case x + 1 y + 2 z + 3 ", res);

            res = Execute("a + CASE x WHEN y THEN z END * b");
            Assert.AreEqual("a + case x y z * b ", res);
        }
    }
}
