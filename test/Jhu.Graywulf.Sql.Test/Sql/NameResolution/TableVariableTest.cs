using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class DeclareTableTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleDeclareTableTest()
        {
            var sql =
@"DECLARE @test TABLE
(
    ID bigint PRIMARY KEY,
    Data1 nvarchar(50),
    Data2 nvarchar(50) NULL,
    Data3 nvarchar(50) NOT NULL
)";

            var gt =
@"DECLARE @test TABLE
(
    [ID] [bigint] PRIMARY KEY,
    [Data1] [nvarchar](50),
    [Data2] [nvarchar](50) NULL,
    [Data3] [nvarchar](50) NOT NULL
)";

            var ss = ParseAndResolveNames<DeclareTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var v = ss.VariableReference.Variable;
            var t = ss.VariableReference.DataTypeReference.DataType;
            Assert.AreEqual("@test", v.Name);
            Assert.AreEqual(4, t.Columns.Count);

            Assert.IsFalse(t.Columns["ID"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data1"].DataType.IsNullable);
            Assert.IsTrue(t.Columns["Data2"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data3"].DataType.IsNullable);
        }

        [TestMethod]
        public void DeclareTableWithUDTTest()
        {
            var sql =
@"DECLARE @test TABLE
(
    ID bigint PRIMARY KEY,
    Data1 dbo.SimpleUDT,
    Data2 SimpleUDT NOT NULL,
    Data3 dbo.ClrUDT NULL,
    Data4 ClrUDT NOT NULL
)";

            var gt =
@"DECLARE @test TABLE
(
    [ID] [bigint] PRIMARY KEY,
    [Data1] [dbo].[SimpleUDT],
    [Data2] [dbo].[SimpleUDT] NOT NULL,
    [Data3] [dbo].[ClrUDT] NULL,
    [Data4] [dbo].[ClrUDT] NOT NULL
)";

            var ss = ParseAndResolveNames<DeclareTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var v = ss.VariableReference.Variable;
            var t = ss.VariableReference.DataTypeReference.DataType;
            Assert.AreEqual("@test", v.Name);
            Assert.AreEqual(5, t.Columns.Count);

            Assert.IsFalse(t.Columns["ID"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data1"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data2"].DataType.IsNullable);
            Assert.IsTrue(t.Columns["Data3"].DataType.IsNullable);
            Assert.IsFalse(t.Columns["Data4"].DataType.IsNullable);
        }

        [TestMethod]
        public void DeclareAndResolveTableTest()
        {
            var sql =
@"DECLARE @test TABLE
(
    ID bigint IDENTITY,
    Data float
)

SELECT * FROM @test";

            var gt =
@"DECLARE @test TABLE
(
    [ID] [bigint] IDENTITY,
    [Data] [float]
)

SELECT * FROM @test";

            var ss = ParseAndResolveNames<StatementBlock>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DeclareAndResolveTableWithAliasTest()
        {
            var sql =
@"DECLARE @test TABLE
(
    ID bigint IDENTITY,
    Data float
)

SELECT * FROM @test a";

            var gt =
@"DECLARE @test TABLE
(
    [ID] [bigint] IDENTITY,
    [Data] [float]
)

SELECT * FROM @test [a]";

            var ss = ParseAndResolveNames<StatementBlock>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DeclareAndResolveTableInSubqueryTest()
        {
            var sql =
@"DECLARE @test TABLE
(
    ID bigint IDENTITY,
    Data float
)

SELECT * FROM (SELECT * FROM @test) AS q";

            var gt =
@"DECLARE @test TABLE
(
    [ID] [bigint] IDENTITY,
    [Data] [float]
)

SELECT * FROM (SELECT * FROM @test) AS [q]";

            var ss = ParseAndResolveNames<StatementBlock>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }
    }
}
