using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class FunctionTest : SqlNameResolverTestBase
    {        
        [TestMethod]
        public void ScalarUdfTest()
        {
            var sql = "SELECT dbo.ScalarFunction()";
            var q = Parse(sql);

            Assert.AreEqual(1, q.FunctionReferences.Count);
        }

        [TestMethod]
        public void UdtTest()
        {
            var sql = "SELECT * FROM dbo.TestTableValuedFunction() q";
            var q = Parse(sql);

            Assert.AreEqual(1, q.FunctionReferences.Count);
        }
    }
}
