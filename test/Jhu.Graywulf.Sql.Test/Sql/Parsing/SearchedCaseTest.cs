using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class SearchedCaseTest
    {
        private SearchedCaseExpression Parse(string query)
        {
            var p = new SqlParser();
            return p.Execute<SearchedCaseExpression>(query);
        }

        [TestMethod]
        public void SearchedCaseOneItemTest()
        {
            var sql = "CASE WHEN a = 1 THEN 2 END";
            var sb = Parse(sql);

            sql = "CASE WHEN(a=1)THEN(2)END";
            sb = Parse(sql);
        }

        [TestMethod]
        public void SearchedCaseWithElseTest()
        {
            var sql = "CASE WHEN a = 1 THEN 2 ELSE 3 END";
            var sb = Parse(sql);

            sql = "CASE WHEN(a=1)THEN(2)ELSE(3)END";
            sb = Parse(sql);
        }

        [TestMethod]
        public void SearchedCaseMultipleItemsTest()
        {
            var sql = "CASE WHEN a = 1 THEN 2 WHEN b = 3 THEN 4 END";
            var sb = Parse(sql);

            sql = "CASE WHEN(a=1)THEN(2)WHEN(b=3)THEN(4)END";
            sb = Parse(sql);
        }

        [TestMethod]
        public void SearchedCaseMultipleItemsWithElseTest()
        {
            var sql = "CASE WHEN a = 1 THEN 2 WHEN b = 3 THEN 4 ELSE 5 END";
            var sb = Parse(sql);

            sql = "CASE WHEN(a=1)THEN(2)WHEN(b=3)THEN(4)ELSE(5)END";
            sb = Parse(sql);
        }
    }
}
