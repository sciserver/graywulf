using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class TableTableTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleDeclareTableTest()
        {
            var sql =
@"DECLARE @test TABLE
(
    ID bigint PRIMARY KEY,
    Data nvarchar(50)
)";

            var gt =
@"DECLARE @test TABLE
(
    [ID] [bigint] PRIMARY KEY,
    [Data] [nvarchar](50)
)";

            var ss = Parse<DeclareTableStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var v = ss.VariableReference.Variable;
            var t = ss.VariableReference.DataTypeReference.DataType;
            Assert.AreEqual("@test", v.Name);
            Assert.AreEqual(2, t.Columns.Count);
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

SELECT [@test].[ID] AS [ID], [@test].[Data] AS [Data] FROM @test";

            var ss = Parse<StatementBlock>(sql);

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

SELECT [a].[ID] AS [a_ID], [a].[Data] AS [a_Data] FROM @test [a]";

            var ss = Parse<StatementBlock>(sql);

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

SELECT [q].[ID] AS [q_ID], [q].[Data] AS [q_Data] FROM (SELECT [@test].[ID], [@test].[Data] FROM @test) AS [q]";

            var ss = Parse<StatementBlock>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }
    }
}
