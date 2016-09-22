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
        #region User defined types tests

        [TestMethod]
        public void GetSimpleUdtTest()
        {
            var ds = CreateTestDataset();
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "SimpleUDT"];

            Assert.IsTrue(ds.UserDefinedTypes.Count == 1);
            Assert.IsTrue(t.IsUserDefined);
            Assert.IsFalse(t.IsTableType);
            Assert.IsFalse(t.IsAssemblyType);
            Assert.IsTrue(t.IsAlias);
            Assert.IsFalse(t.HasLength);
            Assert.IsFalse(t.HasPrecision);
            Assert.IsFalse(t.HasScale);
            Assert.AreEqual("SimpleUDT", t.TypeName);
            Assert.AreEqual("SimpleUDT", t.TypeNameWithLength);
            Assert.AreEqual(typeof(string), t.Type);
            Assert.AreEqual(SqlDbType.NVarChar, t.SqlDbType);
            Assert.AreEqual(2, t.ByteSize);
            Assert.AreEqual(0, t.Scale);
            Assert.AreEqual(0, t.Precision);
            Assert.AreEqual(50, t.Length);
            Assert.IsFalse(t.IsMaxLength);
            Assert.AreEqual(4000, t.MaxLength);
            Assert.IsFalse(t.IsFixedLength);
            Assert.IsFalse(t.IsSqlArray);
            Assert.AreEqual(0, t.ArrayLength);
            Assert.IsTrue(t.IsNullable);
        }

        [TestMethod]
        public void GetClrUdtTest()
        {
            var ds = CreateTestDataset();
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ClrUDT"];

            Assert.IsTrue(ds.UserDefinedTypes.Count == 1);
            Assert.IsTrue(t.IsUserDefined);
            Assert.IsFalse(t.IsTableType);
            Assert.IsTrue(t.IsAssemblyType);
            Assert.IsFalse(t.IsAlias);
            Assert.IsFalse(t.HasLength);
            Assert.IsFalse(t.HasPrecision);
            Assert.IsFalse(t.HasScale);
            Assert.AreEqual("ClrUDT", t.TypeName);
            Assert.AreEqual("ClrUDT", t.TypeNameWithLength);
            Assert.AreEqual(null, t.Type);
            Assert.AreEqual(SqlDbType.Udt, t.SqlDbType);
            Assert.AreEqual(1, t.ByteSize);
            Assert.AreEqual(0, t.Scale);
            Assert.AreEqual(0, t.Precision);
            Assert.AreEqual(1, t.Length);
            Assert.IsFalse(t.IsMaxLength);
            Assert.AreEqual(13, t.MaxLength);
            Assert.IsTrue(t.IsFixedLength);
            Assert.IsFalse(t.IsSqlArray);
            Assert.AreEqual(0, t.ArrayLength);
            Assert.IsTrue(t.IsNullable);
        }

        [TestMethod]
        public void GetTableTypeTest()
        {
            var ds = CreateTestDataset();
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableUDT"];

            Assert.IsTrue(ds.UserDefinedTypes.Count == 1);
            Assert.IsTrue(t.IsUserDefined);
            Assert.IsTrue(t.IsTableType);
            Assert.IsFalse(t.IsAssemblyType);
            Assert.IsFalse(t.IsAlias);
            Assert.IsFalse(t.HasLength);
            Assert.IsFalse(t.HasPrecision);
            Assert.IsFalse(t.HasScale);
            Assert.AreEqual("TableUDT", t.TypeName);
            Assert.AreEqual("TableUDT", t.TypeNameWithLength);
            Assert.AreEqual(null, t.Type);
            Assert.AreEqual(SqlDbType.Structured, t.SqlDbType);
            Assert.AreEqual(1, t.ByteSize);
            Assert.AreEqual(0, t.Scale);
            Assert.AreEqual(0, t.Precision);
            Assert.AreEqual(1, t.Length);
            Assert.IsFalse(t.IsMaxLength);
            Assert.AreEqual(-1, t.MaxLength);
            Assert.IsFalse(t.IsFixedLength);
            Assert.IsFalse(t.IsSqlArray);
            Assert.AreEqual(0, t.ArrayLength);
            Assert.IsFalse(t.IsNullable);

            Assert.AreEqual(2, t.Columns.Count);
        }

        [TestMethod]
        public void GetTableUDTWithUDTTest()
        {
            var ds = CreateTestDataset();
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableUDTWithUDT"];

            Assert.AreEqual("dbo", t.Columns["Data"].DataType.SchemaName);
            Assert.AreEqual("SimpleUDT", t.Columns["Data"].DataType.TypeName);
        }

        [TestMethod]
        public void GetTableUDTWithCLRUDTTest()
        {
            var ds = CreateTestDataset();
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableUDTWithCLRUDT"];

            Assert.AreEqual("dbo", t.Columns["Data"].DataType.SchemaName);
            Assert.AreEqual("ClrUDT", t.Columns["Data"].DataType.TypeName);
        }

        [TestMethod]
        public void GetTableUDTWithIndexTest()
        {
            var ds = CreateTestDataset();
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableUDTWithIndex"];

            Assert.AreEqual(null, t.Columns["Data"].DataType.SchemaName);
            Assert.AreEqual("nvarchar", t.Columns["Data"].DataType.TypeName);
            Assert.AreEqual(1, t.Indexes.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetNonexistentTypeTest()
        {
            var ds = CreateTestDataset();
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentType"];
        }

        [TestMethod]
        public void LoadAllTypesTest()
        {
            var ds = CreateTestDataset();
            ds.UserDefinedTypes.LoadAll();

            Assert.AreEqual(6, ds.UserDefinedTypes.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.UserDefinedTypes.IsAllLoaded);
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
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetNonexistentTableTest()
        {
            var ds = CreateTestDataset();
            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentTable"];
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
            Assert.IsTrue(t1.Columns["ID"].DataType.TypeName == "bigint");

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

            Assert.IsTrue(t.Columns["BigIntColumn"].DataType.TypeNameWithLength == "bigint");
            Assert.IsTrue(t.Columns["NumericColumn"].DataType.TypeNameWithLength == "decimal");                   // precision?
            Assert.IsTrue(t.Columns["BitColumn"].DataType.TypeNameWithLength == "bit");
            Assert.IsTrue(t.Columns["SmallIntColumn"].DataType.TypeNameWithLength == "smallint");
            Assert.IsTrue(t.Columns["DecimalColumn"].DataType.TypeNameWithLength == "decimal");
            Assert.IsTrue(t.Columns["SmallMoneyColumn"].DataType.TypeNameWithLength == "smallmoney");
            Assert.IsTrue(t.Columns["IntColumn"].DataType.TypeNameWithLength == "int");
            Assert.IsTrue(t.Columns["TinyIntColumn"].DataType.TypeNameWithLength == "tinyint");
            Assert.IsTrue(t.Columns["MoneyColumn"].DataType.TypeNameWithLength == "money");
            Assert.IsTrue(t.Columns["FloatColumn"].DataType.TypeNameWithLength == "float");
            Assert.IsTrue(t.Columns["RealColumn"].DataType.TypeNameWithLength == "real");
            Assert.IsTrue(t.Columns["DateColumn"].DataType.TypeNameWithLength == "date");
            Assert.IsTrue(t.Columns["DateTimeOffsetColumn"].DataType.TypeNameWithLength == "datetimeoffset");     // precision?
            Assert.IsTrue(t.Columns["DateTime2Column"].DataType.TypeNameWithLength == "datetime2");               // precision?
            Assert.IsTrue(t.Columns["SmallDateTimeColumn"].DataType.TypeNameWithLength == "smalldatetime");
            Assert.IsTrue(t.Columns["DateTimeColumn"].DataType.TypeNameWithLength == "datetime");
            Assert.IsTrue(t.Columns["TimeColumn"].DataType.TypeNameWithLength == "time");                         // precision?
            Assert.IsTrue(t.Columns["CharColumn"].DataType.TypeNameWithLength == "char(10)");
            Assert.IsTrue(t.Columns["VarCharColumn"].DataType.TypeNameWithLength == "varchar(10)");
            Assert.IsTrue(t.Columns["VarCharMaxColumn"].DataType.TypeNameWithLength == "varchar(max)");
            Assert.IsTrue(t.Columns["TextColumn"].DataType.TypeNameWithLength == "text");
            Assert.IsTrue(t.Columns["NCharColumn"].DataType.TypeNameWithLength == "nchar(10)");
            Assert.IsTrue(t.Columns["NVarCharColumn"].DataType.TypeNameWithLength == "nvarchar(10)");
            Assert.IsTrue(t.Columns["NVarCharMaxColumn"].DataType.TypeNameWithLength == "nvarchar(max)");
            Assert.IsTrue(t.Columns["NTextColumn"].DataType.TypeNameWithLength == "ntext");
            Assert.IsTrue(t.Columns["BinaryColumn"].DataType.TypeNameWithLength == "binary(10)");
            Assert.IsTrue(t.Columns["VarBinaryColumn"].DataType.TypeNameWithLength == "varbinary(10)");
            Assert.IsTrue(t.Columns["VarBinaryMaxColumn"].DataType.TypeNameWithLength == "varbinary(max)");
            Assert.IsTrue(t.Columns["ImageColumn"].DataType.TypeNameWithLength == "image");
            Assert.IsTrue(t.Columns["TimeStampColumn"].DataType.TypeNameWithLength == "timestamp");
            Assert.IsTrue(t.Columns["UniqueIdentifierColumn"].DataType.TypeNameWithLength == "uniqueidentifier");
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

                        Assert.IsTrue(columns[0].DataType.TypeNameWithLength == "bigint");
                        Assert.IsTrue(columns[1].DataType.TypeNameWithLength == "decimal");                   // precision?
                        Assert.IsTrue(columns[2].DataType.TypeNameWithLength == "bit");
                        Assert.IsTrue(columns[3].DataType.TypeNameWithLength == "smallint");
                        Assert.IsTrue(columns[4].DataType.TypeNameWithLength == "decimal");
                        Assert.IsTrue(columns[5].DataType.TypeNameWithLength == "smallmoney");
                        Assert.IsTrue(columns[6].DataType.TypeNameWithLength == "int");
                        Assert.IsTrue(columns[7].DataType.TypeNameWithLength == "tinyint");
                        Assert.IsTrue(columns[8].DataType.TypeNameWithLength == "money");
                        Assert.IsTrue(columns[9].DataType.TypeNameWithLength == "float");
                        Assert.IsTrue(columns[10].DataType.TypeNameWithLength == "real");
                        Assert.IsTrue(columns[11].DataType.TypeNameWithLength == "date");
                        Assert.IsTrue(columns[12].DataType.TypeNameWithLength == "datetimeoffset");     // precision?
                        Assert.IsTrue(columns[13].DataType.TypeNameWithLength == "datetime2");               // precision?
                        Assert.IsTrue(columns[14].DataType.TypeNameWithLength == "smalldatetime");
                        Assert.IsTrue(columns[15].DataType.TypeNameWithLength == "datetime");
                        Assert.IsTrue(columns[16].DataType.TypeNameWithLength == "time");                         // precision?
                        Assert.IsTrue(columns[17].DataType.TypeNameWithLength == "char(10)");
                        Assert.IsTrue(columns[18].DataType.TypeNameWithLength == "varchar(10)");
                        Assert.IsTrue(columns[19].DataType.TypeNameWithLength == "varchar(max)");
                        Assert.IsTrue(columns[20].DataType.TypeNameWithLength == "text");
                        Assert.IsTrue(columns[21].DataType.TypeNameWithLength == "nchar(10)");
                        Assert.IsTrue(columns[22].DataType.TypeNameWithLength == "nvarchar(10)");
                        Assert.IsTrue(columns[23].DataType.TypeNameWithLength == "nvarchar(max)");
                        Assert.IsTrue(columns[24].DataType.TypeNameWithLength == "ntext");
                        Assert.IsTrue(columns[25].DataType.TypeNameWithLength == "binary(10)");
                        Assert.IsTrue(columns[26].DataType.TypeNameWithLength == "varbinary(10)");
                        Assert.IsTrue(columns[27].DataType.TypeNameWithLength == "varbinary(max)");
                        Assert.IsTrue(columns[28].DataType.TypeNameWithLength == "image");
                        Assert.IsTrue(columns[29].DataType.TypeNameWithLength == "timestamp");
                        Assert.IsTrue(columns[30].DataType.TypeNameWithLength == "uniqueidentifier");
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
            Assert.AreEqual("int", v.Columns["ID"].DataType.TypeName);
        }

        [TestMethod]
        public void LoadAllViewsTest()
        {
            var ds = CreateTestDataset();
            ds.Views.LoadAll();

            Assert.AreEqual(13, ds.Views.Count);    // Update this if test database schema changes
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

        [TestMethod]
        public void GetViewIndexesTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var t1 = ds.Views[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"];

            Assert.IsTrue(t1.Indexes.Count == 1);
            Assert.IsTrue(t1.Indexes["PK_TableWithPrimaryKey"].IsPrimaryKey);

        }

        [TestMethod]
        public void GetViewIndexColumnsTest()
        {
            var ds = CreateTestDataset();

            // Get a single table
            var ic = ds.Views[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"].Indexes["PK_TableWithPrimaryKey"].Columns["ID"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

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
            Assert.IsTrue(sp.Parameters["@param1"].DataType.TypeName == "int");
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
