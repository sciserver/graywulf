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
        public void SimpleCreateTableTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint PRIMARY KEY,
    Data1 nvarchar(50),
    Data2 nvarchar(50) NOT NULL,
    Data3 nvarchar(50) NULL
)";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint] PRIMARY KEY,
    [Data1] [nvarchar](50),
    [Data2] [nvarchar](50) NOT NULL,
    [Data3] [nvarchar](50) NULL
)";

            var ss = ParseAndResolveNames<CreateTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var t = (Schema.Table)ss.DatabaseObjectReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(4, t.Columns.Count);

            Assert.IsFalse(t.Columns["ID"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data1"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data2"].DataType.IsNullable);
            Assert.IsTrue(t.Columns["Data3"].DataType.IsNullable);
        }

        [TestMethod]
        public void CreateTableWithUDTTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint PRIMARY KEY,
    Data1 dbo.SimpleUDT,
    Data2 SimpleUDT NOT NULL,
    Data3 dbo.ClrUDT NULL,
    Data4 ClrUDT NOT NULL
)";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint] PRIMARY KEY,
    [Data1] [dbo].[SimpleUDT],
    [Data2] [dbo].[SimpleUDT] NOT NULL,
    [Data3] [dbo].[ClrUDT] NULL,
    [Data4] [dbo].[ClrUDT] NOT NULL
)";

            var ss = ParseAndResolveNames<CreateTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var t = (Schema.Table)ss.DatabaseObjectReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(5, t.Columns.Count);

            Assert.IsFalse(t.Columns["ID"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data1"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data2"].DataType.IsNullable);
            Assert.IsTrue(t.Columns["Data3"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data4"].DataType.IsNullable);

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

            var ss = ParseAndResolveNames<CreateTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var t = (Schema.Table)ss.DatabaseObjectReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(3, t.Columns.Count);

            Assert.AreEqual("ID", t.Columns["ID"].ColumnName);
            Assert.AreEqual("bigint", t.Columns["ID"].DataType.TypeName);
        }

        [TestMethod]
        public void CreateTableWithConstraintsTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint,
    Data nvarchar(50),
    CONSTRAINT PK_test PRIMARY KEY (ID),
    INDEX IX_text (ID, Data)
)";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint],
    [Data] [nvarchar](50),
    CONSTRAINT PK_test PRIMARY KEY ([ID]),
    INDEX IX_text ([ID], [Data])
)";

            var ss = ParseAndResolveNames<CreateTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var t = (Schema.Table)ss.DatabaseObjectReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(2, t.Columns.Count);
        }

        [TestMethod]
        public void CreateAndResolveTableTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint IDENTITY,
    Data float
)

SELECT * FROM test
";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint] IDENTITY,
    [Data] [float]
)

SELECT * FROM [Graywulf_Schema_Test].[dbo].[test]
";

            var ss = ParseAndResolveNames<StatementBlock>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }
    }
}
