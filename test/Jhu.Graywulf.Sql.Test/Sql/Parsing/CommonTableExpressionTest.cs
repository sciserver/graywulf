using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class CommonTableExpressionTest
    {
        [TestMethod]
        public void SubqueryTest()
        {
            var sql = @"(SELECT * FROM test ORDER BY ID)";
            new SqlParser().Execute<Subquery>(sql);
        }

        [TestMethod]
        public void CommonTableSpecificationTest()
        {
            var sql = "q AS (SELECT * FROM test)";
            new SqlParser().Execute<CommonTableSpecification>(sql);

            sql = "[q]AS(SELECT * FROM test)";
            new SqlParser().Execute<CommonTableSpecification>(sql);
        }

        [TestMethod]
        public void CommonTableSpecificationListTest()
        {
            var sql = "a AS (SELECT 1), b AS (SELECT 2)";
            new SqlParser().Execute<CommonTableSpecificationList>(sql);

            sql = "a AS (SELECT 1), b AS (SELECT 2), c AS (SELECT 3)";
            new SqlParser().Execute<CommonTableSpecificationList>(sql);

            sql = "a AS (SELECT 1),b AS (SELECT 2)";
            new SqlParser().Execute<CommonTableSpecificationList>(sql);

            sql = "a AS (SELECT 1) , b AS (SELECT 2)";
            new SqlParser().Execute<CommonTableSpecificationList>(sql);
        }

        [TestMethod]
        public void SimpleCommonTableExpressionTest()
        {
            var sql = "WITH a AS (SELECT 1), b AS (SELECT 2)";
            new SqlParser().Execute<CommonTableExpression>(sql);
        }

        [TestMethod]
        public void SelectWithCteTest()
        {
            var sql =
@"WITH q AS (SELECT * FROM test)
SELECT * FROM q";
            new SqlParser().Execute<SelectStatement>(sql);

            sql =
@"WITH q AS (SELECT * FROM test),
       b AS (SELECT * FROM test2 ORDER BY ID)
SELECT * FROM q
ORDER BY q.ID";
            new SqlParser().Execute<SelectStatement>(sql);
        }

        [TestMethod]
        public void InsertWithCteTest()
        {
            var sql =
@"WITH q AS (SELECT * FROM test)
INSERT test2
SELECT * FROM q";
            new SqlParser().Execute<InsertStatement>(sql);

            sql =
            @"WITH q AS (SELECT * FROM test),
       b AS (SELECT * FROM test2 ORDER BY ID)
INSERT test3
SELECT * FROM q
ORDER BY q.ID";
            new SqlParser().Execute<InsertStatement>(sql);
        }

        [TestMethod]
        public void UpdateWithCteTest()
        {
            var sql =
@"WITH q AS (SELECT * FROM test)
UPDATE test
SET ID = 12
FROM test2 INNER JOIN test3 ON test2.id = test3.id";
            new SqlParser().Execute<UpdateStatement>(sql);

            sql =
@"WITH q AS (SELECT * FROM test),
       b AS (SELECT * FROM test2 ORDER BY ID)
UPDATE test
SET ID = 12
FROM q INNER JOIN b ON q.id = b.id";
            new SqlParser().Execute<UpdateStatement>(sql);
        }

        [TestMethod]
        public void DeleteWithCteTest()
        {
            var sql =
@"WITH q AS (SELECT * FROM test)
DELETE test";
            new SqlParser().Execute<DeleteStatement>(sql);

            sql =
@"WITH q AS (SELECT * FROM test),
       b AS (SELECT * FROM test2 ORDER BY ID)
DELETE test
FROM q INNER JOIN b ON q.ID = b.ID";
            new SqlParser().Execute<DeleteStatement>(sql);
        }
    }
}
