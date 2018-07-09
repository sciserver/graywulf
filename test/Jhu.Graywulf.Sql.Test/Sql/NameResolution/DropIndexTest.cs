using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class DropIndexTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleDropIndexTest()
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
)

DROP INDEX IX_test ON test";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint],
    [Data] [float]
)

CREATE INDEX IX_test ON [Graywulf_Schema_Test].[dbo].[test]
(
    [ID]
)

DROP INDEX IX_test ON [Graywulf_Schema_Test].[dbo].[test]";

            var ss = ParseAndResolveNames<StatementBlock>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var ci = ss.FindDescendantRecursive<DropIndexStatement>();
            var t = (Schema.Table)ci.TargetTable.TableReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(2, t.Columns.Count);
        }


    }
}
