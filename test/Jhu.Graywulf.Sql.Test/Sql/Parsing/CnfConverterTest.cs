using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class CnfConverterTest
    {
        private LogicalExpression GetSearchCondition(string sql)
        {
            SqlParser p = new SqlParser();
            var select = (SelectStatement)p.Execute(new SelectStatement(), sql);

            var where = select.FindDescendantRecursive<WhereClause>();
            return where.FindDescendant<LogicalExpression>();
        }

        private string VisitTestHelper(string sql)
        {
            var cnf = new LogicalExpressions.CnfConverter();
            return cnf.Visit(GetSearchCondition(sql).GetExpressionTree()).ToString();
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
        public void DistributeOrFromLeftTest()
        {
            var sql = "SELECT * WHERE a = 1 OR b = 2 AND c = 3";
            Assert.AreEqual("(a = 1 OR b = 2) AND (a = 1 OR c = 3)", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 OR b = 2 AND c = 3 AND d = 4";
            Assert.AreEqual("(a = 1 OR b = 2) AND (a = 1 OR c = 3) AND (a = 1 OR d = 4)", VisitTestHelper(sql));
        }

        [TestMethod]
        public void DistributeOrFromRightTest()
        {
            var sql = "SELECT * WHERE a = 1 AND b = 2 OR c = 3";
            Assert.AreEqual("(a = 1 OR c = 3) AND (b = 2 OR c = 3)", VisitTestHelper(sql));

            sql = "SELECT * WHERE a = 1 AND b = 2 AND c = 3 OR d = 4";
            Assert.AreEqual("(a = 1 OR d = 4) AND (b = 2 OR d = 4) AND (c = 3 OR d = 4)", VisitTestHelper(sql));
        }

        [TestMethod]
        public void DistributeOrFromBothTest()
        {
            var sql = "SELECT * WHERE a = 1 OR b = 2 AND c = 3 OR d = 4";
            Assert.AreEqual("(a = 1 OR b = 2 OR d = 4) AND (a = 1 OR c = 3 OR d = 4)", VisitTestHelper(sql));
        }

        [TestMethod]
        public void DistributeCompositeTest()
        {
            // --- distribution of comopsites
            var sql = "SELECT * WHERE a = 1 AND b = 2 OR c = 3 AND d = 4";
            Assert.AreEqual("(a = 1 OR c = 3) AND (a = 1 OR d = 4) AND (b = 2 OR c = 3) AND (b = 2 OR d = 4)", VisitTestHelper(sql));
        }

        [TestMethod]
        public void BracketsTest()
        {
            var sql = "SELECT * WHERE (a = 1 OR b = 2) AND (c = 3 OR d = 4 AND e = 5 AND (f = 6 OR g = 7))";
            Assert.AreEqual("(a = 1 OR b = 2) AND (c = 3 OR d = 4) AND (c = 3 OR e = 5) AND (c = 3 OR f = 6 OR g = 7)", VisitTestHelper(sql));
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
            Assert.AreEqual("(a = 1 OR b = 2) AND (c = 3 OR NOT d = 4) AND (c = 3 OR e = 5) AND (c = 3 OR NOT f = 6 OR NOT g = 7)", VisitTestHelper(sql));
        }
    }
}
