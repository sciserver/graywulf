using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.ParserLib;
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
            Assert.AreEqual("12", exp.FindDescendantRecursive<Number>().ToString());
        }

        [TestMethod]
        public void SingleHexLiteralTest()
        {
            var sql = "0X0123456789ABcDeF";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("0X0123456789ABcDeF", exp.FindDescendantRecursive<HexLiteral>().ToString());
        }

        [TestMethod]
        public void SingleVariableTest()
        {
            var sql = "@variable";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("@variable", exp.FindDescendantRecursive<Variable>().ToString());
        }

        [TestMethod]
        public void SingleSystemVariableTest()
        {
            var sql = "@@sysvar";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("@@sysvar", exp.FindDescendantRecursive<SystemVariable>().ToString());
        }

        [TestMethod]
        public void SingleColumnNameTest()
        {
            var sql = "alma";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("alma", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void FullColumnNameTest()
        {
            var sql = "dataset:database.schema.table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("dataset:database.schema.table.column", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void BracketsTest()
        {
            var sql = "(alma)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("alma", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void FunctionCallTest()
        {
            var sql = "a + function(b)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().ToString());
        }

        [TestMethod]
        public void UnaryOperatorWithNumberTest()
        {
            var sql = "-1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("1", exp.FindDescendantRecursive<Number>().ToString());
        }

        [TestMethod]
        public void UnaryOperatorWithColumnIdentifierTest()
        {
            var sql = "-table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("-table.column", exp.ToString());
            Assert.AreEqual("-", exp.FindDescendantRecursive<UnaryOperator>().FindDescendantRecursive<Minus>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void StringConstantTest()
        {
            var sql = "'string'";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("'string'", exp.ToString());
            Assert.AreEqual("'string'", exp.FindDescendantRecursive<StringConstant>().ToString());
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
            Assert.AreEqual("a+b", exp.ToString());
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnName>().ToString());
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().ToString());
        }

        [TestMethod]
        public void ArithmeticOperatorWhitespacesTest()
        {
            var sql = "a + b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a + b", exp.ToString());
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnName>().ToString());
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().ToString());
        }

        [TestMethod]
        public void BitwiseOperatorTest()
        {
            var sql = "a|b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a|b", exp.ToString());
            Assert.AreEqual("a", exp.FindDescendantRecursive<AnyVariable>().ToString());
            Assert.AreEqual("|", exp.FindDescendantRecursive<BitwiseOperator>().ToString());
        }

        [TestMethod]
        public void BitwiseOperatorWhitespacesTest()
        {
            var sql = "a | b";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("a | b", exp.ToString());
            Assert.AreEqual("a", exp.FindDescendantRecursive<ColumnName>().ToString());
            Assert.AreEqual("|", exp.FindDescendantRecursive<BitwiseOperator>().ToString());
        }

        [TestMethod]
        public void ComplexExpressionTest()
        {
            var sql = "(a + table.b) * 2 / ((SIN(c) | d) & ~2)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
        }

    }
}
