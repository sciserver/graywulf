using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ExpressionTest
    {
        private Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<Expression>(query);
        }

        [TestMethod]
        public void SingleNumberTest()
        {
            var sql = "12";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("12", exp.FindDescendantRecursive<NumericConstant>().Value);
        }

        [TestMethod]
        public void SingleHexLiteralTest()
        {
            var sql = "0X0123456789ABcDeF";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("0X0123456789ABcDeF", exp.FindDescendantRecursive<HexLiteral>().Value);
        }

        [TestMethod]
        public void SingleVariableTest()
        {
            var sql = "@variable";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("@variable", exp.FindDescendantRecursive<Variable>().Value);
        }

        [TestMethod]
        public void SingleSystemVariableTest()
        {
            var sql = "@@sysvar";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("@@sysvar", exp.FindDescendantRecursive<SystemVariable>().Value);
        }

        [TestMethod]
        public void SingleColumnNameTest()
        {
            var sql = "alma";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("alma", exp.FindDescendantRecursive<ColumnIdentifier>().Value);
        }

        [TestMethod]
        public void LongColumnNameTest()
        {
            var sql = "schema1.table1.column1.property1.property2";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("schema1.table1.column1.property1.property2", exp.Value);
            Assert.IsTrue(exp.FindDescendantRecursive<ColumnIdentifier>().ColumnReference.IsMultiPartIdentifier);
        }

        [TestMethod]
        public void BracketsTest()
        {
            var sql = "(alma)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("alma", exp.FindDescendantRecursive<ColumnIdentifier>().Value);
        }

        [TestMethod]
        public void FunctionCallTest()
        {
            var sql = "a + function(b)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<ScalarFunctionCall>().FunctionReference.FunctionName);
        }

        [TestMethod]
        public void UnaryOperatorWithNumberTest()
        {
            var sql = "-1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("1", exp.FindDescendantRecursive<NumericConstant>().Value);
        }

        [TestMethod]
        public void UnaryOperatorWithColumnIdentifierTest()
        {
            var sql = "-table1.column1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("-table1.column1", exp.Value);
            Assert.AreEqual("-", exp.FindDescendantRecursive<UnaryOperator>().FindDescendantRecursive<Minus>().Value);
            Assert.IsTrue(exp.FindDescendantRecursive<ColumnIdentifier>().ColumnReference.IsMultiPartIdentifier);
        }

        [TestMethod]
        public void StringConstantTest()
        {
            var sql = "'string'";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("'string'", exp.Value);
            Assert.AreEqual("'string'", exp.FindDescendantRecursive<StringConstant>().Value);
        }
        
        [TestMethod]
        public void ArithmeticOperatorTest()
        {
            var sql = "a+b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a+b", exp.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnIdentifier>().Value);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
        }

        [TestMethod]
        public void ArithmeticOperatorWhitespacesTest()
        {
            var sql = "a + b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a + b", exp.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnIdentifier>().Value);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
        }

        [TestMethod]
        public void BitwiseOperatorTest()
        {
            var sql = "a|b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a|b", exp.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnIdentifier>().Value);
            Assert.AreEqual("|", exp.FindDescendantRecursive<BitwiseOperator>().Value);
        }

        [TestMethod]
        public void BitwiseOperatorWhitespacesTest()
        {
            var sql = "a | b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a | b", exp.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnIdentifier>().Value);
            Assert.AreEqual("|", exp.FindDescendantRecursive<BitwiseOperator>().Value);
        }

        [TestMethod]
        public void ComplexExpressionTest()
        {
            var sql = "(a + table1.b) * 2 / ((SIN(c) | d) & ~2)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
        }

        [TestMethod]
        public void SingleColumnSubqueryTest()
        {
            var sql = "(SELECT 1)";
            var exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void RankingFunctionTest()
        {
            var sql = "ROW_NUMBER() OVER (PARTITION BY a ORDER BY b, c)";
            var exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void WindowedAggregateFunctionTest()
        {
            var sql = "AVG(x) OVER (PARTITION BY a)";
            var exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void UdtVariablePropertyAccessTest()
        {
            var sql = "@var.prop";
            var exp = ExpressionTestHelper(sql);

            sql = "@var . prop";
            exp = ExpressionTestHelper(sql);

            sql = "@var.prop1.prop2.prop3";
            exp = ExpressionTestHelper(sql);

            sql = "@var1.prop1 + @var2.prop2";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void UdtVariableMethodCallTest()
        {
            var sql = "@var.method(a)";
            var exp = ExpressionTestHelper(sql);

            sql = "@var.method(a).mathod(b, c)";
            exp = ExpressionTestHelper(sql);

            sql = "@var.method(a).mathod(b, c) + @var2.method2()";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void UdtStaticMethodCallTest()
        {
            var sql = "udt::method(a,b)";
            var exp = ExpressionTestHelper(sql);

            sql = "dbo . udt :: method ( 1 , 2 )";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void UdtStaticPropertyAccessTest()
        {
            var sql = "udt::prop";
            var exp = ExpressionTestHelper(sql);

            sql = "dbo.udt::prop";
            exp = ExpressionTestHelper(sql);
        }

        #region Is single column or constant tests

        [TestMethod]
        public void IsSingleColumnTest()
        {
            var sql = "colname";
            var exp = ExpressionTestHelper(sql);
            Assert.IsTrue(exp.IsSingleColumn);

            sql = "(colname)";
            exp = ExpressionTestHelper(sql);
            Assert.IsTrue(exp.IsSingleColumn);

            sql = "( colname )";
            exp = ExpressionTestHelper(sql);
            Assert.IsTrue(exp.IsSingleColumn);

            sql = "( ( colname ) )";
            exp = ExpressionTestHelper(sql);
            Assert.IsTrue(exp.IsSingleColumn);

            sql = "a+b";
            exp = ExpressionTestHelper(sql);
            Assert.IsFalse(exp.IsSingleColumn);

            sql = "12";
            exp = ExpressionTestHelper(sql);
            Assert.IsFalse(exp.IsSingleColumn);

            sql = "@a";
            exp = ExpressionTestHelper(sql);
            Assert.IsFalse(exp.IsSingleColumn);

            sql = "udt::Method()";
            exp = ExpressionTestHelper(sql);
            Assert.IsFalse(exp.IsSingleColumn);
        }

        [TestMethod]
        public void IsConstantNumber()
        {
            var sql = "12";
            var exp = ExpressionTestHelper(sql);
            Assert.IsTrue(exp.IsConstantNumber);

            sql = "(12)";
            exp = ExpressionTestHelper(sql);
            Assert.IsTrue(exp.IsConstantNumber);

            sql = "( 12 )";
            exp = ExpressionTestHelper(sql);
            Assert.IsTrue(exp.IsConstantNumber);

            sql = "( ( 12 ) )";
            exp = ExpressionTestHelper(sql);
            Assert.IsTrue(exp.IsConstantNumber);

            sql = "1+2";
            exp = ExpressionTestHelper(sql);
            Assert.IsFalse(exp.IsConstantNumber);

            sql = "@a";
            exp = ExpressionTestHelper(sql);
            Assert.IsFalse(exp.IsConstantNumber);

            sql = "udt::Method()";
            exp = ExpressionTestHelper(sql);
            Assert.IsFalse(exp.IsConstantNumber);
        }

        #endregion
    }
}
