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
        public void ColumnNameWithTableNameTest()
        {
            var sql = "table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("table1.column1", exp.Value);
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
        public void ColumnNameWithWhitespacesTest()
        {
            var sql = "schema1 . table1 . column1";
            var exp = Parse(sql);
            Assert.AreEqual("schema1 . table1 . column1", exp.Value);
            Assert.AreEqual("schema1", exp.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.TableReference.TableName);
            Assert.AreEqual("column1", exp.ColumnReference.ColumnName);
        }
    }
}
