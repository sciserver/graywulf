using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class ColumnReferenceTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void TryMatchByColumnNameTest()
        {
            var tr1 = new TableReference()
            {
                TableName = "table1",
            };
            var tr2 = new TableReference()
            {
                TableName = "table2",
            };

            var cr1 = new ColumnReference()
            {
                ColumnName = "column1"
            };
            var cr2 = new ColumnReference()
            {
                ColumnName = "column2",
                TableReference = tr1
            };
            var cr3 = new ColumnReference(cr1)
            {
                TableReference = tr1
            };

            // Match by column name
            Assert.IsFalse(cr1.TryMatch(tr1, cr2));
            Assert.IsTrue(cr1.TryMatch(tr1, cr3));

            var cr4 = new ColumnReference()
            {
                ColumnName = "column1",
                TableReference = tr1,
            };
            var cr5 = new ColumnReference()
            {
                ColumnName = "column1",
                TableReference = tr2
            };
            
            // Match by column name and table 
            Assert.IsTrue(cr3.TryMatch(tr1, cr4));
            Assert.IsFalse(cr4.TryMatch(tr2, cr5));
        }

        [TestMethod]
        public void TryMatchByColumnAliasTest()
        {
            var tr1 = new TableReference()
            {
                TableName = "table1",
            };
            var tr2 = new TableReference()
            {
                TableName = "table2",
            };

            var cr1 = new ColumnReference()
            {
                ColumnName = "column1"
            };
            var cr2 = new ColumnReference()
            {
                ColumnName = "cc",
                ColumnAlias = "column1",
                TableReference = tr1
            };
            var cr3 = new ColumnReference()
            {
                ColumnName = "column1",
                ColumnAlias = "cc",
                TableReference = tr1
            };

            // Column alias of the argument must override column name
            Assert.IsTrue(cr1.TryMatch(tr1, cr2));
            Assert.IsFalse(cr1.TryMatch(tr1, cr3));
        }
    }
}
