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
    public class FunctionIdentifierTest
    {
        private Jhu.Graywulf.SqlParser.FunctionCall ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.FunctionCall)p.Execute(new Jhu.Graywulf.SqlParser.FunctionCall(), query);
        }

        [TestMethod]
        public void SimpleFunctionNameTest()
        {
            var sql = "function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().ToString());
        }

        [TestMethod]
        public void UdfNameTest()
        {
            var sql = "schema.function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().ToString());
        }

        [TestMethod]
        public void UdfNameWithDatabaseNameTest()
        {
            var sql = "database.schema.function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().ToString());
        }

        [TestMethod]
        public void UdfNameWhitespaceTest()
        {
            var sql = "database . schema . function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().ToString());
        }

        [TestMethod]
        public void FunctionCallOnUdtTest()
        {
            //Assert.Inconclusive();
        }

    }
}
