using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Data;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.SqlCodeGen.SqlServer;

namespace Jhu.Graywulf.Schema.SqlServer.Test
{
    [TestClass]
    public class SqlServerDatasetTest : SqlServerTestBase
    {
        #region Dataset tests

        [TestMethod]
        public void GetAnyObjectTest()
        {
            var ds = CreateTestDataset();

            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithPrimaryKey"), typeof(Table));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"), typeof(View));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "InlineTableValuedFunction"), typeof(TableValuedFunction));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ScalarFunction"), typeof(ScalarFunction));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "StoredProcedure"), typeof(StoredProcedure));
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

            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];

            Assert.IsTrue(ds.Tables.Count == 1);
        }

        [TestMethod]
        public void GetSingleTableWithoutSchemaNameTest()
        {
            var ds = CreateTestDataset();

            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];
            var t2 = ds.Tables[ds.DatabaseName, "", "Author"];

            Assert.IsTrue(ds.Tables.Count == 1);

            Assert.AreEqual(t1, t2);
        }


        [TestMethod]
        public void GetMultipleTableTest()
        {
            var ds = CreateTestDataset();

            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];
            var t2 = ds.Tables[ds.DatabaseName, "", "Author"];
            Table t3 = ds.Tables[ds.DatabaseName, "", "Book"];

            Assert.IsTrue(ds.Tables.Count == 2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreEqual(t3.SchemaName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName);

            // Try to load object that's not a table

            // Use wrong database name
        }

        [TestMethod]
        public void GetNonexistentTableTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentTable"];
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

            Assert.AreEqual(7, ds.Tables.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.Tables.IsAllLoaded);
        }

        [TestMethod]
        public void GetTableColumnsTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];

            Assert.IsTrue(t1.Columns.Count == 2);
            Assert.IsTrue(t1.Columns["ID"].DataType.Name == "bigint");

            // Test cache
            Assert.AreEqual(t1.Columns, ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Columns);
            Assert.AreEqual(t1.Columns["ID"], ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Columns["ID"]);
        }

        [TestMethod]
        public void GetTableIndexesTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];

            Assert.IsTrue(t1.Indexes.Count == 1);
            Assert.IsTrue(t1.Indexes["PK_Author"].IsPrimaryKey);

            // Test cache
            Assert.AreEqual(t1.Indexes, ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Indexes);
            Assert.AreEqual(t1.Indexes["PK_Author"], ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Indexes["PK_Author"]);
        }

        [TestMethod]
        public void GetTableIndexColumnsTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var ic = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Indexes["PK_Author"].Columns["ID"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

            // Test cache
            Assert.AreEqual(ic, ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Indexes["PK_Author"].Columns["ID"]);
        }

        [TestMethod]
        public void TableStatisticsTest()
        {
            var ds = CreateTestDataset();

            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];
            Assert.IsTrue(t.Statistics.RowCount == 0);
        }

        #endregion
        #region Columns test

        [TestMethod]
        public void ColumnTypesTest()
        {
            var ds = CreateTestDataset();

            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithAllTypes"];

            Assert.IsTrue(t.Columns.Count == 31);

            Assert.IsTrue(t.Columns["BigIntColumn"].DataType.NameWithLength == "bigint");
            Assert.IsTrue(t.Columns["NumericColumn"].DataType.NameWithLength == "decimal");                   // precision?
            Assert.IsTrue(t.Columns["BitColumn"].DataType.NameWithLength == "bit");
            Assert.IsTrue(t.Columns["SmallIntColumn"].DataType.NameWithLength == "smallint");
            Assert.IsTrue(t.Columns["DecimalColumn"].DataType.NameWithLength == "decimal");
            Assert.IsTrue(t.Columns["SmallMoneyColumn"].DataType.NameWithLength == "smallmoney");
            Assert.IsTrue(t.Columns["IntColumn"].DataType.NameWithLength == "int");
            Assert.IsTrue(t.Columns["TinyIntColumn"].DataType.NameWithLength == "tinyint");
            Assert.IsTrue(t.Columns["MoneyColumn"].DataType.NameWithLength == "money");
            Assert.IsTrue(t.Columns["FloatColumn"].DataType.NameWithLength == "float");
            Assert.IsTrue(t.Columns["RealColumn"].DataType.NameWithLength == "real");
            Assert.IsTrue(t.Columns["DateColumn"].DataType.NameWithLength == "date");
            Assert.IsTrue(t.Columns["DateTimeOffsetColumn"].DataType.NameWithLength == "datetimeoffset");     // precision?
            Assert.IsTrue(t.Columns["DateTime2Column"].DataType.NameWithLength == "datetime2");               // precision?
            Assert.IsTrue(t.Columns["SmallDateTimeColumn"].DataType.NameWithLength == "smalldatetime");
            Assert.IsTrue(t.Columns["DateTimeColumn"].DataType.NameWithLength == "datetime");
            Assert.IsTrue(t.Columns["TimeColumn"].DataType.NameWithLength == "time");                         // precision?
            Assert.IsTrue(t.Columns["CharColumn"].DataType.NameWithLength == "char(10)");
            Assert.IsTrue(t.Columns["VarCharColumn"].DataType.NameWithLength == "varchar(10)");
            Assert.IsTrue(t.Columns["VarCharMaxColumn"].DataType.NameWithLength == "varchar(max)");
            Assert.IsTrue(t.Columns["TextColumn"].DataType.NameWithLength == "text");
            Assert.IsTrue(t.Columns["NCharColumn"].DataType.NameWithLength == "nchar(10)");
            Assert.IsTrue(t.Columns["NVarCharColumn"].DataType.NameWithLength == "nvarchar(10)");
            Assert.IsTrue(t.Columns["NVarCharMaxColumn"].DataType.NameWithLength == "nvarchar(max)");
            Assert.IsTrue(t.Columns["NTextColumn"].DataType.NameWithLength == "ntext");
            Assert.IsTrue(t.Columns["BinaryColumn"].DataType.NameWithLength == "binary(10)");
            Assert.IsTrue(t.Columns["VarBinaryColumn"].DataType.NameWithLength == "varbinary(10)");
            Assert.IsTrue(t.Columns["VarBinaryMaxColumn"].DataType.NameWithLength == "varbinary(max)");
            Assert.IsTrue(t.Columns["ImageColumn"].DataType.NameWithLength == "image");
            Assert.IsTrue(t.Columns["TimeStampColumn"].DataType.NameWithLength == "timestamp");
            Assert.IsTrue(t.Columns["UniqueIdentifierColumn"].DataType.NameWithLength == "uniqueidentifier");
        }

        [TestMethod]
        public void ColumnTypesTest2()
        {
            var ds = CreateTestDataset();
            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithAllTypes"];
            var codegen = SqlCodeGeneratorFactory.CreateCodeGenerator(ds);
            var sql = codegen.GenerateSelectStarQuery(t, 100);

            using (var cn = t.Dataset.OpenConnection())
            {
                using (var cmd = new SmartCommand(ds, cn.CreateCommand()))
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        var columns = dr.Columns;

                        Assert.IsTrue(columns.Count == 31);

                        var o = 0;

                        Assert.IsTrue(columns[0].DataType.NameWithLength == "bigint");
                        Assert.IsTrue(columns[1].DataType.NameWithLength == "decimal");                   // precision?
                        Assert.IsTrue(columns[2].DataType.NameWithLength == "bit");
                        Assert.IsTrue(columns[3].DataType.NameWithLength == "smallint");
                        Assert.IsTrue(columns[4].DataType.NameWithLength == "decimal");
                        Assert.IsTrue(columns[5].DataType.NameWithLength == "smallmoney");
                        Assert.IsTrue(columns[6].DataType.NameWithLength == "int");
                        Assert.IsTrue(columns[7].DataType.NameWithLength == "tinyint");
                        Assert.IsTrue(columns[8].DataType.NameWithLength == "money");
                        Assert.IsTrue(columns[9].DataType.NameWithLength == "float");
                        Assert.IsTrue(columns[10].DataType.NameWithLength == "real");
                        Assert.IsTrue(columns[11].DataType.NameWithLength == "date");
                        Assert.IsTrue(columns[12].DataType.NameWithLength == "datetimeoffset");     // precision?
                        Assert.IsTrue(columns[13].DataType.NameWithLength == "datetime2");               // precision?
                        Assert.IsTrue(columns[14].DataType.NameWithLength == "smalldatetime");
                        Assert.IsTrue(columns[15].DataType.NameWithLength == "datetime");
                        Assert.IsTrue(columns[16].DataType.NameWithLength == "time");                         // precision?
                        Assert.IsTrue(columns[17].DataType.NameWithLength == "char(10)");
                        Assert.IsTrue(columns[18].DataType.NameWithLength == "varchar(10)");
                        Assert.IsTrue(columns[19].DataType.NameWithLength == "varchar(max)");
                        Assert.IsTrue(columns[20].DataType.NameWithLength == "text");
                        Assert.IsTrue(columns[21].DataType.NameWithLength == "nchar(10)");
                        Assert.IsTrue(columns[22].DataType.NameWithLength == "nvarchar(10)");
                        Assert.IsTrue(columns[23].DataType.NameWithLength == "nvarchar(max)");
                        Assert.IsTrue(columns[24].DataType.NameWithLength == "ntext");
                        Assert.IsTrue(columns[25].DataType.NameWithLength == "binary(10)");
                        Assert.IsTrue(columns[26].DataType.NameWithLength == "varbinary(10)");
                        Assert.IsTrue(columns[27].DataType.NameWithLength == "varbinary(max)");
                        Assert.IsTrue(columns[28].DataType.NameWithLength == "image");
                        Assert.IsTrue(columns[29].DataType.NameWithLength == "timestamp");
                        Assert.IsTrue(columns[30].DataType.NameWithLength == "uniqueidentifier");
                    }
                }
            }
        }

        #endregion
        #region View tests

        [TestMethod]
        public void GetViewTest()
        {
            var ds = CreateTestDataset();

            var v = ds.Views[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"];

            Assert.AreEqual(1, ds.Views.Count);

            Assert.AreEqual(4, v.Columns.Count);
            Assert.AreEqual("int", v.Columns["ID"].DataType.Name);
        }

        [TestMethod]
        public void LoadAllViewsTest()
        {
            var ds = CreateTestDataset();
            ds.Views.LoadAll();

            Assert.AreEqual(6, ds.Views.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.Views.IsAllLoaded);
        }

        [TestMethod]
        public void GetNonexistentViewTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.Views[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentView"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void GetViewUnderlyingPrimaryKeyTest()
        {
            var ds = CreateTestDataset();
            var v = ds.Views[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"];

            Assert.AreEqual(1, v.Indexes.Count);
            Assert.IsTrue(v.Indexes.FirstOrDefault().Value.IsUnique);

            v = ds.Views[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar2"];

            Assert.AreEqual(1, v.Indexes.Count);
            Assert.IsTrue(v.Indexes.FirstOrDefault().Value.IsUnique);
        }

        #endregion
        #region Table-valued function tests

        [TestMethod]
        public void GetInlineTableValuedFunctionTest()
        {
            var ds = CreateTestDataset();

            var tvf = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "InlineTableValuedFunction"];

            Assert.AreEqual(2, tvf.Parameters.Count);
            Assert.AreEqual(2, tvf.Columns.Count);
        }

        [TestMethod]
        public void GetMultiStatementTableValuedFunctionTest()
        {
            var ds = CreateTestDataset();

            var tvf = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "MultiStatementTableValuedFunction"];

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
            Assert.AreEqual(3, ds.TableValuedFunctions.Count);    // Update this if test database schema changes
        }

        [TestMethod]
        public void GetNonexistentTableValuedFunctionTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentFunction"];
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
                var t = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ScalarFunction"];
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

            var f = ds.ScalarFunctions[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ScalarFunction"];

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
            Assert.AreEqual(2, ds.ScalarFunctions.Count);    // Update this if test database schema changes
        }

        [TestMethod]
        public void GetNonexistentScalarFunctionTest()
        {
            var ds = CreateTestDataset();

            try
            {
                var t = ds.ScalarFunctions[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentFunction"];
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
                var t = ds.ScalarFunctions[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "InlineTableValuedFunction"];
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

            var sp = ds.StoredProcedures[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "StoredProcedure"];

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
                var t = ds.StoredProcedures[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentSp"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        #endregion
    }
}
