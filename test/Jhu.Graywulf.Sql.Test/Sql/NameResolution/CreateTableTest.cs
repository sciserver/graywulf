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

            /*
            var ct = ss.CommonTableExpression.EnumerateCommonTableSpecifications().ToArray();
            var ts = ct[0].Subquery.QueryExpression.EnumerateSourceTableReferences(false).ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = ss.QueryExpression.EnumerateQuerySpecifications().FirstOrDefault().ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
            */
        }

        
    }
}
