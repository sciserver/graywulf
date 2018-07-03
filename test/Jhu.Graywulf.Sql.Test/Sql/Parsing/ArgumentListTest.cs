using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ArgumentListTest
    {
        [TestMethod]
        public void SimpleArgumentListTest()
        {
            var sql = "a+b, c/d";
            var args = new SqlParser().Execute<ArgumentList>(sql).EnumerateArguments().ToArray();

            Assert.AreEqual("+", args[0].FindDescendantRecursive<Plus>().Value);
            Assert.AreEqual("a", args[0].FindDescendantRecursive<ColumnIdentifier>().Value);
            Assert.AreEqual("/", args[1].FindDescendantRecursive<Div>().Value);
            Assert.AreEqual("c", args[1].FindDescendantRecursive<ColumnIdentifier>().Value);
        }

        [TestMethod]
        public void CreateArgumentListTest()
        {
            var exp1 = new SqlParser().Execute<Expression>("a + b");
            var exp2 = new SqlParser().Execute<Expression>("c / d");

            var argl = ArgumentList.Create(exp1);
            Assert.AreEqual("a + b", argl.Value);

            argl = ArgumentList.Create(exp1, exp2);
            Assert.AreEqual("a + b, c / d", argl.Value);
        }

    }
}
