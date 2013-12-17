using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Types;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.Schema.SqlServer.Test
{
    [TestClass]
    public class SqlServerDatasetTest
    {
        private SqlServerDataset CreateTestDataset()
        {
            var csb = new SqlConnectionStringBuilder(Jhu.Graywulf.Schema.Test.AppSettings.SqlServerConnectionString);

            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, csb.ConnectionString)
            {
                DatabaseName = csb.InitialCatalog
            };

            return ds;
        }

        #region Dataset tests

        [TestMethod]
        public void GetAnyObjectTest()
        {
            var ds = CreateTestDataset();

            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "dbo", "TableWithPrimaryKey"), typeof(Table));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "dbo", "ViewWithStar"), typeof(View));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "dbo", "InlineTableValuedFunction"), typeof(TableValuedFunction));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "dbo", "ScalarFunction"), typeof(ScalarFunction));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, "dbo", "StoredProcedure"), typeof(StoredProcedure));
        }

        [TestMethod]
        public void GetStatistics()
        {
            var ds = CreateTestDataset();

            Assert.IsTrue(ds.Statistics.DataSpace > 0);
        }

        #endregion
        #region Table tests

        [TestMethod]
        public void GetSingleTableTest()
        {
            var ds = CreateTestDataset();

            var t = ds.Tables[ds.DatabaseName, "dbo", "Author"];
            
            Assert.IsTrue(ds.Tables.Count == 1);
        }

        [TestMethod]
        public void GetSingleTableWithoutSchemaNameTest()
        {
            var ds = CreateTestDataset();

            var t1 = ds.Tables[ds.DatabaseName, "dbo", "Author"];
            var t2 = ds.Tables[ds.DatabaseName, "", "Author"];
            
            Assert.IsTrue(ds.Tables.Count == 1);
            
            Assert.AreEqual(t1, t2);
        }


        [TestMethod]
        public void GetMultipleTableTest()
        {
            var ds = CreateTestDataset();

            var t1 = ds.Tables[ds.DatabaseName, "dbo", "Author"];
            var t2 = ds.Tables[ds.DatabaseName, "", "Author"];
            Table t3 = ds.Tables[ds.DatabaseName, "", "Book"];
            
            Assert.IsTrue(ds.Tables.Count == 2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreEqual(t3.SchemaName, "dbo");

            // Try to load object that's not a table

            // Use wrong database name
        }

        [TestMethod]
        public void GetNonexistentTableTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.Tables[ds.DatabaseName, "dbo", "NonExistentTable"];
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
            ds.Tables.LoadAll();

            Assert.AreEqual(4, ds.Tables.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.Tables.IsAllLoaded);
        }

        [TestMethod]
        public void GetTableColumnsTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var t1 = ds.Tables[ds.DatabaseName, "dbo", "Author"];

            Assert.IsTrue(t1.Columns.Count == 2);
            Assert.IsTrue(t1.Columns["ID"].DataType.Name == "bigint");

            // Test cache
            Assert.AreEqual(t1.Columns, ds.Tables[ds.DatabaseName, "dbo", "Author"].Columns);
            Assert.AreEqual(t1.Columns["ID"], ds.Tables[ds.DatabaseName, "dbo", "Author"].Columns["ID"]);
        }

        [TestMethod]
        public void GetTableIndexesTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var t1 = ds.Tables[ds.DatabaseName, "dbo", "Author"];

            Assert.IsTrue(t1.Indexes.Count == 1);
            Assert.IsTrue(t1.Indexes["PK_Author"].IsPrimaryKey);

            // Test cache
            Assert.AreEqual(t1.Indexes, ds.Tables[ds.DatabaseName, "dbo", "Author"].Indexes);
            Assert.AreEqual(t1.Indexes["PK_Author"], ds.Tables[ds.DatabaseName, "dbo", "Author"].Indexes["PK_Author"]);
        }

        [TestMethod]
        public void GetTableIndexColumnsTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var ic = ds.Tables[ds.DatabaseName, "dbo", "Author"].Indexes["PK_Author"].Columns["ID"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

            // Test cache
            Assert.AreEqual(ic, ds.Tables[ds.DatabaseName, "dbo", "Author"].Indexes["PK_Author"].Columns["ID"]);
        }

        [TestMethod]
        public void TableStatisticsTest()
        {
            var ds = CreateTestDataset();

            var t = ds.Tables[ds.DatabaseName, "dbo", "Author"];
            Assert.IsTrue(t.Statistics.RowCount == 0);
        }

        #endregion
        #region View tests

        [TestMethod]
        public void GetViewTest()
        {
            var ds = CreateTestDataset();

            var v = ds.Views[ds.DatabaseName, "dbo", "ViewWithStar"];

            Assert.AreEqual(1, ds.Views.Count);

            Assert.AreEqual(4, v.Columns.Count);
            Assert.AreEqual("int", v.Columns["ID"].DataType.Name);
        }

        [TestMethod]
        public void LoadAllViewsTest()
        {
            var ds = CreateTestDataset();
            ds.Views.LoadAll();

            Assert.AreEqual(5, ds.Views.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.Views.IsAllLoaded);
        }

        [TestMethod]
        public void GetNonexistentViewTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.Views[ds.DatabaseName, "dbo", "NonExistentView"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        #endregion
        #region Table-valued function tests

        [TestMethod]
        public void GetInlineTableValuedFunctionTest()
        {
            var ds = CreateTestDataset();

            var tvf = ds.TableValuedFunctions[ds.DatabaseName, "dbo", "InlineTableValuedFunction"];

            Assert.AreEqual(2, tvf.Parameters.Count);
            Assert.AreEqual(2, tvf.Columns.Count);
        }

        [TestMethod]
        public void GetMultiStatementTableValuedFunctionTest()
        {
            var ds = CreateTestDataset();

            var tvf = ds.TableValuedFunctions[ds.DatabaseName, "dbo", "MultiStatementTableValuedFunction"];

            Assert.AreEqual(2, tvf.Parameters.Count);
            Assert.AreEqual(2, tvf.Columns.Count);
        }

        [TestMethod]
        public void GetClrTableValuedFunctionTest()
        {
            // TODO: implement test
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoadAllTableValuedFunctionTest()
        {
            var ds = CreateTestDataset();

            ds.TableValuedFunctions.LoadAll();
            Assert.IsTrue(ds.TableValuedFunctions.Count == 2);    // Update this if test database schema changes
        }

        [TestMethod]
        public void GetNonexistentTableValuedFunctionTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.TableValuedFunctions[ds.DatabaseName, "dbo", "NonExistentFunction"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void GetScalarFunctionAsTableValuedFunctionTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.TableValuedFunctions[ds.DatabaseName, "dbo", "ScalarFunction"];
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

            var f = ds.ScalarFunctions[ds.DatabaseName, "dbo", "ScalarFunction"];

            Assert.AreEqual(3, f.Parameters.Count);
        }

        [TestMethod]
        public void GetClrScalarFunctionTest()
        {
            // TODO: implement test
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoadAllScalarFunctionsTest()
        {
            var ds = CreateTestDataset();

            ds.ScalarFunctions.LoadAll();
            Assert.IsTrue(ds.ScalarFunctions.Count == 1);    // Update this if test database schema changes
        }

        [TestMethod]
        public void GetNonexistentScalarFunctionTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.ScalarFunctions[ds.DatabaseName, "dbo", "NonExistentFunction"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void GetTableValuedFunctionAsScalarFunctionTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.ScalarFunctions[ds.DatabaseName, "dbo", "InlineTableValuedFunction"];
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

            var sp = ds.StoredProcedures[ds.DatabaseName, "dbo", "StoredProcedure"];

            Assert.IsTrue(sp.Parameters.Count == 2);
            Assert.IsTrue(sp.Parameters["@param1"].DataType.Name == "int");
        }

        [TestMethod]
        public void GetClrStoredProcedureTest()
        {
            // TODO: implement test
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoadAllStoredProceduresTest()
        {
            var ds = CreateTestDataset();

            ds.StoredProcedures.LoadAll();
            Assert.AreEqual(1, ds.StoredProcedures.Count);
        }

        [TestMethod]
        public void GetNonexistentStoredProcedureTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.StoredProcedures[ds.DatabaseName, "dbo", "NonExistentSp"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        #endregion
    }
}
