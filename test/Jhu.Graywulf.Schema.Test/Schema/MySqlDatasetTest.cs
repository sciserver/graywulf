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
        }

        [TestMethod]
        public void ScalarFunctionTest()
        {
            MySqlDataset target = CreateTarget();

            target.ScalarFunctions.LoadAll();
            Assert.IsTrue(target.ScalarFunctions.Count == 0);
        }

        [TestMethod]
        public void StoredProcedureTest()
        {
            MySqlDataset target = CreateTarget();

            target.StoredProcedures.LoadAll();
            Assert.IsTrue(target.StoredProcedures.Count == 1);
        }

        [TestMethod]
        public void StoredProcedureParametersTest()
        {
            MySqlDataset target = CreateTarget();

            StoredProcedure sp = target.StoredProcedures["GraywulfSchemaTest", "", "spTest"];

            Assert.IsTrue(sp.Parameters.Count == 1);
            Assert.IsTrue(sp.Parameters["hello"].DataType.Name == "varchar");
        }


        [TestMethod]
        public void MetaObjectsTest()
        {
            MySqlDataset target = CreateTarget();

            Table t1 = target.Tables["GraywulfSchemaTest", "", "Book"];
            Assert.IsTrue(t1.Metadata.Summary == "this is my own table comment");

            Table t2 = target.Tables["GraywulfSchemaTest", "", "Author"];
            Assert.IsTrue(t2.Metadata.Summary == "");
        }

        [TestMethod]
        public void MetaColumnsTest()
        {
            MySqlDataset target = CreateTarget();

            Table t1 = target.Tables["GraywulfSchemaTest", "", "Book"];
            Column c1 = t1.Columns["ID"];
            Assert.IsTrue(c1.Metadata.Summary == "id of user");

            Table t2 = target.Tables["GraywulfSchemaTest", "", "Author"];
            Column c2 = t2.Columns["ID"];
            Assert.IsTrue(c2.Metadata.Summary == "");
        }

        [TestMethod]
        public void MetaParametersTest()
        {
            MySqlDataset target = CreateTarget();

            Parameter p = target.StoredProcedures["GraywulfSchemaTest", "", "spTest"].Parameters["hello"];
            //Parameter p = sp.Parameters["hello"];
            Assert.IsTrue(p.Metadata.Summary == "spTestComment");
        }

        [TestMethod]
        public void TableColumnsDataTypeTest()
        {
            MySqlDataset target = CreateTarget();

            //Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "", "SampleData"];

            Assert.IsTrue(t1.Columns.Count == 39);
            Assert.IsTrue(t1.Columns["column_bool"].DataType.Name == "tinyint");
            Assert.IsTrue(t1.Columns["column_tinyint"].DataType.Name == "tinyint");
            Assert.IsTrue(t1.Columns["column_smallint"].DataType.Name == "smallint");
            Assert.IsTrue(t1.Columns["column_mediumint"].DataType.Name == "bigint");
            Assert.IsTrue(t1.Columns["column_integer"].DataType.Name == "int");
            Assert.IsTrue(t1.Columns["column_bigint"].DataType.Name == "bigint");
            Assert.IsTrue(t1.Columns["column_int"].DataType.Name == "int");
            Assert.IsTrue(t1.Columns["column_float"].DataType.Name == "float");
            Assert.IsTrue(t1.Columns["column_double"].DataType.Name == "real");
            Assert.IsTrue(t1.Columns["column_decimal"].DataType.Name == "decimal");
            Assert.IsTrue(t1.Columns["column_date"].DataType.Name == "date");
            Assert.IsTrue(t1.Columns["column_datetime"].DataType.Name == "datetime");
            Assert.IsTrue(t1.Columns["column_year"].DataType.Name == "tinyint");
            Assert.IsTrue(t1.Columns["column_time"].DataType.Name == "time");
            Assert.IsTrue(t1.Columns["column_timestamp"].DataType.Name == "timestamp");
            Assert.IsTrue(t1.Columns["column_tinytext"].DataType.Name == "text");
            Assert.IsTrue(t1.Columns["column_text"].DataType.Name == "text");
            Assert.IsTrue(t1.Columns["column_mediumtext"].DataType.Name == "text");
            Assert.IsTrue(t1.Columns["column_longtext"].DataType.Name == "text");
            Assert.IsTrue(t1.Columns["column_tinyblob"].DataType.Name == "nvarchar");
            Assert.IsTrue(t1.Columns["column_blob"].DataType.Name == "nvarchar");
            Assert.IsTrue(t1.Columns["column_mediumblob"].DataType.Name == "nvarchar");
            Assert.IsTrue(t1.Columns["column_longblob"].DataType.Name == "nvarchar");
            Assert.IsTrue(t1.Columns["column_bit"].DataType.Name == "bit");
            Assert.IsTrue(t1.Columns["column_set"].DataType.Name == "nvarchar");
            Assert.IsTrue(t1.Columns["column_enum"].DataType.Name == "nvarchar");
            Assert.IsTrue(t1.Columns["column_binary"].DataType.Name == "binary");
            Assert.IsTrue(t1.Columns["column_varbinary"].DataType.Name == "varbinary");
            Assert.IsTrue(t1.Columns["column_geometry"].DataType.Name == "nvarchar");
            Assert.IsTrue(t1.Columns["column_char"].DataType.Name == "char");
            Assert.IsTrue(t1.Columns["column_nchar"].DataType.Name == "char");
            Assert.IsTrue(t1.Columns["column_varchar"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_nvarchar"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_real"].DataType.Name == "real");

            //Test cache
            Assert.AreEqual(t1.Columns, target.Tables["GraywulfSchemaTest", "", "SampleData"].Columns);
            Assert.AreEqual(t1.Columns["column_tinyint"], target.Tables["GraywulfSchemaTest", "", "SampleData"].Columns["column_tinyint"]);
        }
    }
}
