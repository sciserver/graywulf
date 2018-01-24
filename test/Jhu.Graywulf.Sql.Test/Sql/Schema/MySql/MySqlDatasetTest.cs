using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.MySql;

namespace Jhu.Graywulf.Schema.MySql.Test
{
    [TestClass]
    public class MySqlDatasetTest
    {
        private MySqlDataset CreateTestDataset()
        {
            var csb = new MySqlConnectionStringBuilder(Jhu.Graywulf.Schema.Test.AppSettings.MySqlConnectionString);

            var ds = new MySqlDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, csb.ConnectionString)
            {
                DatabaseName = csb.Database
            };

            return ds;
        }

        #region Dataset tests

        [TestMethod]
        public void GetAnyObjectTest()
        {
            var ds = CreateTestDataset();

            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "", "TableWithPrimaryKey"), typeof(Table));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "", "ViewWithStar"), typeof(View));
            //Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "", "InlineTableValuedFunction"), typeof(TableValuedFunction));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "", "ScalarFunction"), typeof(ScalarFunction));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "", "StoredProcedure"), typeof(StoredProcedure));
        }

        /*
         * TODO: statistics are not implemented on non-SQL Server platforms
        [TestMethod]
        public void GetStatistics()
        {
            var ds = CreateTestDataset();

            Assert.IsTrue(ds.Statistics.DataSpace > 0);
        }
        */

        #endregion
        #region Table tests

        [TestMethod]
        public void GetSingleTableTest()
        {
            var ds = CreateTestDataset();

            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.MySql.Constants.DefaultSchemaName, "Author"];

            Assert.IsTrue(ds.Tables.Count == 1);
        }

        [TestMethod]
        public void GetSingleTableWithoutSchemaNameTest()
        {
            var ds = CreateTestDataset();

            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.MySql.Constants.DefaultSchemaName, "Author"];
            var t2 = ds.Tables[ds.DatabaseName, "", "Author"];

            Assert.IsTrue(ds.Tables.Count == 1);

            Assert.AreEqual(t1, t2);
        }


        [TestMethod]
        public void GetMultipleTableTest()
        {
            var ds = CreateTestDataset();

            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.MySql.Constants.DefaultSchemaName, "Author"];
            var t2 = ds.Tables[ds.DatabaseName, "", "Author"];
            Table t3 = ds.Tables[ds.DatabaseName, "", "Book"];

            Assert.IsTrue(ds.Tables.Count == 2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreEqual(t3.SchemaName, "");
        }

        [TestMethod]
        public void GetNonexistentTableTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.Tables[ds.DatabaseName, "", "NonExistentTable"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void LoadAllTablesTest()
        {
            var ds = CreateTestDataset();
            ds.Tables.LoadAll(true);

            Assert.AreEqual(5, ds.Tables.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.Tables.IsAllLoaded);
        }

        [TestMethod]
        public void GetTableColumnsTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var t1 = ds.Tables[ds.DatabaseName, "", "Author"];

            Assert.IsTrue(t1.Columns.Count == 2);
            Assert.IsTrue(t1.Columns["ID"].DataType.TypeName == "bigint");

            // Test cache
            Assert.AreEqual(t1.Columns, ds.Tables[ds.DatabaseName, "", "Author"].Columns);
            Assert.AreEqual(t1.Columns["ID"], ds.Tables[ds.DatabaseName, "", "Author"].Columns["ID"]);
        }

        [TestMethod]
        public void GetTableIndexesTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var t1 = ds.Tables[ds.DatabaseName, "", "Author"];

            Assert.IsTrue(t1.Indexes.Count == 1);
            Assert.IsTrue(t1.Indexes["PRIMARY"].IsPrimaryKey);

            // Test cache
            Assert.AreEqual(t1.Indexes, ds.Tables[ds.DatabaseName, "", "Author"].Indexes);
            Assert.AreEqual(t1.Indexes["PRIMARY"], ds.Tables[ds.DatabaseName, "", "Author"].Indexes["PRIMARY"]);

            // Table with two indices
            var t2 = ds.Tables[ds.DatabaseName, "", "tablewithindexes"];
            Assert.AreEqual(2, t2.Indexes.Count);
        }

        [TestMethod]
        public void GetTableIndexColumnsTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var ic = ds.Tables[ds.DatabaseName, "", "Author"].Indexes["PRIMARY"].Columns["ID"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

            // Test cache
            Assert.AreEqual(ic, ds.Tables[ds.DatabaseName, "", "Author"].Indexes["PRIMARY"].Columns["ID"]);

            // Non-primary key
            var ic2 = ds.Tables[ds.DatabaseName, "", "tablewithindexes"].Indexes["Data1_UNIQUE"].Columns["Data1"];
        }

        /* Statistics not implemented for MySQL
        [TestMethod]
        public void TableStatisticsTest()
        {
            
        }
         * */

        #endregion
        #region View tests

        [TestMethod]
        public void GetViewTest()
        {
            var ds = CreateTestDataset();

            var v = ds.Views[ds.DatabaseName, "", "ViewWithStar"];

            Assert.AreEqual(1, ds.Views.Count);

            Assert.AreEqual(4, v.Columns.Count);
            Assert.AreEqual("int", v.Columns["ID"].DataType.TypeName);
        }

        [TestMethod]
        public void LoadAllViewsTest()
        {
            var ds = CreateTestDataset();
            ds.Views.LoadAll(true);

            Assert.AreEqual(5, ds.Views.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.Views.IsAllLoaded);
        }

        [TestMethod]
        public void GetNonexistentViewTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.Views[ds.DatabaseName, Jhu.Graywulf.Schema.MySql.Constants.DefaultSchemaName, "NonExistentView"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        #endregion
        #region Scalar function tests

        [TestMethod]
        public void GetScalarFunctionTest()
        {
            var ds = CreateTestDataset();

            var f = ds.ScalarFunctions[ds.DatabaseName, "", "ScalarFunction"];

            Assert.AreEqual(3, f.Parameters.Count);
        }

        [TestMethod]
        public void LoadAllScalarFunctionsTest()
        {
            var ds = CreateTestDataset();

            ds.ScalarFunctions.LoadAll(true);
            Assert.AreEqual(1, ds.ScalarFunctions.Count);    // Update this if test database schema changes
        }

        [TestMethod]
        public void GetNonexistentScalarFunctionTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.ScalarFunctions[ds.DatabaseName, "", "NonExistentFunction"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        #endregion
        #region Stored procedure tests

        [TestMethod]
        public void GetStoredProcedureTest()
        {
            var ds = CreateTestDataset();

            var sp = ds.StoredProcedures[ds.DatabaseName, "", "StoredProcedure"];

            Assert.IsTrue(sp.Parameters.Count == 2);
            Assert.IsTrue(sp.Parameters["param1"].DataType.TypeName == "int");
        }
        
        [TestMethod]
        public void LoadAllStoredProceduresTest()
        {
            var ds = CreateTestDataset();

            ds.StoredProcedures.LoadAll(true);
            Assert.AreEqual(1, ds.StoredProcedures.Count);
        }

        [TestMethod]
        public void GetNonexistentStoredProcedureTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.StoredProcedures[ds.DatabaseName, Jhu.Graywulf.Schema.MySql.Constants.DefaultSchemaName, "NonExistentSp"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        #endregion
       

#if false
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
#endif
    }
}
