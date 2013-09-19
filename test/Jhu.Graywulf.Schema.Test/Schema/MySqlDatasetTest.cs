using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.MySql;

namespace Jhu.Graywulf.Schema.Test.Schema
{
    [TestClass]
    public class MySqlDatasetTest
    {
        private MySqlDataset CreateTarget()
        {
            
            return new MySqlDataset("test", Jhu.Graywulf.Test.Constants.TestConnectionStringMySql);
        }

        [TestMethod]
        public void TableLoadTest()
        {
            MySqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "", "Author"];
            Assert.IsTrue(target.Tables.Count == 1);

            // Get another with missing schema name
            Table t2 = target.Tables["GraywulfSchemaTest", "", "Author"];
            Assert.IsTrue(target.Tables.Count == 1);
            Assert.AreEqual(t1, t2);

            // Read from DB with missing schema name
            Table t3 = target.Tables["GraywulfSchemaTest", "", "Book"];
            Assert.IsTrue(target.Tables.Count == 2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreEqual(t3.SchemaName, "");

            // Try to load a non-existent table
            try
            {
                Table t4 = target.Tables["GraywulfSchemaTest", "", "NonExistentTable"];
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
            MySqlDataset target = CreateTarget();
            target.Tables.LoadAll();
            //Assert.IsTrue(target.Tables.Count == 4);
            Assert.IsTrue(target.Tables.IsAllLoaded);
        }

        [TestMethod]
        public void TableColumnsTest()
        {
            MySqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "", "Author"];

            Assert.IsTrue(t1.Columns.Count == 2);
            Assert.IsTrue(t1.Columns["ID"].DataType.Name == "bigint");

            // Test cache
            Assert.AreEqual(t1.Columns, target.Tables["GraywulfSchemaTest", "", "Author"].Columns);
            Assert.AreEqual(t1.Columns["ID"], target.Tables["GraywulfSchemaTest", "", "Author"].Columns["ID"]);
        }

        //[TestMethod]
        //public void TableRename()
        //{
        //    MySqlDataset target = CreateTarget();




        //}

        [TestMethod]
        public void TableIndexesTest()
        {
            MySqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "", "Author"];

            Assert.IsTrue(t1.Indexes.Count == 1);
            Assert.IsTrue(t1.Indexes["PK_Author"].IsPrimaryKey);

            // Test cache
            Assert.AreEqual(t1.Indexes, target.Tables["GraywulfSchemaTest", "", "Author"].Indexes);
            Assert.AreEqual(t1.Indexes["PK_Author"], target.Tables["GraywulfSchemaTest", "", "Author"].Indexes["PK_Author"]);
        }

        [TestMethod]
        public void TableIndexesColumnsTest()
        {
            MySqlDataset target = CreateTarget();

            // Get a single table
            IndexColumn ic = target.Tables["GraywulfSchemaTest", "", "Author"].Indexes["PK_Author"].Columns["ID"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

            // Test cache
            Assert.AreEqual(ic, target.Tables["GraywulfSchemaTest", "", "Author"].Indexes["PK_Author"].Columns["ID"]);
        }

        [TestMethod]
        public void TableValueFunctionTest()
        {
            MySqlDataset target = CreateTarget();

            target.TableValuedFunctions.LoadAll();
            Assert.IsTrue(target.TableValuedFunctions.Count == 0);

            // TODO: test CLR and SQL functions separately
        }

        [TestMethod]
        public void ScalarFunctionTest()
        {
            MySqlDataset target = CreateTarget();

            target.ScalarFunctions.LoadAll();
            Assert.IsTrue(target.ScalarFunctions.Count == 0);

            // TODO: test CLR and SQL functions separately
        }

        [TestMethod]
        public void StoredProcedureTest()
        {
            MySqlDataset target = CreateTarget();

            target.StoredProcedures.LoadAll();
            Assert.IsTrue(target.StoredProcedures.Count == 1);

            // TODO: test CLR and SQL functions separately
        }

        [TestMethod]
        public void StoredProcedureParametersTest()
        {
            MySqlDataset target = CreateTarget();

            StoredProcedure sp = target.StoredProcedures["GraywulfSchemaTest", "", "spTest"];

            Assert.IsTrue(sp.Parameters.Count == 1);
            Assert.IsTrue(sp.Parameters["hello"].DataType.Name == "varchar");
        }
    }
}
