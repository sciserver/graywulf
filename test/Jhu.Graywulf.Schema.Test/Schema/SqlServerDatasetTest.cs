using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.Schema.Test
{
    [TestClass]
    public class SqlServerDatasetTest
    {
        private SqlServerDataset CreateTarget()
        {
            return new SqlServerDataset("test", Jhu.Graywulf.Test.Constants.TestConnectionString);
        }

        [TestMethod]
        public void TableLoadTest()
        {
            SqlServerDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "dbo", "Author"];
            Assert.IsTrue(target.Tables.Count == 1);

            // Get another with missing schema name
            Table t2 = target.Tables["GraywulfSchemaTest", "", "Author"];
            Assert.IsTrue(target.Tables.Count == 1);
            Assert.AreEqual(t1, t2);

            // Read from DB with missing schema name
            Table t3 = target.Tables["GraywulfSchemaTest", "", "Book"];
            Assert.IsTrue(target.Tables.Count == 2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreEqual(t3.SchemaName, "dbo");

            // Try to load a non-existent table
            try
            {
                Table t4 = target.Tables["GraywulfSchemaTest", "dbo", "NonExistentTable"];
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
            SqlServerDataset target = CreateTarget();
            target.Tables.LoadAll();
            //Assert.IsTrue(target.Tables.Count == 4);
            Assert.IsTrue(target.Tables.IsAllLoaded);
        }

        [TestMethod]
        public void TableColumnsTest()
        {
            SqlServerDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "dbo", "Author"];

            Assert.IsTrue(t1.Columns.Count == 2);
            Assert.IsTrue(t1.Columns["ID"].DataType.Name == "bigint");

            // Test cache
            Assert.AreEqual(t1.Columns, target.Tables["GraywulfSchemaTest", "dbo", "Author"].Columns);
            Assert.AreEqual(t1.Columns["ID"], target.Tables["GraywulfSchemaTest", "dbo", "Author"].Columns["ID"]);
        }

        [TestMethod]
        public void TableIndexesTest()
        {
            SqlServerDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "dbo", "Author"];

            Assert.IsTrue(t1.Indexes.Count == 1);
            Assert.IsTrue(t1.Indexes["PK_Author"].IsPrimaryKey);

            // Test cache
            Assert.AreEqual(t1.Indexes, target.Tables["GraywulfSchemaTest", "dbo", "Author"].Indexes);
            Assert.AreEqual(t1.Indexes["PK_Author"], target.Tables["GraywulfSchemaTest", "dbo", "Author"].Indexes["PK_Author"]);
        }

        [TestMethod]
        public void TableIndexeColumnsTest()
        {
            SqlServerDataset target = CreateTarget();

            // Get a single table
            IndexColumn ic = target.Tables["GraywulfSchemaTest", "dbo", "Author"].Indexes["PK_Author"].Columns["ID"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

            // Test cache
            Assert.AreEqual(ic, target.Tables["GraywulfSchemaTest", "dbo", "Author"].Indexes["PK_Author"].Columns["ID"]);
        }

        [TestMethod]
        public void TableValueFunctionTest()
        {
            SqlServerDataset target = CreateTarget();

            target.TableValuedFunctions.LoadAll();
            Assert.IsTrue(target.TableValuedFunctions.Count == 1);

            // TODO: test CLR and SQL functions separately
        }

        [TestMethod]
        public void ScalarFunctionTest()
        {
            SqlServerDataset target = CreateTarget();

            target.ScalarFunctions.LoadAll();
            Assert.IsTrue(target.ScalarFunctions.Count == 1);

            // TODO: test CLR and SQL functions separately
        }

        [TestMethod]
        public void StoredProcedureTest()
        {
            SqlServerDataset target = CreateTarget();
            
            target.StoredProcedures.LoadAll();
            Assert.IsTrue(target.StoredProcedures.Count == 1);

            // TODO: test CLR and SQL functions separately
        }

        [TestMethod]
        public void StoredProcedureParametersTest()
        {
            SqlServerDataset target = CreateTarget();

            StoredProcedure sp = target.StoredProcedures["GraywulfSchemaTest", "dbo", "spTest"];

            Assert.IsTrue(sp.Parameters.Count == 1);
            Assert.IsTrue(sp.Parameters["@hello"].DataType.Name == "nvarchar");
        }
    }
}
