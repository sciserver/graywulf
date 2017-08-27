using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class DnfConverterTest
    {
        private BooleanExpression GetSearchCondition(string sql)
        {
            SqlParser p = new SqlParser();
            var select = (SelectStatement)p.Execute(new SelectStatement(), sql);

            var where = select.FindDescendantRecursive<WhereClause>();
            return where.FindDescendant<BooleanExpression>();
        }

        private string VisitTestHelper(string sql)
        {
            var dnf = new LogicalExpressions.DnfConverter();
            return dnf.Visit(GetSearchCondition(sql).GetExpressionTree()).ToString();
        }

        [TestMethod]
        public void SimpleOrTest()
        {
            // --- test OR
            var sql = "SELECT * WHERE a = 1";
            Assert.AreEqual("a = 1", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2";
            Assert.AreEqual("a = 1 OR b = 2", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2 OR c = 3";
            Assert.AreEqual("a = 1 OR b = 2 OR c = 3", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2 OR c = 3 OR d = 4";
            Assert.AreEqual("a = 1 OR b = 2 OR c = 3 OR d = 4", VisitTestHelper(sql));

        }

        [TestMethod]
        public void SimpleAndTest()
        {
            var sql = "SELECT * WHERE a = 1 AND b = 2";
            Assert.AreEqual("a = 1 AND b = 2", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 AND c = 3";
            Assert.AreEqual("a = 1 AND b = 2 AND c = 3", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 AND c = 3 AND d = 4";
            Assert.AreEqual("a = 1 AND b = 2 AND c = 3 AND d = 4", VisitTestHelper(sql));

        }

        [TestMethod]
        public void DistributeAndFromLeftTest()
        {
            var sql = "SELECT * WHERE a = 1 AND b = 2 OR c = 3";
            Assert.AreEqual("a = 1 AND b = 2 OR c = 3", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 AND c = 3 OR d = 4";
            Assert.AreEqual("a = 1 AND b = 2 AND c = 3 OR d = 4", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND (b = 2 OR c = 3)";
            Assert.AreEqual("a = 1 AND b = 2 OR a = 1 AND c = 3", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 AND (c = 3 OR d = 4)";
            Assert.AreEqual("a = 1 AND b = 2 AND c = 3 OR a = 1 AND b = 2 AND d = 4", VisitTestHelper(sql));
        }

        [TestMethod]
        public void DistributeAndFromRightTest()
        {
            var sql = "SELECT * WHERE a = 1 OR b = 2 AND c = 3";
            Assert.AreEqual("a = 1 OR b = 2 AND c = 3", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2 AND c = 3 AND d = 4";
            Assert.AreEqual("a = 1 OR b = 2 AND c = 3 AND d = 4", VisitTestHelper(sql));

            sql = "SELECT * WHERE (a = 1 OR b = 2) AND c = 3";
            Assert.AreEqual("a = 1 AND c = 3 OR b = 2 AND c = 3", VisitTestHelper(sql));

            sql = "SELECT * WHERE (a = 1 OR b = 2) AND c = 3 AND d = 4";
            Assert.AreEqual("a = 1 AND c = 3 AND d = 4 OR b = 2 AND c = 3 AND d = 4", VisitTestHelper(sql));
        }

        [TestMethod]
        public void DistributeOrFromBothTest()
        {
            var sql = "SELECT * WHERE a = 1 AND (b = 2 OR c = 3) AND d = 4";
            Assert.AreEqual("a = 1 AND b = 2 AND d = 4 OR a = 1 AND c = 3 AND d = 4", VisitTestHelper(sql));
        }

        [TestMethod]
        public void BracketsTest()
        {
            var sql = "SELECT * WHERE (a = 1 OR b = 2) AND (c = 3 OR d = 4 AND e = 5 AND (f = 6 OR g = 7))";
            Assert.AreEqual("a = 1 AND c = 3 OR a = 1 AND d = 4 AND e = 5 AND f = 6 OR a = 1 AND d = 4 AND e = 5 AND g = 7 OR b = 2 AND c = 3 OR b = 2 AND d = 4 AND e = 5 AND f = 6 OR b = 2 AND d = 4 AND e = 5 AND g = 7", VisitTestHelper(sql));
        }

        [TestMethod]
        public void NegationsTest()
        {
            var sql = "SELECT * WHERE NOT a = 1";
            Assert.AreEqual("NOT a = 1", VisitTestHelper(sql));

            sql = "SELECT * WHERE NOT (a = 1)";
            Assert.AreEqual("NOT a = 1", VisitTestHelper(sql));

            sql = "SELECT * WHERE NOT (a = 1 AND NOT b = 2)";
            Assert.AreEqual("NOT a = 1 OR b = 2", VisitTestHelper(sql));

            sql = "SELECT * WHERE NOT a = 1 AND NOT b = 2";
            Assert.AreEqual("NOT a = 1 AND NOT b = 2", VisitTestHelper(sql));

            sql = "SELECT * WHERE NOT (a = 1 OR NOT b = 2)";
            Assert.AreEqual("NOT a = 1 AND b = 2", VisitTestHelper(sql));

            sql = "SELECT * WHERE (a = 1 OR b = 2) AND (c = 3 OR NOT d = 4 AND e = 5 AND NOT (f = 6 AND g = 7))";
            Assert.AreEqual("a = 1 AND c = 3 OR a = 1 AND NOT d = 4 AND e = 5 AND NOT f = 6 OR a = 1 AND NOT d = 4 AND e = 5 AND NOT g = 7 OR b = 2 AND c = 3 OR b = 2 AND NOT d = 4 AND e = 5 AND NOT f = 6 OR b = 2 AND NOT d = 4 AND e = 5 AND NOT g = 7", VisitTestHelper(sql));
        }
    }
}
