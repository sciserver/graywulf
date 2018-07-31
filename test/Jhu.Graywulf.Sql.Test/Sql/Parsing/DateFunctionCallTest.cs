using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class DateFunctionCallTest
    {
        [TestMethod]
        public void ParseAllAndDistinctTest()
        {
            var sql = "DATEADD(year, a, 2)";
            var exp = new SqlParser().Execute<Expression>(sql);
            var fc = exp.FindDescendantRecursive<FunctionCall>();
            Assert.IsInstanceOfType(fc, typeof(DateFunctionCall));
        }
    }
}
