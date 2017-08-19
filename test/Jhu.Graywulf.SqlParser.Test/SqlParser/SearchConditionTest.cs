using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlParser.Test
{
    [TestClass]
    public class SearchConditionTest
    {
        private BooleanExpression GetSearchCondition(string sql)
        {
            SqlParser p = new SqlParser();
            var select = (SelectStatement)p.Execute(new SelectStatement(), sql);

            var where = select.FindDescendantRecursive<WhereClause>();
            return where.FindDescendant<BooleanExpression>();
        }

        // --

        private IEnumerable<LogicalExpressions.Expression> EnumerateRawExpressionsTestHelper(string sql)
        {
            var sc = GetSearchCondition(sql);

            MethodInfo m = typeof(BooleanExpression).GetMethod(
                "EnumerateRawExpressions",
                BindingFlags.Instance | BindingFlags.NonPublic);

            return (IEnumerable<LogicalExpressions.Expression>)m.Invoke(sc, null);
        }

        [TestMethod]
        public void EnumerateRawExpressionsTest()
        {
            var sql = "SELECT * WHERE a = 1";
            Assert.AreEqual(1, EnumerateRawExpressionsTestHelper(sql).Count());

            sql = "SELECT * WHERE a = 1 OR b = 2";
            Assert.AreEqual(3, EnumerateRawExpressionsTestHelper(sql).Count());

            sql = "SELECT * WHERE a = 1 OR b = 2 AND b = 3";
            Assert.AreEqual(5, EnumerateRawExpressionsTestHelper(sql).Count());

            // Test brackets
            sql = "SELECT * WHERE a = 1 OR (b = 2 AND b = 3)";
            Assert.AreEqual(3, EnumerateRawExpressionsTestHelper(sql).Count());

            sql = "SELECT * WHERE a = 1 OR (b = 2 AND c = 3) AND d = 4";
            Assert.AreEqual(5, EnumerateRawExpressionsTestHelper(sql).Count());
        }

        // --

        private string GetExpressionTreeTestHelper(string sql)
        {
            var exp = GetSearchCondition(sql).GetExpressionTree();
            return exp.ToString();
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
            var exp = GetSearchCondition(sql).GetExpressionTree();
            return exp.GetParsingTree().ToString();
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
