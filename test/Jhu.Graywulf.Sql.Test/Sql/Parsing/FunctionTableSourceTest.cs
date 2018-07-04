using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class FunctionTableSourceTest
    {
        private FunctionTableSource ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<FunctionTableSource>(query);
        }

        [TestMethod]
        public void SimpleFunctionCallTest()
        {
            var sql = "dbo.TableValuedFunction() f";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("dbo.TableValuedFunction() f", exp.Value);
            Assert.AreEqual("f", exp.FindDescendantRecursive<TableAlias>().Value);

            sql = "TableValuedFunction() f";
            exp = ExpressionTestHelper(sql);

            sql = "dbo.TableValuedFunction()f";
            exp = ExpressionTestHelper(sql);

            sql = "dbo.TableValuedFunction() AS f";
            exp = ExpressionTestHelper(sql);

            sql = "dbo.TableValuedFunction()AS[f]";
            exp = ExpressionTestHelper(sql);
        }
    }
}
