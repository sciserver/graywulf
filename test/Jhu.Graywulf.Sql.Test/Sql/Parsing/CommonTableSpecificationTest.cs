using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class CommonTableSpecificationTest
    {
        [TestMethod]
        public void SubqueryTest()
        {
            var sql = @"(SELECT * FROM test ORDER BY ID)";
            new SqlParser().Execute<Subquery>(sql);
        }

        [TestMethod]
        public void SimpleCommonTableSpecificationTest()
        {
            var sql = "q AS (SELECT * FROM test)";
            new SqlParser().Execute<CommonTableSpecification>(sql);

            sql = "[q]AS(SELECT * FROM test)";
            new SqlParser().Execute<CommonTableSpecification>(sql);
        }

        [TestMethod]
        public void ColumnAliasesTest()
        {
            var sql = "q (a, b) AS (SELECT * FROM test)";
            new SqlParser().Execute<CommonTableSpecification>(sql);

            sql = "q ( a, b ) AS (SELECT * FROM test)";
            new SqlParser().Execute<CommonTableSpecification>(sql);

            sql = "[q](a,b)AS(SELECT * FROM test)";
            new SqlParser().Execute<CommonTableSpecification>(sql);
        }
    }
}
