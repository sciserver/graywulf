using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.SqlParser
{
    [TestClass]
    public class LabelAndGotoTest
    {
        private StatementBlock Parse(string query)
        {
            var p = new SqlParser();
            return (StatementBlock)p.Execute(new StatementBlock(), query);
        }

        [TestMethod]
        public void SimpleLabelAndGotoTest()
        {
            var sql =
@"
label1:
GOTO label2
label2:

GOTO label1
";
            var sb = Parse(sql);
        }
        
    }
}
