using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser.Test
{
    [TestClass]
    public class TableValuedFunctionTest
    {
        private Jhu.Graywulf.SqlParser.SelectStatement ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.SelectStatement)p.Execute(new Jhu.Graywulf.SqlParser.SelectStatement(), query);
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
