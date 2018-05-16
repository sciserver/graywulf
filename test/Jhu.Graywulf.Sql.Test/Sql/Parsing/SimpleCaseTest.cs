using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class SimpleCaseTest
    {
        private SimpleCaseExpression Parse(string query)
        {
            var p = new SqlParser();
            return p.Execute<SimpleCaseExpression>(query);
        }

        [TestMethod]
        public void SimpleCaseOneItemTest()
        {
            var sql = "CASE test WHEN 1 THEN 2 END";
            var sb = Parse(sql);

            sql = "CASE(test)WHEN(1)THEN(2)END";
            sb = Parse(sql);
        }

        [TestMethod]
        public void SimpleCaseWithElseTest()
        {
            var sql = "CASE test WHEN 1 THEN 2 ELSE 3 END";
            var sb = Parse(sql);

            sql = "CASE(test)WHEN(1)THEN(2)ELSE(3)END";
            sb = Parse(sql);
        }

        [TestMethod]
        public void SimpleCaseMultipleItemsTest()
        {
            var sql = "CASE test WHEN 1 THEN 2 WHEN 3 THEN 4 END";
            var sb = Parse(sql);

            sql = "CASE(test)WHEN(1)THEN(2)WHEN(3)THEN(4)END";
            sb = Parse(sql);
        }

        [TestMethod]
        public void SimpleCaseMultipleItemsWithElseTest()
        {
            var sql = "CASE test WHEN 1 THEN 2 WHEN 3 THEN 4 ELSE 5 END";
            var sb = Parse(sql);

            sql = "CASE(test)WHEN(1)THEN(2)WHEN(3)THEN(4)ELSE(5)END";
            sb = Parse(sql);
        }
    }
}
