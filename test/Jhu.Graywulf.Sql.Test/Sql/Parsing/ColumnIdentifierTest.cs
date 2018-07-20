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
            Assert.IsFalse(exp.ColumnReference.IsMultiPartIdentifier);
        }

        [TestMethod]
        public void ColumnNameWithTableNameTest()
        {
            var sql = "table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("table1.column1", exp.Value);
            Assert.IsFalse(exp.ColumnReference.IsMultiPartIdentifier);
        }

        [TestMethod]
        public void ColumnNameWithSchemaAndTableNameTest()
        {
            var sql = "schema1.table1.column1";
            var exp = Parse(sql);
            Assert.AreEqual("schema1.table1.column1", exp.Value);
            Assert.IsFalse(exp.ColumnReference.IsMultiPartIdentifier);
        }
        
        [TestMethod]
        public void ColumnNameWithWhitespacesTest()
        {
            var sql = "schema1 . table1 . column1";
            var exp = Parse(sql);
            Assert.AreEqual("schema1 . table1 . column1", exp.Value);
            Assert.IsFalse(exp.ColumnReference.IsMultiPartIdentifier);
        }
    }
}
