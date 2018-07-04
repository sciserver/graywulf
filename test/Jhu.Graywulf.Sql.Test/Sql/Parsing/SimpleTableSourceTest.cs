using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class SimpleTableSourceTest
    {
        private SimpleTableSource ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<SimpleTableSource>(query);
        }

        [TestMethod]
        public void SimpleTableSourceWithAliasTest()
        {
            var sql = "table1 t";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("table1 t", exp.Value);
            Assert.AreEqual("t", exp.FindDescendantRecursive<TableAlias>().Value);

            sql = "table1 AS t";
            exp = ExpressionTestHelper(sql);

            sql = "[table1]AS[t]";
            exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        public void SimpleTableSourceWithHintsTest()
        {
            var sql = "table1 t WITH ( TEST , TEST_HINT )";
            var exp = ExpressionTestHelper(sql);

            sql = "[table1][t]WITH(TEST,TEST_HINT)";
            exp = ExpressionTestHelper(sql);

            sql = "table1 t WITH ( TEST )";
            exp = ExpressionTestHelper(sql);

            sql = "[table1][t]WITH(TEST)";
            exp = ExpressionTestHelper(sql);

            sql = "[table1][t]WITH()";
            exp = ExpressionTestHelper(sql);

            sql = "[table1] [t] WITH ( )";
            exp = ExpressionTestHelper(sql);
        }

        // TODO: Add tablesample tests
    }
}
