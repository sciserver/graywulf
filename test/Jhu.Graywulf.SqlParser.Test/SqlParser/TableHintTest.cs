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
    public class TableHintTest
    {
        private TableHint[] TableHintTestHelper(string query)
        {
            var p = new SqlParser();
            var ss = (SelectStatement)p.Execute(new SelectStatement(), query);
            var hl = ss.FindDescendantRecursive<TableHintList>();

            return hl.EnumerateDescendantsRecursive<TableHint>().ToArray();
        }

        [TestMethod]
        public void SimpleTableTest()
        {
            var sql = "SELECT column1 FROM table1 WITH(NOLOCK)";

            var hints = TableHintTestHelper(sql);

            Assert.AreEqual("NOLOCK", hints[0].ToString());
        }


        [TestMethod]
        public void SimpleTableWithAliasTest()
        {
            var sql = "SELECT column1 FROM table1 t WITH( NOLOCK)";

            var hints = TableHintTestHelper(sql);

            Assert.AreEqual("NOLOCK", hints[0].ToString());
        }


        [TestMethod]
        public void CommaSeparatedCrossJoinTest()
        {
            var sql = "SELECT column1, column2 FROM table1 WITH(NOLOCK ), table2";

            var hints = TableHintTestHelper(sql);

            Assert.AreEqual("NOLOCK", hints[0].ToString());
        }
    }
}
