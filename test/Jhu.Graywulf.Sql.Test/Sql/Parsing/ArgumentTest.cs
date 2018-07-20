using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ArgumentTest
    {
        [TestMethod]
        public void SimpleArgumentTest()
        {
            var sql = "a+b";
            var exp = new SqlParser().Execute<Argument>(sql);
            Assert.AreEqual("a+b", exp.Value);
            Assert.AreEqual("+", exp.FindDescendantRecursive<Plus>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<Operand>().Value);
        }

        [TestMethod]
        public void CreateArgumentTest()
        {
            var sql = "a+b";
            var exp = new SqlParser().Execute<Expression>(sql);
            var arg = Argument.Create(exp);
            Assert.AreEqual("a+b", arg.Value);
        }

    }
}
