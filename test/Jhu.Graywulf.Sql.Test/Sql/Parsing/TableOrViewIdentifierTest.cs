using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class TableOrViewIdentifierTest
    {
        private TableOrViewIdentifier TablenameTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<TableOrViewIdentifier>(query);
        }

        [TestMethod]
        public void SimpleTableNameTest()
        {
            var sql = "table1";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("table1", exp.TableReference.TableName);
        }

        [TestMethod]
        public void TableNameWithSchemaTest()
        {
            var sql = "schema1.table1";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
        }

        [TestMethod]
        public void TableNameWithDatabaseNameTest()
        {
            var sql = "database1.schema1.table1";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("database1", exp.TableReference.DatabaseName);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
        }

        [TestMethod]
        public void TableNameWithDatabaseNameWithoutSchemaNameTest()
        {
            var sql = "database1..table1";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("database1", exp.TableReference.DatabaseName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
        }

        [TestMethod]
        public void TableNameWithDatasetPrefixTest()
        {
            var sql = "dataset:database1.schema1.table1";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("dataset", exp.TableReference.DatasetName);
            Assert.AreEqual("database1", exp.TableReference.DatabaseName);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
        }

        [TestMethod]
        public void TableNameWhitespacesTest()
        {
            var sql = "dataset : database1 . schema1 . table1";
            var exp = TablenameTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("dataset", exp.TableReference.DatasetName);
            Assert.AreEqual("database1", exp.TableReference.DatabaseName);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolution.NameResolverException))]
        public void TooManyPartsTest()
        {
            var sql = "part1.part2.part3.part4";
            var exp = TablenameTestHelper(sql);
        }
    }
}
