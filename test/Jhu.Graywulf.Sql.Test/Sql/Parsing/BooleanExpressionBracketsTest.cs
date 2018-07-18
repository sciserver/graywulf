using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class BooleanExpressionBracketsTest
    {
        private LgocalExpressionBrackets Parse(string query)
        {
            var p = new SqlParser();
            return p.Execute<LgocalExpressionBrackets>(query);
        }

        [TestMethod]
        public void SimpleBooleanExpressionBracketsTest()
        {
            var sql = "(ID=6 AND Data=10)";
            var sb = Parse(sql);

            sql = "( ( ID=6 )AND( Data=10 ) )";
            sb = Parse(sql);
        }

        [TestMethod]
        public void CreateFromBooleanExpressionTest()
        {
            var exp = new SqlParser().Execute<BooleanExpression>("a = b");

            var bc = LgocalExpressionBrackets.Create(exp);
            Assert.AreEqual("(a = b)", bc.Value);
        }
    }
}
