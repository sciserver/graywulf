using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class NameResolutionErrorTest : SqlNameResolverTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void DuplicateTableAliasTest()
        {
            var sql =
@"SELECT * 
FROM Author a
INNER JOIN Book a ON a.ID = a.ID";
            var exp = ParseAndResolveNames(sql);
        }

        [TestMethod]
        public void DuplicateColumnAliasTest()
        {
            var sql =
@"SELECT *
FROM (SELECT Name AS Name1, Name AS Name1
      FROM Author a) AS q";
            var exp = ParseAndResolveNames(sql);
        }
    }
}
