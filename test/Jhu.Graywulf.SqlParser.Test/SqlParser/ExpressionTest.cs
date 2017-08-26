using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser.Test
{
    [TestClass]
    public class ExpressionTest
    {
        private Jhu.Graywulf.SqlParser.Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.Expression)p.Execute(new Jhu.Graywulf.SqlParser.Expression(), query);
        }

        [TestMethod]
        public void SingleNumberTest()
        {
            var sql = "12";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("12", exp.FindDescendantRecursive<Number>().Value);
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
            Assert.AreEqual("alma", exp.FindDescendantRecursive<ColumnName>().Value);
        }

        [TestMethod]
        public void FullColumnNameTest()
        {
            var sql = "dataset:database1.schema1.table1.column1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("dataset:database1.schema1.table1.column1", exp.Value);
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().Value);
            Assert.AreEqual("database1", exp.FindDescendantRecursive<DatabaseName>().Value);
            Assert.AreEqual("schema1", exp.FindDescendantRecursive<SchemaName>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().Value);
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().Value);
        }

        [TestMethod]
        public void BracketsTest()
        {
            var sql = "(alma)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("alma", exp.FindDescendantRecursive<ColumnName>().Value);
        }

        [TestMethod]
        public void FunctionCallTest()
        {
            var sql = "a + function(b)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
        }

        [TestMethod]
        public void UnaryOperatorWithNumberTest()
        {
            var sql = "-1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("1", exp.FindDescendantRecursive<Number>().Value);
        }

        [TestMethod]
        public void UnaryOperatorWithColumnIdentifierTest()
        {
            var sql = "-table1.column1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("-table1.column1", exp.Value);
            Assert.AreEqual("-", exp.FindDescendantRecursive<UnaryOperator>().FindDescendantRecursive<Minus>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().Value);
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().Value);
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
        public void SimpleCaseTest()
        {
            // *** TODO
            //Assert.Inconclusive();
        }

        [TestMethod]
        public void SearchedCaseTest()
        {
            // *** TODO
            //Assert.Inconclusive();
        }

        [TestMethod]
        public void ArithmeticOperatorTest()
        {
            var sql = "a+b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a+b", exp.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnName>().Value);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
        }

        [TestMethod]
        public void ArithmeticOperatorWhitespacesTest()
        {
            var sql = "a + b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a + b", exp.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnName>().Value);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
        }

        [TestMethod]
        public void BitwiseOperatorTest()
        {
            var sql = "a|b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a|b", exp.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<AnyVariable>().Value);
            Assert.AreEqual("|", exp.FindDescendantRecursive<BitwiseOperator>().Value);
        }

        [TestMethod]
        public void BitwiseOperatorWhitespacesTest()
        {
            var sql = "a | b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a | b", exp.Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnName>().Value);
            Assert.AreEqual("|", exp.FindDescendantRecursive<BitwiseOperator>().Value);
        }

        [TestMethod]
        public void ComplexExpressionTest()
        {
            var sql = "(a + table1.b) * 2 / ((SIN(c) | d) & ~2)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
        }

    }
}
