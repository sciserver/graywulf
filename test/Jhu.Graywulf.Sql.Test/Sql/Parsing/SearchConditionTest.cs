using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class SearchConditionTest
    {
        private LogicalExpression GetSearchCondition(string sql)
        {
            SqlParser p = new SqlParser();
            var select = (SelectStatement)p.Execute(new SelectStatement(), sql);

            var where = select.FindDescendantRecursive<WhereClause>();
            return where.FindDescendant<LogicalExpression>();
        }
        
        // --

        private string GetExpressionTreeTestHelper(string sql)
        {
            var tree = new LogicalExpressions.ExpressionTreeBuilder().Execute(GetSearchCondition(sql));
            return tree.ToString();
        }

        [TestMethod]
        public void GetExpressionTreeTest()
        {
            var sql = "SELECT * WHERE a = 1";
            Assert.AreEqual("a = 1", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2";
            Assert.AreEqual("a = 1 AND b = 2", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2";
            Assert.AreEqual("a = 1 OR b = 2", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2 AND c = 3";
            Assert.AreEqual("a = 1 OR b = 2 AND c = 3", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 OR c = 3";
            Assert.AreEqual("a = 1 AND b = 2 OR c = 3", GetExpressionTreeTestHelper(sql));

            // --- test brackets

            sql = "SELECT * WHERE a = 1 OR (b = 2 OR c = 3)";
            Assert.AreEqual("a = 1 OR b = 2 OR c = 3", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 OR c = 3";
            Assert.AreEqual("a = 1 AND b = 2 OR c = 3", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE (a = 1 AND b = 2) OR c = 3";
            Assert.AreEqual("a = 1 AND b = 2 OR c = 3", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND (b = 2 OR c = 3)";
            Assert.AreEqual("a = 1 AND (b = 2 OR c = 3)", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE (a = 1 AND b = 2) AND (c = 3 AND d = 4)";
            Assert.AreEqual("a = 1 AND b = 2 AND c = 3 AND d = 4", GetExpressionTreeTestHelper(sql));

            // --- test NOT

            sql = "SELECT * WHERE NOT a = 1";
            Assert.AreEqual("NOT a = 1", GetExpressionTreeTestHelper(sql));

            sql = "SELECT * WHERE NOT (a = 1)";
            Assert.AreEqual("NOT a = 1", GetExpressionTreeTestHelper(sql));

            // --- test precedende

            sql = "SELECT * WHERE a = 1 AND (b = 2 OR c = 3)";
            Assert.IsNotInstanceOfType(GetExpressionTreeTestHelper(sql), typeof(LogicalExpressions.OperatorAnd));

            sql = "SELECT * WHERE a = 1 OR b = 2 AND c = 3";
            Assert.IsNotInstanceOfType(GetExpressionTreeTestHelper(sql), typeof(LogicalExpressions.OperatorAnd));

            sql = "SELECT * WHERE a = 1 OR (b = 2 OR c = 3)";
            Assert.IsNotInstanceOfType(GetExpressionTreeTestHelper(sql), typeof(LogicalExpressions.OperatorOr));
        }

        private string GetParsingTreeTestHelper(string sql)
        {
            var tree = new LogicalExpressions.ExpressionTreeBuilder().Execute(GetSearchCondition(sql));
            return tree.GetParsingTree().Value;
        }

        [TestMethod]
        public void GetParsingTreeTest()
        {
            var sql = "SELECT * WHERE a = 1";
            Assert.AreEqual("a = 1", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2";
            Assert.AreEqual("a = 1 AND b = 2", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2";
            Assert.AreEqual("a = 1 OR b = 2", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2 AND c = 3";
            Assert.AreEqual("a = 1 OR b = 2 AND c = 3", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 OR c = 3";
            Assert.AreEqual("a = 1 AND b = 2 OR c = 3", GetParsingTreeTestHelper(sql));

            // --- test brackets

            sql = "SELECT * WHERE a = 1 OR (b = 2 OR c = 3)";
            Assert.AreEqual("a = 1 OR b = 2 OR c = 3", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 OR c = 3";
            Assert.AreEqual("a = 1 AND b = 2 OR c = 3", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE (a = 1 AND b = 2) OR c = 3";
            Assert.AreEqual("a = 1 AND b = 2 OR c = 3", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND (b = 2 OR c = 3)";
            Assert.AreEqual("a = 1 AND (b = 2 OR c = 3)", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE (a = 1 AND b = 2) AND (c = 3 AND d = 4)";
            Assert.AreEqual("a = 1 AND b = 2 AND c = 3 AND d = 4", GetParsingTreeTestHelper(sql));

            // --- test NOT

            sql = "SELECT * WHERE NOT a = 1";
            Assert.AreEqual("NOT a = 1", GetParsingTreeTestHelper(sql));

            sql = "SELECT * WHERE NOT (a = 1)";
            Assert.AreEqual("NOT a = 1", GetParsingTreeTestHelper(sql));
        }
    }
}
