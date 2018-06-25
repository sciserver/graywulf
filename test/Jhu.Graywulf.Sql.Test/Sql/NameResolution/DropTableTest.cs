using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class DropTableTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleDropTableTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID bigint PRIMARY KEY,
    Data nvarchar(50)
)

DROP TABLE test";

            var gt =
@"CREATE TABLE [Graywulf_Schema_Test].[dbo].[test]
(
    [ID] [bigint] PRIMARY KEY,
    [Data] [nvarchar](50)
)

DROP TABLE [Graywulf_Schema_Test].[dbo].[test]";

            var ss = Parse<StatementBlock>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var t = (Schema.Table)ss.FindDescendantRecursive<DropTableStatement>().DatabaseObjectReference.DatabaseObject;
            Assert.AreEqual("test", t.TableName);
            Assert.AreEqual(2, t.Columns.Count);
        }
    }
}
