using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing.NameResolver
{
    [TestClass]
    public class SystemVariableTest : SqlNameResolverTestBase
    {
        private StatementBlock Parse(string sql)
        {
            var p = new SqlParser().Execute<StatementBlock>(sql);
            var nr = new SqlNameResolver();
            nr.Execute(p);
            return p;
        }
        
        [TestMethod]
        public void SetFromQueryTest()
        {
            var sql = "SELECT @@VERSION";
            var ss = Parse(sql);
        }
    }
}
