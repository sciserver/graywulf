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
    public class SelectStatementTest
    {
        private Jhu.Graywulf.SqlParser.SelectStatement ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.SelectStatement)p.Execute(new Jhu.Graywulf.SqlParser.SelectStatement(), query);
        }

        [TestMethod]
        public void SelectTest()
        {
            var sql = "SELECT * FROM table";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void SelectOrderByTest()
        {
            var sql = "SELECT * FROM table ORDER BY a";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("a", exp.FindDescendantRecursive<OrderByClause>().FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void SelectUnionQueryTest()
        {
            var sql = @"
SELECT a FROM table1
UNION
SELECT b FROM table2";

            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("UNION", exp.FindDescendantRecursive<QueryOperator>().ToString());
        }

        [TestMethod]
        public void SelectUnionQueryOrderByTest()
        {
            var sql = @"
SELECT a FROM table1
UNION
SELECT b FROM table2
ORDER BY 1";

            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("UNION", exp.FindDescendantRecursive<QueryOperator>().ToString());
        }

        [TestMethod]
        public void SelectDistinctTest()
        {
            var sql = "SELECT DISTINCT a FROM table";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void SelectTopTest()
        {
            var sql = "SELECT TOP 10 a FROM table";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("10", exp.FindDescendantRecursive<TopExpression>().FindDescendantRecursive<Number>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void SelectListTest()
        {
            var sql = "SELECT a, b, c, d FROM table";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual(4, exp.FindDescendantRecursive<SelectList>().EnumerateDescendantsRecursive<ColumnExpression>().Count());
            Assert.AreEqual("a, b, c, d", exp.FindDescendantRecursive<SelectList>().ToString());
        }

        [TestMethod]
        public void SelectIntoTest()
        {
            var sql = "SELECT a, b, c, d INTO table1 FROM table2";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<IntoClause>().FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void SelectWithNoFromTest()
        {
            var sql = "SELECT 1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
        }

        [TestMethod]
        public void SelectWhereTest()
        {
            var sql = "SELECT a WHERE ID = 12";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("ID = 12", exp.FindDescendantRecursive<WhereClause>().FindDescendantRecursive<SearchCondition>().ToString());
        }

        [TestMethod]
        public void SelectGroupByTest()
        {
            var sql = "SELECT AVG(a) FROM table GROUP BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("ID", exp.FindDescendantRecursive<GroupByClause>().FindDescendant<GroupByList>().ToString());
        }

        [TestMethod]
        public void SelectHavingTest()
        {
            var sql = "SELECT AVG(a) FROM table HAVING AVG(ID) > 1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("AVG(ID) > 1", exp.FindDescendantRecursive<HavingClause>().FindDescendant<SearchCondition>().ToString());
        }
    }
}
