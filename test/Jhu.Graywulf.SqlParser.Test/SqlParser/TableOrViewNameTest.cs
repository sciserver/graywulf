using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser.Test
{
    [TestClass]
    public class TableOrViewNameTest
    {
        private Jhu.Graywulf.SqlParser.TableOrViewName TablenameTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.TableOrViewName)p.Execute(new Jhu.Graywulf.SqlParser.TableOrViewName(), query);
        }

        [TestMethod]
        public void SimpleTableNameTest()
        {
            var sql = "table";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void TableNameWithSchemaTest()
        {
            var sql = "schema.table";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void TableNameWithDatabaseNameTest()
        {
            var sql = "database.schema.table";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void TableNameWithDatabaseNameWithoutSchemaNameTest()
        {
            var sql = "database..table";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void TableNameWithDatasetPrefixTest()
        {
            var sql = "dataset:database.schema.table";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }

        [TestMethod]
        public void TableNameWhitespacesTest()
        {
            var sql = "dataset : database . schema . table";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
        }
    }
}
