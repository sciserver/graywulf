﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class CreateIndexTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleCreateIndexTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint,
    Data float
)

CREATE INDEX IX_test ON test
(
    ID
)";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint],
    [Data] [float]
)

CREATE INDEX [IX_test] ON [Graywulf_Schema_Test].[dbo].[test]
(
    [ID]
)";

            var ss = ParseAndResolveNames<StatementBlock>(sql);
            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var ci = ss.FindDescendantRecursive<CreateIndexStatement>();
            var t = (Schema.Table)ci.TargetTable.TableReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(2, t.Columns.Count);
        }

        [TestMethod]
        public void CreateIndexWithIncludedColumnsTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint,
    Data float
)

CREATE INDEX IX_test ON test (ID)
INCLUDE (Data)";


            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint],
    [Data] [float]
)

CREATE INDEX [IX_test] ON [Graywulf_Schema_Test].[dbo].[test] ([ID])
INCLUDE ([Data])";

            var ss = ParseAndResolveNames<StatementBlock>(sql);
            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var ci = ss.FindDescendantRecursive<CreateIndexStatement>();
            var t = (Schema.Table)ci.TargetTable.TableReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(2, t.Columns.Count);
        }


    }
}
