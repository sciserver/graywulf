using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ColumnIdentifierTest : ParsingTestBase
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
            Assert.IsTrue(exp.TableReference.IsUndefined);
            Assert.AreEqual("column", exp.ColumnReference.ColumnName);
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
            Assert.AreEqual("table1.column1", exp.Value);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
        }

        [TestMethod]
        public void ColumnNameWithTableNameAndDatasetPrefixTest()
        {
            var sql = "dataset:table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("dataset:table1.column1", exp.Value);
            Assert.AreEqual("dataset", exp.TableReference.DatasetName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
        }

        [TestMethod]
        public void ColumnNameWithSchemaAndTableNameTest()
        {
            var sql = "schema1.table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("schema1.table1.column1", exp.Value);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
        }

        [TestMethod]
        public void ColumnNameWithSchemaAndTableNameAndDatasetPrefixTest()
        {
            var sql = "dataset:schema1.table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("dataset:schema1.table1.column1", exp.Value);
            Assert.AreEqual("dataset", exp.TableReference.DatasetName);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
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
            Assert.AreEqual("database1.schema1.table1.column1", exp.Value);
            Assert.AreEqual("database1", exp.TableReference.DatabaseName);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
        }

        [TestMethod]
        public void ColumnNameFourPartNameAndDatasetPrefixTest()
        {
            var sql = "dataset:database1.schema1.table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("dataset:database1.schema1.table1.column1", exp.Value);
            Assert.AreEqual("dataset", exp.TableReference.DatasetName);
            Assert.AreEqual("database1", exp.TableReference.DatabaseName);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
        }

        [TestMethod]
        public void ColumnNameFourPartNameAndDatasetPrefixWhitespacesTest()
        {
            var sql = "dataset : database1 . schema1 . table1 . column1";
            var exp = Parse(sql);
            Assert.AreEqual("dataset : database1 . schema1 . table1 . column1", exp.Value);
            Assert.AreEqual("dataset", exp.TableReference.DatasetName);
            Assert.AreEqual("database1", exp.TableReference.DatabaseName);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
        }

        [TestMethod]
        public void ColumnNameFourPartNameWithMissingSchemaNameTest()
        {
            var sql = "database1..table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("database1..table1.column1", exp.Value);
            Assert.AreEqual("database1", exp.TableReference.DatabaseName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
        }

        [TestMethod]
        public void SingleStarTest()
        {
            var sql = "*";
            var exp = Parse(sql);
            Assert.AreEqual("*", exp.Value);
            Assert.IsTrue(exp.ColumnReference.IsStar);
        }

        [TestMethod]
        public void StarWithTablePrefixTest()
        {
            var sql = "dataset:database1.schema1.table1.*";
            var exp = Parse(sql);
            Assert.AreEqual("dataset:database1.schema1.table1.*", exp.Value);
            Assert.AreEqual("dataset", exp.ColumnReference.ParentTableReference.DatasetName);
            Assert.AreEqual("database1", exp.ColumnReference.ParentTableReference.DatabaseName);
            Assert.AreEqual("schema1", exp.ColumnReference.ParentTableReference.SchemaName);
            Assert.AreEqual("table1", exp.ColumnReference.ParentTableReference.TableName);
            Assert.IsTrue(exp.ColumnReference.IsStar);
        }

        [TestMethod]
        public void CreateSingleStarTest()
        {
            var ci = ColumnIdentifier.CreateStar();
            Assert.AreEqual("*", ci.Value);
            Assert.IsTrue(ci.ColumnReference.IsStar);

            // TODO: this is fishy, this doesn't supposed to have a table reference
            // because it is the exported column
            Assert.IsTrue(ci.ColumnReference.ParentTableReference.IsUndefined);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CreateTableStarTest()
        {
            var cg = CreateCodeGenerator();
            var tr = new NameResolution.TableReference()
            {
                TableName = "test"
            };
            var ci = ColumnIdentifier.CreateStar(tr);
            Assert.AreEqual("[test].*", cg.Execute(ci));
            Assert.IsTrue(ci.ColumnReference.IsStar);

            // TODO: this is fishy, this doesn't supposed to have a table reference
            // because it is the exported column
            Assert.AreEqual("test", ci.ColumnReference.ParentTableReference.TableName);
            Assert.Inconclusive();
        }
    }
}
