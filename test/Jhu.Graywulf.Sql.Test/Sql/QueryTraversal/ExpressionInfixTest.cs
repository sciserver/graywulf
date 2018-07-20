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
        [TestMethod]
        public void TraverseExpressionInfixTest()
        {
            var sql = "a + b * -c + d - (e + f) * g";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("a + b * - c + d - ( e + f ) * g ", res);

            sql = "1 + @b * -@@c + COUNT(*) - (e + f) * g";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("1 + @b * - @@c + COUNT(*) - ( e + f ) * g ", res);

            sql = "a ^ b % ~c | d & (e + f) * g";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("a ^ b % ~ c | d & ( e + f ) * g ", res);
        }

        [TestMethod]
        public void TraverseExpressionInfixWithFunctionTest1()
        {
            var sql = "f2()";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("f2 ( ) ", res);

            sql = "f2(a)";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("f2 ( , a ) ", res);

            sql = "f2(a, b, c)";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("f2 ( , a , b , c ) ", res);
        }

        [TestMethod]
        public void TraverseExpressionInfixWithFunctionTest2()
        {
            var sql = "a * f1(b) + f2(a + b * c, d, e)";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("a * f1 ( , b ) + f2 ( , a + b * c , d , e ) ", res);
        }

        [TestMethod]
        public void TraverseExpressionInfixWithStaticUdtPropertyTest()
        {
            var sql = "dbo.udt::prop1";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("dbo.udt ::prop1 ", res);

            sql = "dbo.udt::prop1.prop2";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("dbo.udt ::prop1 .prop2 ", res);

            sql = "a + dbo.udt::prop1.prop2 * b";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("a + dbo.udt ::prop1 .prop2 * b ", res);
        }

        [TestMethod]
        public void TraverseExpressionInfixWithUdtPropertyTest()
        {
            var sql = "@a.prop1";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("@a .prop1 ", res);

            sql = "@a.prop1.prop2";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("@a .prop1 .prop2 ", res);

            sql = "(a + b).prop1.prop2";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("( a + b ) .prop1 .prop2 ", res);

            sql = "(a + b).prop1.prop2.prop3";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("( a + b ) .prop1 .prop2 .prop3 ", res);

            sql = "(SELECT TOP 1 udtcol FROM tab1).prop1.prop2";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("subquery .prop1 .prop2 ", res);
        }

        [TestMethod]
        public void TraverseExpressionInfixWithStaticUdtMethodTest()
        {
            var sql = "dbo.udt::method1()";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("dbo.udt ::method1 ( ) ", res);

            sql = "dbo.udt::method1(a, b)";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("dbo.udt ::method1 ( , a , b ) ", res);

            sql = "a + dbo.udt::method1().method2(a) * b";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("a + dbo.udt ::method1 ( ) .method2 ( , a ) * b ", res);
        }

        [TestMethod]
        public void TraverseExpressionInfixWithUdtMethodTest()
        {
            var sql = "@a.method1()";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("@a .method1 ( ) ", res);

            sql = "@a.method1().method2(b, c)";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("@a .method1 ( ) .method2 ( , b , c ) ", res);

            sql = "(a + b).method1().method2(b, c)";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("( a + b ) .method1 ( ) .method2 ( , b , c ) ", res);

            sql = "(a + b).method1((c * d).method2(e, f))";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("( a + b ) .method1 ( , ( c * d ) .method2 ( , e , f ) ) ", res);

            sql = "(SELECT TOP 1 udtcol FROM tab1).method1().method2(b, c)";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("subquery .method1 ( ) .method2 ( , b , c ) ", res);
        }

        [TestMethod]
        public void TraverseExpressionInfixUdtMembersTest()
        {
            var sql = "@a.prop1.method1(b, c).prop2.prop3.method2()";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("@a .prop1 .method1 ( , b , c ) .prop2 .prop3 .method2 ( ) ", res);
        }

        [TestMethod]
        public void TraverseExpressionInfixRankingFunctionTest()
        {
            var sql = "a + ROW_NUMBER() OVER (PARTITION BY a ORDER BY c ASC, d DESC) + e";
            var exp = new SqlParser().Execute<Expression>(sql);
            var res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("a + ROW_NUMBER ( ) over ( partitionby , a orderby , c , d ) + e ", res);

            sql = "a + NTILE(4) OVER (PARTITION BY a ORDER BY c ASC, d DESC) + e";
            exp = new SqlParser().Execute<Expression>(sql);
            res = new TestVisitorSink().Execute(exp, ExpressionTraversalMode.Infix);
            Assert.AreEqual("a + NTILE ( , 4 ) over ( partitionby , a orderby , c , d ) + e ", res);
        }
    }
}
