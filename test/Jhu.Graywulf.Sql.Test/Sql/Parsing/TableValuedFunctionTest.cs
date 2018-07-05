using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class TableValuedFunctionTest : ParsingTestBase
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

        [TestMethod]
        public void CreateTvfCallTest()
        {
            var cg = CreateCodeGenerator();
            var fr = new FunctionReference()
            {
                SchemaName = "dbo",
                FunctionName = "test",
                IsUserDefined = true
            };

            var fun = TableValuedFunctionCall.Create(fr, Expression.CreateNumber("1.1"));
            Assert.AreEqual("[dbo].[test](1.1)", cg.Execute(fun));

            fun = TableValuedFunctionCall.Create(fr, new[] { Expression.CreateNumber("1.1"), Expression.CreateString("'test'") });
            Assert.AreEqual("[dbo].[test](1.1, 'test')", cg.Execute(fun));

            fun = TableValuedFunctionCall.Create(fr, null);
            Assert.AreEqual("[dbo].[test]()", cg.Execute(fun));
        }
    }
}
