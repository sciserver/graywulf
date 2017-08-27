using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class QueryHintTest
    {
        [TestMethod]
        public void SelectWithOptionTest()
        {
            var sql =
@"SELECT TOP 10 ID 
FROM test
GROUP BY ID
OPTION (RECOMPILE)";
            new SqlParser().Execute<SelectStatement>(sql);
        }
    }
}
