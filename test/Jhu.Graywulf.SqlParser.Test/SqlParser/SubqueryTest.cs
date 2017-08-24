using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.SqlParser
{
    [TestClass]
    public class SubQueryTest
    {
        [TestMethod]
        public void SimpleSubqueryTest()
        {
            var sql = @"(SELECT * FROM test)";
            new SqlParser().Execute<Subquery>(sql);

            sql = @"(SELECT*FROM[test])";
            new SqlParser().Execute<SelectStatement>(sql);

            sql = @"( SELECT * FROM [test] )";
            new SqlParser().Execute<SelectStatement>(sql);
        }

        [TestMethod]
        public void SubqueryWithOrderByTest()
        {
            var sql = @"(SELECT * FROM test ORDER BY alma)";
            new SqlParser().Execute<Subquery>(sql);
        }

        [TestMethod]
        public void SubqueryWithUnionTest()
        {
            var sql = @"(SELECT * FROM test UNION SELECT * FROM test2 ORDER BY alma)";
            new SqlParser().Execute<Subquery>(sql);
        }
    }
}
