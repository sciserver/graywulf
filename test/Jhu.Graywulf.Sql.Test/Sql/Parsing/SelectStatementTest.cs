using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class SelectStatementTest
    {
        private SelectStatement ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<SelectStatement>(query);
        }

        [TestMethod]
        public void SelectTest()
        {
            var sql = "SELECT * FROM table1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectOrderByTest()
        {
            var sql = "SELECT * FROM table1 ORDER BY a";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableOrViewIdentifier>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<OrderByClause>().FindDescendantRecursive<ColumnIdentifier>().Value);
        }

        [TestMethod]
        public void SelectUnionQueryTest()
        {
            var sql = 
@"SELECT a FROM table1
UNION
SELECT b FROM table2";

            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("UNION", exp.FindDescendantRecursive<QueryOperator>().Value);
        }

        [TestMethod]
        public void SelectUnionQueryOrderByTest()
        {
            var sql = 
@"SELECT a FROM table1
UNION
SELECT b FROM table2
ORDER BY 1";

            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("UNION", exp.FindDescendantRecursive<QueryOperator>().Value);
        }

        [TestMethod]
        public void SelectDistinctTest()
        {
            var sql = "SELECT DISTINCT a FROM table1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectTopTest()
        {
            var sql = "SELECT TOP 10 a FROM table1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("10", exp.FindDescendantRecursive<TopExpression>().FindDescendantRecursive<NumericConstant>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectListTest()
        {
            var sql = "SELECT a, b, c, d FROM table1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual(4, exp.FindDescendantRecursive<SelectList>().EnumerateDescendantsRecursive<ColumnExpression>().Count());
            Assert.AreEqual("a, b, c, d", exp.FindDescendantRecursive<SelectList>().Value);
        }

        [TestMethod]
        public void SelectIntoTest()
        {
            var sql = "SELECT a, b, c, d INTO table1 FROM table2";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<IntoClause>().FindDescendantRecursive<TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectWithNoFromTest()
        {
            var sql = "SELECT 1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
        }

        [TestMethod]
        public void SelectWhereTest()
        {
            var sql = "SELECT a WHERE ID = 12";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("ID = 12", exp.FindDescendantRecursive<WhereClause>().FindDescendantRecursive<BooleanExpression>().Value);
        }

        [TestMethod]
        public void SelectGroupByTest()
        {
            var sql = "SELECT AVG(a) FROM table1 GROUP BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("ID", exp.FindDescendantRecursive<GroupByClause>().FindDescendant<GroupByList>().Value);
        }

        [TestMethod]
        public void SelectHavingTest()
        {
            var sql = "SELECT AVG(a) FROM table1 HAVING AVG(ID) > 1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("AVG(ID) > 1", exp.FindDescendantRecursive<HavingClause>().FindDescendant<BooleanExpression>().Value);
        }
    }
}
