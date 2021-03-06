﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.QueryGeneration;

namespace Jhu.Graywulf.Sql.Schema.SqlServer
{
    [TestClass]
    public class SqlServerDatasetTest : Jhu.Graywulf.Test.TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            StopLogger();
        }

        #region Dataset tests

        [TestMethod]
        public void GetAnyObjectTest()
        {
            var ds = SchemaTestDataset;

            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithPrimaryKey"), typeof(Table));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"), typeof(View));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "InlineTableValuedFunction"), typeof(TableValuedFunction));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ScalarFunction"), typeof(ScalarFunction));
            Assert.IsInstanceOfType(ds.GetObject(ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "StoredProcedure"), typeof(StoredProcedure));
        }

        [TestMethod]
        public void GetStatistics()
        {
            var ds = SchemaTestDataset;

            Assert.IsTrue(ds.Statistics.DataSpace > 0);
            Assert.IsTrue(ds.Statistics.UsedSpace > 0);
            Assert.IsTrue(ds.Statistics.LogSpace > 0);
        }

        #endregion
        #region User defined types tests

        [TestMethod]
        public void GetSimpleUdtTest()
        {
            var ds = SchemaTestDataset;
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "SimpleUDT"];

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
            var ds = SchemaTestDataset;
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ClrUDT"];

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
            var ds = SchemaTestDataset;
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableUDT"];

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
            var ds = SchemaTestDataset;
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableUDTWithUDT"];

            Assert.AreEqual("dbo", t.Columns["Data"].DataType.SchemaName);
            Assert.AreEqual("SimpleUDT", t.Columns["Data"].DataType.TypeName);
        }

        [TestMethod]
        public void GetTableUDTWithCLRUDTTest()
        {
            var ds = SchemaTestDataset;
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableUDTWithCLRUDT"];

            Assert.AreEqual("dbo", t.Columns["Data"].DataType.SchemaName);
            Assert.AreEqual("ClrUDT", t.Columns["Data"].DataType.TypeName);
        }

        [TestMethod]
        public void GetTableUDTWithIndexTest()
        {
            var ds = SchemaTestDataset;
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableUDTWithIndex"];

            Assert.AreEqual(null, t.Columns["Data"].DataType.SchemaName);
            Assert.AreEqual("nvarchar", t.Columns["Data"].DataType.TypeName);
            Assert.AreEqual(1, t.Indexes.Count);
            Assert.AreEqual(1, t.Indexes.Values.FirstOrDefault().Columns.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetNonexistentTypeTest()
        {
            var ds = SchemaTestDataset;
            var t = ds.UserDefinedTypes[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentType"];
        }

        [TestMethod]
        public void LoadAllTypesTest()
        {
            var ds = SchemaTestDataset;
            ds.UserDefinedTypes.LoadAll(true);

            Assert.AreEqual(7, ds.UserDefinedTypes.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.UserDefinedTypes.IsAllLoaded);
        }

        #endregion
        #region Table tests

        [TestMethod]
        public void GetSingleTableTest()
        {
            var ds = SchemaTestDataset;

            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];

            Assert.IsTrue(ds.Tables.Count == 1);
        }

        [TestMethod]
        public void GetSingleTableWithoutSchemaNameTest()
        {
            var ds = SchemaTestDataset;

            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];
            var t2 = ds.Tables[ds.DatabaseName, "", "Author"];

            Assert.IsTrue(ds.Tables.Count == 1);

            Assert.AreEqual(t1, t2);
        }


        [TestMethod]
        public void GetMultipleTableTest()
        {
            var ds = SchemaTestDataset;

            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];
            var t2 = ds.Tables[ds.DatabaseName, "", "Author"];
            Table t3 = ds.Tables[ds.DatabaseName, "", "Book"];

            Assert.IsTrue(ds.Tables.Count == 2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreEqual(t3.SchemaName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName);

            // Try to load object that's not a table

            // Use wrong database name
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetNonexistentTableTest()
        {
            var ds = SchemaTestDataset;
            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentTable"];
        }

        [TestMethod]
        public void LoadAllTablesTest()
        {
            var ds = SchemaTestDataset;
            ds.Tables.LoadAll(true);

            Assert.IsTrue(0 < ds.Tables.Count);
            Assert.IsTrue(ds.Tables.IsAllLoaded);
        }

        [TestMethod]
        public void GetTableColumnsTest()
        {
            var ds = SchemaTestDataset;

            // Get a single table
            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];

            Assert.IsTrue(t1.Columns.Count == 2);
            Assert.IsTrue(t1.Columns["ID"].DataType.TypeName == "bigint");

            Assert.IsTrue(t1.Columns["ID"].IsKey);
            Assert.IsFalse(t1.Columns["Name"].IsKey);

            // Test cache
            Assert.AreEqual(t1.Columns, ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Columns);
            Assert.AreEqual(t1.Columns["ID"], ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Columns["ID"]);
        }

        [TestMethod]
        public void GetTableIndexesTest()
        {
            var ds = SchemaTestDataset;

            // Get a single table
            var t1 = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];

            Assert.IsTrue(t1.Indexes.Count == 1);
            Assert.IsTrue(t1.Indexes["PK_Author"].IsPrimaryKey);

            // Test cache
            Assert.AreEqual(t1.Indexes, ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Indexes);
            Assert.AreEqual(t1.Indexes["PK_Author"], ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Indexes["PK_Author"]);
        }

        [TestMethod]
        public void GetTableIndexColumnsTest()
        {
            var ds = SchemaTestDataset;

            // Get a single table
            var ic = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Indexes["PK_Author"].Columns["ID"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

            // Test cache
            Assert.AreEqual(ic, ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"].Indexes["PK_Author"].Columns["ID"]);
        }

        [TestMethod]
        public void TableStatisticsTest()
        {
            var ds = SchemaTestDataset;

            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "Author"];
            Assert.IsTrue(t.Statistics.RowCount == 0);
        }

        #endregion
        #region Create table and index tests

        private Table CreateTestTable(bool createPrimaryKey, bool createIndex)
        {
            var ds = SchemaTestDataset;
            ds.IsMutable = true;

            if (ds.Tables.ContainsKey(null, "dbo", "testtable"))
            {
                ds.Tables[null, "dbo", "testtable"].Drop();
            }

            var table = new Table(ds)
            {
                SchemaName = "dbo",
                TableName = "testtable",
            };

            var id = new Column(table)
            {
                ID = 0,
                ColumnName = "ID",
                DataType = DataTypes.SqlBigInt
            };

            var data = new Column(table)
            {
                ID = 1,
                ColumnName = "Data",
                DataType = DataTypes.SqlNVarChar
            };

            table.Columns.TryAdd(id.ColumnName, id);
            table.Columns.TryAdd(data.ColumnName, data);

            if (createPrimaryKey)
            {
                CreatePrimaryKey(table);
            }

            if (createIndex)
            {
                CreateIndex(table);
            }

            return table;
        }

        private Index CreatePrimaryKey(Table table)
        {
            var pk = new Index(table)
            {
                IndexName = "PK_testtable",
                IsPrimaryKey = true,
                IsClustered = true,
                IsUnique = true,
                IsCompressed = true,
            };

            var pkc = new IndexColumn(table.Columns["ID"])
            {
                KeyOrdinal = 0,
            };
            pk.Columns.TryAdd(pkc.Name, pkc);
            table.Indexes.TryAdd(pk.IndexName, pk);

            return pk;
        }

        private Index CreateIndex(Table table)
        {
            var ix = new Index(table)
            {
                IndexName = "IX_testtable_Data",
                IsPrimaryKey = false,
                IsClustered = false,
                IsUnique = false,
                IsCompressed = true
            };

            var ixc = new IndexColumn(table.Columns["Data"])
            {
                KeyOrdinal = 0,
            };
            ix.Columns.TryAdd(ixc.Name, ixc);
            table.Indexes.TryAdd(ix.IndexName, ix);

            return ix;
        }

        [TestMethod]
        public void CreateTableTest()
        {
            var table = CreateTestTable(false, false);
            table.Create();
            table.Drop();
        }

        [TestMethod]
        public void CreateTableWithIndexesTest()
        {
            var table = CreateTestTable(true, true);
            table.Create();
            table.Drop();
        }

        [TestMethod]
        public void AddIndexesTest()
        {
            var table = CreateTestTable(false, false);
            table.Create();

            var pk = CreatePrimaryKey(table);
            pk.Create();

            var ix = CreateIndex(table);
            ix.Create();

            table.Drop();
        }

        [TestMethod]
        public void DropIndexesTest()
        {
            var table = CreateTestTable(true, true);
            table.Create();

            table.Indexes["PK_testtable"].Drop();
            table.Indexes["IX_testtable_Data"].Drop();

            table.Drop();
        }

        [TestMethod]
        public void AddColumnTest()
        {
            var table = CreateTestTable(false, false);
            table.Create();
        }

        #endregion
        #region Columns test

        [TestMethod]
        public void ColumnTypesTest()
        {
            var ds = SchemaTestDataset;

            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithAllTypes"];

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

        #endregion
        #region View tests

        [TestMethod]
        public void GetViewTest()
        {
            var ds = SchemaTestDataset;

            var v = ds.Views[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"];

            Assert.AreEqual(1, ds.Views.Count);

            Assert.AreEqual(4, v.Columns.Count);
            Assert.AreEqual("int", v.Columns["ID"].DataType.TypeName);
        }

        [TestMethod]
        public void LoadAllViewsTest()
        {
            var ds = SchemaTestDataset;
            ds.Views.LoadAll(true);

            Assert.AreEqual(6, ds.Views.Count);    // Update this if test database schema changes
            Assert.IsTrue(ds.Views.IsAllLoaded);
        }

        [TestMethod]
        public void GetNonexistentViewTest()
        {
            var ds = SchemaTestDataset;

            try
            {
                var t = ds.Views[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentView"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void GetViewUnderlyingPrimaryKeyTest()
        {
            var ds = SchemaTestDataset;
            var v = ds.Views[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"];
            
            Assert.AreEqual(1, v.Indexes.Count);
            Assert.IsTrue(v.Indexes.FirstOrDefault().Value.IsUnique);
            Assert.IsTrue(v.Columns["ID"].IsKey);
            Assert.IsFalse(v.Columns["Data1"].IsKey);

            v = ds.Views[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar2"];

            Assert.AreEqual(1, v.Indexes.Count);
            Assert.IsTrue(v.Indexes.FirstOrDefault().Value.IsUnique);
            Assert.IsTrue(v.Columns["ID"].IsKey);
            Assert.IsFalse(v.Columns["Data1"].IsKey);
        }

        [TestMethod]
        public void GetViewIndexesTest()
        {
            var ds = SchemaTestDataset;

            // Get a single table
            var t1 = ds.Views[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"];

            Assert.IsTrue(t1.Indexes.Count == 1);
            Assert.IsTrue(t1.Indexes["PK_TableWithPrimaryKey"].IsPrimaryKey);

        }

        [TestMethod]
        public void GetViewIndexColumnsTest()
        {
            var ds = SchemaTestDataset;

            // Get a single table
            var ic = ds.Views[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ViewWithStar"].Indexes["PK_TableWithPrimaryKey"].Columns["ID"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

        }

        #endregion
        #region Table-valued function tests

        [TestMethod]
        public void GetInlineTableValuedFunctionTest()
        {
            var ds = SchemaTestDataset;

            var tvf = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "InlineTableValuedFunction"];

            Assert.AreEqual(2, tvf.Parameters.Count);
            Assert.AreEqual(2, tvf.Columns.Count);
        }

        [TestMethod]
        public void GetMultiStatementTableValuedFunctionTest()
        {
            var ds = SchemaTestDataset;

            var tvf = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "MultiStatementTableValuedFunction"];

            Assert.AreEqual(2, tvf.Parameters.Count);
            Assert.AreEqual(2, tvf.Columns.Count);
        }

        [TestMethod]
        public void GetClrTableValuedFunctionTest()
        {
            var ds = SchemaTestDataset;
            var tvf = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ClrTableValuedFunction"];

            Assert.AreEqual(1, tvf.Parameters.Count);
            Assert.AreEqual(1, tvf.Columns.Count);
        }

        [TestMethod]
        public void LoadAllTableValuedFunctionTest()
        {
            var ds = SchemaTestDataset;

            ds.TableValuedFunctions.LoadAll(true);
            Assert.AreEqual(4, ds.TableValuedFunctions.Count);    // Update this if test database schema changes
        }

        [TestMethod]
        public void GetNonexistentTableValuedFunctionTest()
        {
            var ds = SchemaTestDataset;

            try
            {
                var t = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentFunction"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void GetScalarFunctionAsTableValuedFunctionTest()
        {
            var ds = SchemaTestDataset;

            try
            {
                var t = ds.TableValuedFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ScalarFunction"];
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
            var ds = SchemaTestDataset;
            var f = ds.ScalarFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ScalarFunction"];

            Assert.AreEqual(3, f.Parameters.Count);
        }

        [TestMethod]
        public void GetClrScalarFunctionTest()
        {
            var ds = SchemaTestDataset;
            var f = ds.ScalarFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ClrFunction"];

            Assert.AreEqual(2, f.Parameters.Count);
        }

        [TestMethod]
        public void LoadAllScalarFunctionsTest()
        {
            var ds = SchemaTestDataset;

            ds.ScalarFunctions.LoadAll(true);
            Assert.AreEqual(3, ds.ScalarFunctions.Count);    // Update this if test database schema changes
        }

        [TestMethod]
        public void GetNonexistentScalarFunctionTest()
        {
            var ds = SchemaTestDataset;

            try
            {
                var t = ds.ScalarFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentFunction"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void GetTableValuedFunctionAsScalarFunctionTest()
        {
            var ds = SchemaTestDataset;

            try
            {
                var t = ds.ScalarFunctions[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "InlineTableValuedFunction"];
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
            var ds = SchemaTestDataset;

            var sp = ds.StoredProcedures[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "StoredProcedure"];

            Assert.IsTrue(sp.Parameters.Count == 2);
            Assert.IsTrue(sp.Parameters["@param1"].DataType.TypeName == "int");
        }

        [TestMethod]
        public void GetClrStoredProcedureTest()
        {
            var ds = SchemaTestDataset;

            var sp = ds.StoredProcedures[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "ClrStoredProc"];

            Assert.IsTrue(sp.Parameters.Count == 1);
            Assert.IsTrue(sp.Parameters["@parameter"].DataType.TypeName == "int");
        }

        [TestMethod]
        public void LoadAllStoredProceduresTest()
        {
            var ds = SchemaTestDataset;

            ds.StoredProcedures.LoadAll(true);
            Assert.AreEqual(2, ds.StoredProcedures.Count);
        }

        [TestMethod]
        public void GetNonexistentStoredProcedureTest()
        {
            var ds = SchemaTestDataset;

            try
            {
                var t = ds.StoredProcedures[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "NonExistentSp"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        #endregion
    }
}
