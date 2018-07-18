using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class SqlQueryVisitorTest
    {
        class TestVisitorSink : SqlQueryVisitorSink
        {
            private SqlQueryVisitor visitor;
            private StringWriter w;

            public string Execute(Expression node, ExpressionTraversalMode direction)
            {
                visitor = new SqlQueryVisitor(this)
                {
                    Options = new SqlQueryVisitorOptions()
                    {
                        ExpressionTraversal = direction,
                        VisitExpressionSubqueries = false,
                    }
                };
                using (w = new StringWriter())
                {
                    visitor.Execute(node);
                    return w.ToString();
                }
            }

            private void Write(string s)
            {
                w.Write(s);
            }

            private void Write(Jhu.Graywulf.Parsing.Token token)
            {
                w.Write(token.Value + " ");
            }

            public override void VisitUnaryOperator(UnaryOperator node)
            {
                Write(node);
            }

            public override void VisitArithmeticOperator(ArithmeticOperator node)
            {
                Write(node);
            }

            public override void VisitBitwiseOperator(BitwiseOperator node)
            {
                Write(node);
            }

            public override void VisitExpressionBracketOpen(BracketOpen node)
            {
                Write(node);
            }

            public override void VisitExpressionBracketClose(BracketClose node)
            {
                Write(node);
            }

            public override void VisitConstant(Constant node)
            {
                Write(node);
            }

            public override void VisitUserVariable(UserVariable node)
            {
                Write(node);
            }

            public override void VisitSystemVariable(SystemVariable node)
            {
                Write(node);
            }

            public override void VisitCountStar(CountStar node)
            {
                Write(node);
            }

            public override void VisitColumnIdentifier(ColumnIdentifier node)
            {
                Write(node);
            }

            public override void VisitExpressionSubquery(ExpressionSubquery node)
            {
                Write("subquery ");
            }

            public override void VisitUdtStaticPropertyAccess(UdtStaticPropertyAccess node)
            {
                Write(node.DataTypeIdentifier);
                Write("::");
                Write(node.PropertyName);
            }

            public override void VisitUdtStaticMethodCall(UdtStaticMethodCall node)
            {
                Write(node.DataTypeIdentifier);
                Write("::");
                Write(node.MethodName);
            }

            public override void VisitUdtPropertyAccess(UdtPropertyAccess node)
            {
                Write(".");
                Write(node.PropertyName);
            }

            public override void VisitUdtMethodCall(UdtMethodCall node)
            {
                Write(".");
                Write(node.MethodName);
            }

            public override void VisitScalarFunctionCall(ScalarFunctionCall node)
            {
                Write(node.FunctionIdentifier);
            }

            public override void VisitWindowedFunctionCall(WindowedFunctionCall node)
            {
                Write(node.FunctionIdentifier);
            }

            public override void VisitArgumentListStart(ArgumentListStart node)
            {
                Write("( ");
            }

            public override void VisitArgumentListEnd(ArgumentListEnd node)
            {
                Write(") ");
            }

            public override void VisitArgument(Argument node)
            {
                Write(", ");
            }

            public override void VisitOrderByArgument(OrderByArgument node)
            {
                Write(", ");
            }

            public override void VisitOverClause(OverClause node)
            {
                Write("over ");
            }

            public override void VisitPartitionByClause(PartitionByClause node)
            {
                Write("partitionby ");
            }

            public override void VisitOrderByClause(OrderByClause node)
            {
                Write("orderby ");
            }
        }

        #region Expression traversal

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
            Assert.AreEqual("a + ROW_NUMBER over partitionby ( a ) orderby ( , c , d ) ( ) + e ", res);
        }

        #endregion
    }
}
