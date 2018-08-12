using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Extensions.Parsing;

namespace Jhu.Graywulf.Sql.Extensions.Parsing
{
    [TestClass]
    public class PartitionedSelectStatementTest
    {
        private SelectStatement ExpressionTestHelper(string query)
        {
            var p = new GraywulfSqlParser();
            return p.Execute<SelectStatement>(query);
        }

        [TestMethod]
        public void SelectTest()
        {
            var sql = "SELECT * FROM table1 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<Sql.Parsing.TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectOrderByTest()
        {
            var sql = "SELECT * FROM table1 PARTITION BY ID ORDER BY a";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<Sql.Parsing.TableOrViewIdentifier>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<OrderByClause>().FindDescendantRecursive<Operand>().Value);
        }

        [TestMethod]
        public void SelectDistinctTest()
        {
            var sql = "SELECT DISTINCT a FROM table1 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<Sql.Parsing.TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectTopTest()
        {
            var sql = "SELECT TOP 10 a FROM table1 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("10", exp.FindDescendantRecursive<TopExpression>().FindDescendantRecursive<Sql.Parsing.NumericConstant>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<Sql.Parsing.TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectListTest()
        {
            var sql = "SELECT a, b, c, d FROM table1 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual(4, exp.FindDescendantRecursive<SelectList>().EnumerateDescendantsRecursive<ColumnExpression>().Count());
            Assert.AreEqual("a, b, c, d", exp.FindDescendantRecursive<SelectList>().Value);
        }

        [TestMethod]
        public void SelectIntoTest()
        {
            var sql = "SELECT a, b, c, d INTO table1 FROM table2 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<IntoClause>().FindDescendantRecursive<Sql.Parsing.TableOrViewIdentifier>().Value);
        }
        
        [TestMethod]
        public void SelectGroupByTest()
        {
            var sql = "SELECT AVG(a) FROM table1 PARTITION BY ID GROUP BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("ID", exp.FindDescendantRecursive<GroupByClause>().FindDescendant<GroupByList>().Value);
        }

        [TestMethod]
        public void SelectHavingTest()
        {
            var sql = "SELECT AVG(a) FROM table1 PARTITION BY ID HAVING AVG(ID) > 1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("AVG(ID) > 1", exp.FindDescendantRecursive<HavingClause>().FindDescendant<LogicalExpression>().Value);
        }
    }
}
