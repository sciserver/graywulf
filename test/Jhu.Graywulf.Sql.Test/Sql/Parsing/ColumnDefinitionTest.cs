using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ColumnDefinitionTest
    {
        private ColumnDefinition Parse(string query)
        {
            var p = new SqlParser();
            var cd = p.Execute<ColumnDefinition>(query);
            return cd;
        }

        [TestMethod]
        public void NullTest()
        {
            var cd = Parse("col1 int NULL");
            Assert.AreEqual(SqlDbType.Int, cd.DataTypeWithSize.DataTypeReference.DataType.SqlDbType);

            cd = Parse("col1[int]NULL");
            Assert.AreEqual(SqlDbType.Int, cd.DataTypeWithSize.DataTypeReference.DataType.SqlDbType);
        }

        [TestMethod]
        public void NotNullTest()
        {
            var dt = Parse("col1 int NOT NULL");
            Assert.AreEqual(SqlDbType.Int, dt.DataTypeWithSize.DataTypeReference.DataType.SqlDbType);

            dt = Parse("col1[int]NOT NULL");
            Assert.AreEqual(SqlDbType.Int, dt.DataTypeWithSize.DataTypeReference.DataType.SqlDbType);
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
    }
}
