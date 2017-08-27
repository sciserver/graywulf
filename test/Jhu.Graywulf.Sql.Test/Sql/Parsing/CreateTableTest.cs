using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class CreateTableTest
    {

        [TestMethod]
        public void SimpleCreateTableTest()
        {
            var sql = @"CREATE TABLE test1 (ID int)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test1 ( ID int )";
            new SqlParser().Execute<CreateTableStatement>(sql);
        }

        [TestMethod]
        public void MultipleTableColumnsTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID int,
    Data nvarchar(max),
    Data2 float
)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int,Data nvarchar(max),Data2 float)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test( ID int , Data nvarchar(max) , Data2 float )";
            new SqlParser().Execute<CreateTableStatement>(sql);
        }

        [TestMethod]
        public void DefaultValueTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID int DEFAULT 1,
    Data nvarchar(max)DEFAULT'string',
    Data2 float DEFAULT (12 + 34)
)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int CONSTRAINT def_test DEFAULT 8)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int CONSTRAINT[def_test]DEFAULT(8))";
            new SqlParser().Execute<CreateTableStatement>(sql);
        }

        [TestMethod]
        public void IdentityTest()
        {
            var sql = @"CREATE TABLE test(ID int IDENTITY)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int IDENTITY(1, 2))";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int IDENTITY ( 1,2 ))";
            new SqlParser().Execute<CreateTableStatement>(sql);
        }

        [TestMethod]
        public void ColumnConstraintTest()
        {
            var sql = @"CREATE TABLE test(ID int IDENTITY PRIMARY KEY)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int IDENTITY PRIMARY KEY CLUSTERED)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int IDENTITY CONSTRAINT pk_test PRIMARY KEY CLUSTERED)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int CONSTRAINT def_test DEFAULT(5) CONSTRAINT pk_test PRIMARY KEY CLUSTERED)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int UNIQUE NONCLUSTERED)";
            new SqlParser().Execute<CreateTableStatement>(sql);
        }

        [TestMethod]
        public void TableConstraintTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID int DEFAULT 1,
    PRIMARY KEY (ID DESC)
)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql =
@"CREATE TABLE test
(
    ID int DEFAULT 1,
    CONSTRAINT PK_test PRIMARY KEY (ID DESC)
)";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql =
@"CREATE TABLE test
(
    ID int DEFAULT 1,
    UNIQUE (ID ASC)
)";
            new SqlParser().Execute<CreateTableStatement>(sql);
        }

        [TestMethod]
        public void DropTableTest()
        {
            var sql = "DROP TABLE test";
            new SqlParser().Execute<DropTableStatement>(sql);

            sql = "DROP TABLE[test]";
            new SqlParser().Execute<DropTableStatement>(sql);
        }

        [TestMethod]
        public void TruncateTableTest()
        {
            var sql = "TRUNCATE TABLE test";
            new SqlParser().Execute<TruncateTableStatement>(sql);

            sql = "TRUNCATE TABLE[test]";
            new SqlParser().Execute<TruncateTableStatement>(sql);
        }
    }
}
