using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class FunctionIdentifierTest
    {
        private FunctionCall ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<FunctionCall>(query);
        }

        [TestMethod]
        public void SimpleFunctionNameTest()
        {
            var sql = "function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
        }

        [TestMethod]
        public void UdfNameTest()
        {
            var sql = "schema.function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
        }

        [TestMethod]
        public void UdfNameWithDatabaseNameTest()
        {
            var sql = "database.schema.function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().Value);
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
        }

        [TestMethod]
        public void UdfNameWhitespaceTest()
        {
            var sql = "database . schema . function(a)";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().Value);
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().Value);
            Assert.AreEqual("function", exp.FindDescendantRecursive<FunctionName>().Value);
        }

        [TestMethod]
        public void FunctionCallOnUdtTest()
        {
            //Assert.Inconclusive();
        }

    }
}
