using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class CommonTableExpressionTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SimpleCteTest()
        {
            var sql = 
@"WITH a AS
(
    SELECT * FROM Author
)
SELECT Name FROM a";

            var gt =
@"WITH [a] AS
(
    SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author]
)
SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [a]";

            var ss = Parse<SelectStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);

            var ct = ss.CommonTableExpression.EnumerateCommonTableSpecifications().ToArray();
            var ts = ct[0].Subquery.QueryExpression.EnumerateSourceTableReferences(false).ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = ss.QueryExpression.EnumerateQuerySpecifications().FirstOrDefault().ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
            Assert.AreEqual(ts[0], cs[0].TableReference);
        }

        [TestMethod]
        public void MultipleCteTest()
        {
            var sql =
@"WITH
    a AS (SELECT * FROM Author),
    b AS (SELECT * FROM Book)
SELECT Name FROM a INNER JOIN b ON a.ID = b.ID";

            var gt =
@"WITH [a] AS
(
    SELECT [Graywulf_Schema_Test].[dbo].[Author].[ID] AS [ID], [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [Graywulf_Schema_Test].[dbo].[Author]
)
SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] AS [Name] FROM [a]";

            var ss = Parse<SelectStatement>(sql);

            var res = GenerateCode(ss);
            Assert.AreEqual(gt, res);
        }
        }
}
