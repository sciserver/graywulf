using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class TableValuedFunctionTest
    {
        private SelectStatement ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<SelectStatement>(query);
        }

        [TestMethod]
        public void SelectTest()
        {
            var sql = "SELECT * FROM dbo.TVF(0) AS t";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("dbo.TVF(0)", exp.FindDescendantRecursive<TableValuedFunctionCall>().Value);
        }

    }
}
