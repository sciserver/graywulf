using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser
{
    [TestClass]
    public class ColumnIdentifierTest
    {
        private ColumnIdentifier Parse(string query)
        {
            return new SqlParser().Execute<ColumnIdentifier>(query);
        }

        [TestMethod]
        public void SingleColumnNameTest()
        {
            var sql = "column";
            var exp = Parse(sql);
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void SingleColumnNameWithDatasetPrefixTest()
        {
            try
            {
                var sql = "dataset:column";
                var exp = Parse(sql);
            }
            catch (ParserException ex)
            {
                Assert.AreEqual(8, ex.Pos);
            }
        }

        [TestMethod]
        public void ColumnNameWithTableNameTest()
        {
            var sql = "table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("table1.column1", exp.ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameWithTableNameAndDatasetPrefixTest()
        {
            var sql = "dataset:table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("dataset:table1.column1", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameWithSchemaAndTableNameTest()
        {
            var sql = "schema1.table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("schema1.table1.column1", exp.ToString());
            Assert.AreEqual("schema1", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameWithSchemaAndTableNameAndDatasetPrefixTest()
        {
            var sql = "dataset:schema1.table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("dataset:schema1.table1.column1", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("schema1", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameWithMissingTableNameTest()
        {
            try
            {
                var sql = "schema1..column1";
                var exp = Parse(sql);
            }
            catch (ParserException ex)
            {
                Assert.AreEqual(9, ex.Pos);
            }
        }

        [TestMethod]
        public void ColumnNameFourPartNameTest()
        {
            var sql = "database1.schema1.table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("database1.schema1.table1.column1", exp.ToString());
            Assert.AreEqual("database1", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema1", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameFourPartNameAndDatasetPrefixTest()
        {
            var sql = "dataset:database1.schema1.table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("dataset:database1.schema1.table1.column1", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("database1", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema1", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameFourPartNameAndDatasetPrefixWhitespacesTest()
        {
            var sql = "dataset : database1 . schema1 . table1 . column1";
            var exp = Parse(sql);
            Assert.AreEqual("dataset : database1 . schema1 . table1 . column1", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("database1", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema1", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameFourPartNameWithMissingSchemaNameTest()
        {
            var sql = "database1..table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("database1..table1.column1", exp.ToString());
            Assert.AreEqual("database1", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column1", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

    }
}
