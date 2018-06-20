using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class CreateTableTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleCreateTableTestTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint PRIMARY KEY,
    Data nvarchar(50)
)";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint] PRIMARY KEY,
    [Data] [nvarchar](50)
)";

            var ss = Parse<CreateTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var t = (Schema.Table)ss.DatabaseObjectReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(2, t.Columns.Count);
        }

        [TestMethod]
        public void CreateTableWithDefaultsTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint DEFAULT 1,
    Data nvarchar(50) DEFAULT GETDATE(),
    Data2 float DEFAULT dbo.ScalarFunction()
)";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint] DEFAULT 1,
    [Data] [nvarchar](50) DEFAULT GETDATE(),
    [Data2] [float] DEFAULT [Graywulf_Schema_Test].[dbo].[ScalarFunction]()
)";

            var ss = Parse<CreateTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var t = (Schema.Table)ss.DatabaseObjectReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(3, t.Columns.Count);

            Assert.AreEqual("ID", t.Columns["ID"].ColumnName);
            Assert.AreEqual("bigint", t.Columns["ID"].DataType.TypeName);
        }
    }
}
