using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
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
            Assert.AreEqual("table1", exp.Value);
        }

        [TestMethod]
        public void SimpleTableWithAliasTest()
        {
            var sql = "SELECT column1 FROM table1 t";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("table1 t", exp.Value);
            Assert.AreEqual("t", exp.FindDescendantRecursive<TableAlias>().Value);

            sql = "SELECT column1 FROM table1 AS t";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT column1 FROM[table1]AS[t]";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void TableValuedFunctionTest()
        {
            var sql = "SELECT column FROM dbo.TableValuedFunction() f";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("dbo.TableValuedFunction() f", exp.Value);
            Assert.AreEqual("f", exp.FindDescendantRecursive<TableAlias>().Value);

            sql = "SELECT column FROM dbo.TableValuedFunction() AS f";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT column FROM dbo.TableValuedFunction()AS[f]";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void CrossApplyTest()
        {
            var sql = "SELECT * FROM tab CROSS APPLY dbo.TableValuedFunction() f";
            var exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM [tab]CROSS APPLY[dbo].TableValuedFunction() f";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM tab OUTER APPLY dbo.TableValuedFunction() f";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM [tab]OUTER APPLY[dbo].TableValuedFunction() f";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void TableValuedVariableTest()
        {
            var sql = "SELECT column FROM @table";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("@table", exp.Value);

            sql = "SELECT column FROM@table";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void SubQueryTest()
        {
            var sql = "SELECT column FROM (SELECT column2 FROM table2) s";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("(SELECT column2 FROM table2) s", exp.Value);
            Assert.AreEqual("s", exp.FindDescendantRecursive<TableAlias>().Value);
        }

        [TestMethod]
        public void CommaSeparatedCrossJoinTest()
        {
            var sql = "SELECT column1, column2 FROM table1, table2";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("table1, table2", exp.Value);
        }

        [TestMethod]
        public void CrossJoinTest()
        {
            var sql = "SELECT column1, column2 FROM table1 CROSS JOIN table2";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("table1 CROSS JOIN table2", exp.Value);
        }

        [TestMethod]
        public void InnerJoinTest()
        {
            var sql = "SELECT column1, column2 FROM table1 t1 INNER JOIN table2 t2 ON t1.ID = t2.ID";

            var exp = ExpressionTestHelper(sql);

            Assert.AreEqual("table1 t1 INNER JOIN table2 t2 ON t1.ID = t2.ID", exp.Value);
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

        [TestMethod]
        public void VariousJoinsTest()
        {
            var sql = "SELECT * FROM t1 JOIN t2 ON t1.ID = t2.ID";
            var exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM[t1]JOIN[t2]ON[t1].ID=t2.ID";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM t1 INNER JOIN t2 ON t1.ID = t2.ID";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM[t1]INNER JOIN[t2]ON[t1].ID=t2.ID";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM t1 LEFT OUTER JOIN t2 ON t1.ID = t2.ID";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM[t1]LEFT OUTER JOIN[t2]ON[t1].ID=t2.ID";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM t1 LEFT JOIN t2 ON t1.ID = t2.ID";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM[t1]LEFT JOIN[t2]ON[t1].ID=t2.ID";
            exp = ExpressionTestHelper(sql);

            //

            sql = "SELECT * FROM t1 CROSS JOIN t2";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM[t1]CROSS JOIN[t2]";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM t1 , t2";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM[t1],[t2]";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM t1 CROSS APPLY t2() AS t";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM[t1]CROSS APPLY[t2]()AS[t]";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM t1 OUTER APPLY t2() AS t";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM[t1]OUTER APPLY[t2]()AS[t]";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void JoinHintsTest()
        {
            var sql = "SELECT * FROM t1 INNER LOOP JOIN t2 ON t1.ID = t2.ID";
            var exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM t1 INNER HASH JOIN t2 ON t1.ID = t2.ID";
            exp = ExpressionTestHelper(sql);

            sql = "SELECT * FROM t1 INNER MERGE JOIN t2 ON t1.ID = t2.ID";
            exp = ExpressionTestHelper(sql);
        }
    }
}
