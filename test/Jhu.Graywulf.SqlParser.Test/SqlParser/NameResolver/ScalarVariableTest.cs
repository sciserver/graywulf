using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.SqlParser.NameResolver
{
    [TestClass]
    public class ScalarVariableTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void DeclareSingleVariableTest()
        {
            var sql = "DECLARE @var int";
            var ds = Parse<DeclareVariableStatement>(sql);
            var res = GenerateCode(ds);
        }
    }
}
