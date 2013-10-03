using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.PostgreSql;

namespace Jhu.Graywulf.Schema.Test.Schema
{

    [TestClass]
    public class PostgreSqlDatasetTest
    {
        private PostgreSqlDataset CreateTarget()
        {

            return new PostgreSqlDataset("test", Jhu.Graywulf.Test.Constants.TestConnectionStringPostgreSql);
        }

        [TestMethod]
        public void TableLoadTest()
        {
            PostgreSqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "public", "author"];
            Assert.IsTrue(target.Tables.Count == 1);

            // Get another with missing schema name
            Table t2 = target.Tables["GraywulfSchemaTest", "", "author"];
            Assert.IsTrue(target.Tables.Count == 1);
            Assert.AreEqual(t1, t2);

            // Read from DB with missing schema name
            Table t3 = target.Tables["GraywulfSchemaTest", "", "book"];
            Assert.IsTrue(target.Tables.Count == 2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreEqual(t3.SchemaName, "public");

            // Try to load a non-existent table
            try
            {
                Table t4 = target.Tables["GraywulfSchemaTest", "public", "NonExistentTable"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }

            // Try to load object that's not a table

            // Use wrong database name
        }

        [TestMethod]
        public void TableLoadAllTest()
        {
            PostgreSqlDataset target = CreateTarget();
            target.Tables.LoadAll();
            Assert.IsTrue(target.Tables.Count == 5);
            Assert.IsTrue(target.Tables.IsAllLoaded);
        }
        [TestMethod]
        public void ViewLoadAllTest()
        {
            PostgreSqlDataset target = CreateTarget();
            target.Views.LoadAll();
            Assert.IsTrue(target.Views.Count == 4);
            Assert.IsTrue(target.Views.IsAllLoaded);
        }

        [TestMethod]
        public void TableColumnsTest()
        {
            PostgreSqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "public", "author"];

            Assert.IsTrue(t1.Columns.Count == 2);
            Assert.IsTrue(t1.Columns["id"].DataType.Name == "bigint");

            // Test cache
            Assert.AreEqual(t1.Columns, target.Tables["GraywulfSchemaTest", "public", "author"].Columns);
            Assert.AreEqual(t1.Columns["id"], target.Tables["GraywulfSchemaTest", "public", "author"].Columns["id"]);
        }

        [TestMethod]
        public void TableIndexesTest()
        {
            PostgreSqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "public", "author"];

            Assert.IsTrue(t1.Indexes.Count == 2);
            Assert.IsTrue(t1.Indexes["pk_author"].IsPrimaryKey);

            // Test cache
            Assert.AreEqual(t1.Indexes, target.Tables["GraywulfSchemaTest", "public", "author"].Indexes);
            Assert.AreEqual(t1.Indexes["pk_author"], target.Tables["GraywulfSchemaTest", "public", "author"].Indexes["pk_author"]);
        }

        [TestMethod]
        public void TableIndexeSColumnsTest()
        {
            PostgreSqlDataset target = CreateTarget();

            // Get a single table
            IndexColumn ic = target.Tables["GraywulfSchemaTest", "public", "author"].Indexes["pk_author"].Columns["id"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

            // Test cache
            Assert.AreEqual(ic, target.Tables["GraywulfSchemaTest", "public", "author"].Indexes["pk_author"].Columns["id"]);
        }

        //[TestMethod]
        //public void TableValueFunctionTest()
        //{
        //    PostgreSqlDataset target = CreateTarget();

        //    target.TableValuedFunctions.LoadAll();
        //    Assert.IsTrue(target.TableValuedFunctions.Count == 0);

        //    // TODO: test CLR and SQL functions separately
        //}

        //[TestMethod]
        //public void ScalarFunctionTest()
        //{
        //    PostgreSqlDataset target = CreateTarget();

        //    target.ScalarFunctions.LoadAll();
        //    Assert.IsTrue(target.ScalarFunctions.Count == 0);

        //    // TODO: test CLR and SQL functions separately
        //}

        //[TestMethod]
        //public void StoredProcedureTest()
        //{
        //    PostgreSqlDataset target = CreateTarget();

        //    target.StoredProcedures.LoadAll();
        //    Assert.IsTrue(target.StoredProcedures.Count == 1);

        //    // TODO: test CLR and SQL functions separately
        //}

        //[TestMethod]
        //public void StoredProcedureParametersTest()
        //{
        //    PostgreSqlDataset target = CreateTarget();

        //    StoredProcedure sp = target.StoredProcedures["GraywulfSchemaTest", "public", "spTest"];

        //    Assert.IsTrue(sp.Parameters.Count == 1);
        //    Assert.IsTrue(sp.Parameters["@hello"].DataType.Name == "nvarchar");
        //}
    }
}
