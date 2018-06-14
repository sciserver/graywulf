using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class UpdateStatementTest
    {
        [TestMethod]
        public void SimpleUpdateTest()
        {
            var sql = @"UPDATE test SET ID = 1";
            new SqlParser().Execute<UpdateStatement>(sql);

            sql = @"UPDATE test SET ID = (1 + 2)";
            new SqlParser().Execute<UpdateStatement>(sql);

            sql = @"UPDATE test SET ID = DEFAULT";
            new SqlParser().Execute<UpdateStatement>(sql);

            sql = @"UPDATE test SET ID = NULL";
            new SqlParser().Execute<UpdateStatement>(sql);

            sql = @"UPDATE[test]SET ID=1";
            new SqlParser().Execute<UpdateStatement>(sql);

            sql = @"UPDATE @test SET ID = DEFAULT";
            new SqlParser().Execute<UpdateStatement>(sql);
        }
        
        [TestMethod]
        public void MultipleSetTest()
        {
            var sql =
@"UPDATE test
SET ID = 1,Data=2 , Data2 = 3 + 4";
            new SqlParser().Execute<UpdateStatement>(sql);
        }

        [TestMethod]
        public void VariousSetFormatsTest()
        {
            var sql =
@"UPDATE test
SET ID = 1,
    @var = 1 + 2,
    col += 12,
    col = (SELECT 1),
    @var = col = 8,
    @var = col -= 12";
            new SqlParser().Execute<UpdateStatement>(sql);
        }

        [TestMethod]
        public void FromClauseTest()
        {
            var sql =
@"UPDATE test
SET ID = 12
FROM test2 INNER JOIN test3 ON test2.id = test3.id";
            new SqlParser().Execute<UpdateStatement>(sql);
        }

        [TestMethod]
        public void WhereClauseTest()
        {
            var sql =
@"UPDATE test
SET Data = 12
WHERE ID = 12";
            new SqlParser().Execute<UpdateStatement>(sql);

            sql =
@"UPDATE test
SET Data = 12
FROM test CROSS JOIN test2
WHERE ID = 12";
            new SqlParser().Execute<UpdateStatement>(sql);
        }
        
        [TestMethod]
        public void CteTest()
        {
            var sql =
@"WITH q AS
(
    SELECT * FROM table1
)
UPDATE z
SET Data += 1
FROM table1 z
INNER JOIN q ON z.ID = q.ID";

            new SqlParser().Execute<UpdateStatement>(sql);
        }
    }
}
