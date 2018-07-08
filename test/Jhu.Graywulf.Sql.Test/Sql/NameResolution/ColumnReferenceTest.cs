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

        [TestMethod]
        public void TryMatchMultipartColumnNameTest()
        {
            var cr = new ColumnReference()
            {
                ColumnName = "column",
                TableReference = new TableReference()
                {
                    SchemaName = "schema",
                    TableName = "table"
                }
            };

            var sql = "column";
            var exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(0, exp.ColumnReference.ColumnNamePartIndex);
            
            sql = "column.property";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(0, exp.ColumnReference.ColumnNamePartIndex);
            Assert.AreEqual("property", exp.ColumnReference.NameParts[1]);

            sql = "column.property1.property2";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(0, exp.ColumnReference.ColumnNamePartIndex);
            Assert.AreEqual("property1", exp.ColumnReference.NameParts[1]);
            Assert.AreEqual("property2", exp.ColumnReference.NameParts[2]);

            var cr2 = new ColumnReference()
            {
                ColumnName = "test",
                TableReference = new TableReference()
                {
                    SchemaName = "column",
                    TableName = "column"
                }
            };

            sql = "column";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsFalse(exp.ColumnReference.TryMatch(cr2.TableReference, cr2));
            Assert.AreEqual(null, exp.ColumnReference.ColumnName);
            Assert.AreEqual(0, exp.ColumnReference.ColumnNamePartIndex);
        }

        [TestMethod]
        public void TryMatchMultipartTableAliasTest()
        {
            var cr = new ColumnReference()
            {
                ColumnName = "column",
                TableReference = new TableReference()
                {
                    SchemaName = "schema",
                    TableName = "table1",
                    Alias = "t",
                }
            };

            var sql = "t.column";
            var exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(1, exp.ColumnReference.ColumnNamePartIndex);

            sql = "t.column.property";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(1, exp.ColumnReference.ColumnNamePartIndex);
            Assert.AreEqual("property", exp.ColumnReference.NameParts[2]);

            sql = "t.column.property1.property2";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(1, exp.ColumnReference.ColumnNamePartIndex);
            Assert.AreEqual("property1", exp.ColumnReference.NameParts[2]);
            Assert.AreEqual("property2", exp.ColumnReference.NameParts[3]);
        }

        [TestMethod]
        public void TryMatchMultipartTableNameTest()
        {
            var cr = new ColumnReference()
            {
                ColumnName = "column",
                TableReference = new TableReference()
                {
                    SchemaName = "schema",
                    TableName = "table1"
                }
            };

            var sql = "table1.column";
            var exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(1, exp.ColumnReference.ColumnNamePartIndex);

            sql = "table1.column.property";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(1, exp.ColumnReference.ColumnNamePartIndex);
            Assert.AreEqual("property", exp.ColumnReference.NameParts[2]);

            sql = "table1.column.property1.property2";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(1, exp.ColumnReference.ColumnNamePartIndex);
            Assert.AreEqual("property1", exp.ColumnReference.NameParts[2]);
            Assert.AreEqual("property2", exp.ColumnReference.NameParts[3]);
        }

        [TestMethod]
        public void TryMatchMultipartTableSchemaAndNameTest()
        {
            var cr = new ColumnReference()
            {
                ColumnName = "column1",
                TableReference = new TableReference()
                {
                    SchemaName = "schema1",
                    TableName = "table1",
                }
            };

            var sql = "schema1.table1.column1";
            var exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(2, exp.ColumnReference.ColumnNamePartIndex);

            sql = "schema1.table1.column1.property";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(2, exp.ColumnReference.ColumnNamePartIndex);
            Assert.AreEqual("property", exp.ColumnReference.NameParts[3]);

            sql = "schema1.table1.column1.property1.property2";
            exp = new SqlParser().Execute<ColumnIdentifier>(sql);
            Assert.IsTrue(exp.ColumnReference.IsMultiPartIdentifier);
            Assert.IsTrue(exp.ColumnReference.TryMatch(cr.TableReference, cr));
            Assert.AreEqual(2, exp.ColumnReference.ColumnNamePartIndex);
            Assert.AreEqual("property1", exp.ColumnReference.NameParts[3]);
            Assert.AreEqual("property2", exp.ColumnReference.NameParts[4]);
        }
    }
}
