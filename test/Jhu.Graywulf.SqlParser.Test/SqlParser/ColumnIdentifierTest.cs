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
    public class ColumnIdentifierTest
    {
        private Jhu.Graywulf.SqlParser.Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return (Jhu.Graywulf.SqlParser.Expression)p.Execute(new Jhu.Graywulf.SqlParser.Expression(), query);
        }

        [TestMethod]
        public void SingleColumnNameTest()
        {
            var sql = "column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void SingleColumnNameWithDatasetPrefixTest()
        {
            try
            {
                var sql = "dataset:column";
                var exp = ExpressionTestHelper(sql);
            }
            catch (ParserException ex)
            {
                Assert.AreEqual(7, ex.Pos);
            }
        }

        [TestMethod]
        public void ColumnNameWithTableNameTest()
        {
            var sql = "table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("table.column", exp.ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameWithTableNameAndDatasetPrefixTest()
        {
            var sql = "dataset:table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("dataset:table.column", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameWithSchemaAndTableNameTest()
        {
            var sql = "schema.table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("schema.table.column", exp.ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameWithSchemaAndTableNameAndDatasetPrefixTest()
        {
            var sql = "dataset:schema.table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("dataset:schema.table.column", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameWithMissingTableNameTest()
        {
            try
            {
                var sql = "schema..column";
                var exp = ExpressionTestHelper(sql);
            }
            catch (ParserException ex)
            {
                Assert.AreEqual(6, ex.Pos);
            }
        }

        [TestMethod]
        public void ColumnNameFourPartNameTest()
        {
            var sql = "database.schema.table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("database.schema.table.column", exp.ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameFourPartNameAndDatasetPrefixTest()
        {
            var sql = "dataset:database.schema.table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("dataset:database.schema.table.column", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameFourPartNameAndDatasetPrefixWhitespacesTest()
        {
            var sql = "dataset : database . schema . table . column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("dataset : database . schema . table . column", exp.ToString());
            Assert.AreEqual("dataset", exp.FindDescendantRecursive<DatasetName>().ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("schema", exp.FindDescendantRecursive<SchemaName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

        [TestMethod]
        public void ColumnNameFourPartNameWithMissingSchemaNameTest()
        {
            var sql = "database..table.column";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual("database..table.column", exp.ToString());
            Assert.AreEqual("database", exp.FindDescendantRecursive<DatabaseName>().ToString());
            Assert.AreEqual("table", exp.FindDescendantRecursive<TableName>().ToString());
            Assert.AreEqual("column", exp.FindDescendantRecursive<ColumnName>().ToString());
        }

    }
}
