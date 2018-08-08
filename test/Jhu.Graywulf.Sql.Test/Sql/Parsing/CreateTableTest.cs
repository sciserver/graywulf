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
        // TODO move most of these to ColumnDefinitionTest

        [TestMethod]
        public void SimpleCreateTableTest()
        {
            var sql = @"CREATE TABLE test1 (ID int)";
            var exp = new SqlParser().Execute<CreateTableStatement>(sql);
            Assert.AreEqual("test1", exp.TableReference.TableName);

            sql = @"CREATE TABLE test1 ( ID int )";
            exp = new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE[test1](ID[int])";
            exp = new SqlParser().Execute<CreateTableStatement>(sql);
        }

        [TestMethod]
        public void TableNameTest()
        {
            var sql = @"CREATE TABLE test1 (ID int)";
            var exp = new SqlParser().Execute<CreateTableStatement>(sql);
            Assert.AreEqual("test1", exp.TableReference.TableName);

            sql = @"CREATE TABLE sch.test1 (ID int)";
            exp = new SqlParser().Execute<CreateTableStatement>(sql);
            Assert.AreEqual("sch", exp.TableReference.SchemaName);
            Assert.AreEqual("test1", exp.TableReference.TableName);

            sql = @"CREATE TABLE db.sch.test1 (ID int)";
            exp = new SqlParser().Execute<CreateTableStatement>(sql);
            Assert.AreEqual("db", exp.TableReference.DatabaseName);
            Assert.AreEqual("sch", exp.TableReference.SchemaName);
            Assert.AreEqual("test1", exp.TableReference.TableName);

            sql = @"CREATE TABLE DS:db.sch.test1 (ID int)";
            exp = new SqlParser().Execute<CreateTableStatement>(sql);
            Assert.AreEqual("DS", exp.TableReference.DatasetName);
            Assert.AreEqual("db", exp.TableReference.DatabaseName);
            Assert.AreEqual("sch", exp.TableReference.SchemaName);
            Assert.AreEqual("test1", exp.TableReference.TableName);
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
        public void TableIndexTest()
        {
            var sql = @"CREATE TABLE test(ID int, INDEX IX_test (ID))";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int, INDEX IX_test CLUSTERED (ID))";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int, INDEX IX_test NONCLUSTERED (ID))";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int, INDEX[IX_test](ID))";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int, INDEX[IX_test]CLUSTERED(ID))";
            new SqlParser().Execute<CreateTableStatement>(sql);

            sql = @"CREATE TABLE test(ID int, INDEX[IX_test]NONCLUSTERED(ID))";
            new SqlParser().Execute<CreateTableStatement>(sql);
        }

        [TestMethod]
        public void StatementAfterCreateTableTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID int DEFAULT 1,
    CONSTRAINT PK_test PRIMARY KEY (ID DESC)
)

PRINT 'hello'";

            new SqlParser().Execute<StatementBlock>(sql);
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
        public void StatementAfterDropTableTest()
        {
            var sql =
@"DROP TABLE test

PRINT 'hello'";

            new SqlParser().Execute<StatementBlock>(sql);
        }

        [TestMethod]
        public void TruncateTableTest()
        {
            var sql = "TRUNCATE TABLE test";
            new SqlParser().Execute<TruncateTableStatement>(sql);

            sql = "TRUNCATE TABLE[test]";
            new SqlParser().Execute<TruncateTableStatement>(sql);
        }

        [TestMethod]
        public void StatementAfterTruncateTableTest()
        {
            var sql =
@"TRUNCATE TABLE test

PRINT 'hello'";

            new SqlParser().Execute<StatementBlock>(sql);
        }
    }
}
