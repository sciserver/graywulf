using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Extensions.Parsing;
using Jhu.Graywulf.Sql.Extensions.NameResolution;

namespace Jhu.Graywulf.Sql.Extensions.QueryValidation
{
    class GraywulfSqlQueryValidatorTest : GraywulfSqlNameResolverTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(Jhu.Graywulf.Parsing.ParserException))]
        public void SelectUnionQueryTest()
        {
            var sql =
@"SELECT a FROM table1 PARTITION BY ID
UNION
SELECT b FROM table2";

            var exp = ExpressionTestHelper(sql);
        }

        [TestMethod]
        [ExpectedException(typeof(Jhu.Graywulf.Parsing.ParserException))]
        public void SelectUnionQueryOrderByTest()
        {
            var sql =
@"SELECT a FROM table1 PARTITION BY ID
UNION
SELECT b FROM table2
ORDER BY 1";

            var exp = ExpressionTestHelper(sql);
        }
    }
}
