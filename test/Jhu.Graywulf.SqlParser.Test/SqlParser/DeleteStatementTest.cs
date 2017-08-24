using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.SqlParser
{
    [TestClass]
    public class DeleteStatementTest
    {
        [TestMethod]
        public void SimpleDeleteTest()
        {
            var sql = @"DELETE test WHERE ID = 1";
            new SqlParser().Execute<DeleteStatement>(sql);

            sql = @"DELETE FROM test WHERE ID = (1 + 2)";
            new SqlParser().Execute<DeleteStatement>(sql);

            sql = @"DELETE[test]WHERE ID=1";
            new SqlParser().Execute<DeleteStatement>(sql);

            sql = @"DELETE FROM [test] WHERE ID=1";
            new SqlParser().Execute<DeleteStatement>(sql);

            sql = @"DELETE FROM[test]WHERE[ID]=1";
            new SqlParser().Execute<DeleteStatement>(sql);

            sql = @"DELETE @test WHERE ID IS NULL";
            new SqlParser().Execute<DeleteStatement>(sql);
        }
        
        [TestMethod]
        public void FromClauseTest()
        {
            var sql =
@"DELETE test
FROM test INNER JOIN test3 ON test.id = test3.id";
            new SqlParser().Execute<DeleteStatement>(sql);

            sql =
@"DELETE test
FROM test";
            new SqlParser().Execute<DeleteStatement>(sql);

            sql =
@"DELETE FROM test
FROM test INNER JOIN test3 ON test.id = test3.id
WHERE id = 12";
            new SqlParser().Execute<DeleteStatement>(sql);
        }
    }
}
