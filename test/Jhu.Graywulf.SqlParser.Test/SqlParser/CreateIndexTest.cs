using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.SqlParser
{
    [TestClass]
    public class CreateIndexTest
    {

        [TestMethod]
        public void SimpleCreateIndexTest()
        {
            var sql = @"CREATE INDEX ix_test1 ON test1 (ID)";
            new SqlParser().Execute<CreateIndexStatement>(sql);

            sql = @"CREATE INDEX ix_test1 ON test1 ( ID )";
            new SqlParser().Execute<CreateIndexStatement>(sql);
        }

        [TestMethod]
        public void MultipleIndexColumnsTest()
        {
            var sql =
@"CREATE INDEX ix_test ON test
(
    ID,
    Data ASC,
    [Data2]DESC
)";
            new SqlParser().Execute<CreateIndexStatement>(sql);

            sql = @"CREATE INDEX[ix_test]ON[test](ID,Data,Data2)";
            new SqlParser().Execute<CreateIndexStatement>(sql);

            sql = @"CREATE INDEX ix_test ON test( ID , Data , Data2 )";
            new SqlParser().Execute<CreateIndexStatement>(sql);
        }
        
        [TestMethod]
        public void IndexTypesTest()
        {
            var sql = @"CREATE UNIQUE CLUSTERED INDEX ix_test ON test (ID)";
            new SqlParser().Execute<CreateIndexStatement>(sql);

            sql = @"CREATE NONCLUSTERED INDEX ix_test ON test (ID)";
            new SqlParser().Execute<CreateIndexStatement>(sql);
        }

        [TestMethod]
        public void IncludedColumnsTest()
        {
            var sql = 
@"CREATE INDEX ix_test ON test
(
    ID
)
INCLUDE
(
    Data
)";
            new SqlParser().Execute<CreateIndexStatement>(sql);

            sql = @"CREATE INDEX ix_test ON test(ID)INCLUDE(Data)";
            new SqlParser().Execute<CreateIndexStatement>(sql);

            sql = @"CREATE INDEX ix_test ON test(ID) INCLUDE ( Data )";
            new SqlParser().Execute<CreateIndexStatement>(sql);

            sql = @"CREATE INDEX ix_test ON test(ID) INCLUDE ( Data1,Data2 )";
            new SqlParser().Execute<CreateIndexStatement>(sql);

            sql = @"CREATE INDEX ix_test ON test(ID) INCLUDE ( Data1 , Data2 )";
            new SqlParser().Execute<CreateIndexStatement>(sql);
        }

        [TestMethod]
        public void DropIndexTest()
        {
            var sql = "DROP INDEX ix_test ON test";
            new SqlParser().Execute<DropIndexStatement>(sql);

            sql = "DROP INDEX[ix_test]ON[test]";
            new SqlParser().Execute<DropIndexStatement>(sql);
        }
    }
}
