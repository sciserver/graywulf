using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ColumnAliasBracketsTest
    {
        [TestMethod]
        public void MultipleAliasesTest()
        {
            var sql = "(a,b)";
            new SqlParser().Execute<ColumnAliasBrackets>(sql);

            sql = "( a, b )";
            new SqlParser().Execute<ColumnAliasBrackets>(sql);
        }
    }
}
