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
    public class TableSourceExpressionTest
    {
        private TableSourceExpression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            var ss = (SelectStatement)p.Execute(new SelectStatement(), query);
            return ss.FindDescendantRecursive<TableSourceExpression>();
        }

        [TestMethod]
        public void SimpleTableTest()
        {
            var sql = "SELECT column1 FROM table1";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("table1", exp.ToString());
        }

        [TestMethod]
        public void SimpleTableWithAliasTest()
        {
            var sql = "SELECT column1 FROM table1 t";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("table1 t", exp.ToString());
            Assert.AreEqual("t", exp.FindDescendantRecursive<TableAlias>().ToString());
        }

        [TestMethod]
        public void TableValuedFunctionTest()
        {
            var sql = "SELECT column FROM dbo.TableValuedFunction() f";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("dbo.TableValuedFunction() f", exp.ToString());
            Assert.AreEqual("f", exp.FindDescendantRecursive<TableAlias>().ToString());
        }

        [TestMethod]
        public void TableValuedVariableTest()
        {
            var sql = "SELECT column FROM @table";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("@table", exp.ToString());
        }

        [TestMethod]
        public void SubQueryTest()
        {
            var sql = "SELECT column FROM (SELECT column2 FROM table2) s";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("(SELECT column2 FROM table2) s", exp.ToString());
            Assert.AreEqual("s", exp.FindDescendantRecursive<TableAlias>().ToString());
        }

        [TestMethod]
        public void CommaSeparatedCrossJoinTest()
        {
            var sql = "SELECT column1, column2 FROM table1, table2";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("table1, table2", exp.ToString());
        }

        [TestMethod]
        public void CrossJoinTest()
        {
            var sql = "SELECT column1, column2 FROM table1 CROSS JOIN table2";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("table1 CROSS JOIN table2", exp.ToString());
        }

        [TestMethod]
        public void InnerJoinTest()
        {
            var sql = "SELECT column1, column2 FROM table1 t1 INNER JOIN table2 t2 ON t1.ID = t2.ID";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("table1 t1 INNER JOIN table2 t2 ON t1.ID = t2.ID", exp.ToString());
        }

        [TestMethod]
        public void MultipleJoinTest()
        {
            var sql = 
@"SELECT column1, column2
FROM table1 t1
INNER JOIN table2 t2 ON t1.ID = t2.ID
INNER JOIN table3 t3 ON t1.ID = t3.ID";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual(
@"table1 t1
INNER JOIN table2 t2 ON t1.ID = t2.ID
INNER JOIN table3 t3 ON t1.ID = t3.ID", exp.Value);
        }
    }
}
