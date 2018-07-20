using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class DataTypeTest
    {
        private DataTypeIdentifier Parse(string query)
        {
            var p = new SqlParser();
            return (DataTypeIdentifier)p.Execute(new DataTypeIdentifier(), query);
        }

        [TestMethod]
        public void SimpleTypeTest()
        {
            var sql = "int";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void TypeWithSizeTest()
        {
            var dt = Parse("nvarchar(10)");
            dt = Parse("nvarchar (10)");
            dt = Parse("nvarchar ( 10 )");
        }

        [TestMethod]
        public void TypeWithMaxSizeTest()
        {
            var sql = "nvarchar(max)";
            var sb = Parse(sql);

            sql = "nvarchar (max)";
            sb = Parse(sql);

            sql = "nvarchar ( max )";
            sb = Parse(sql);
        }

        [TestMethod]
        public void TypeWithScaleAndPrecisionTest()
        {
            var sql = "decimal(5,7)";
            var sb = Parse(sql);

            sql = "decimal ( 5 , 7 )";
            sb = Parse(sql);
        }

        [TestMethod]
        public void AllSystemTypesTest()
        {
            var dt = Parse("tinyint");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.TinyInt, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("smallint");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.SmallInt, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("int");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Int, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("bigint");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.BigInt, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("decimal");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Decimal, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(18, dt.DataTypeReference.DataType.Precision);
            Assert.AreEqual(0, dt.DataTypeReference.DataType.Scale);

            dt = Parse("decimal(5)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Decimal, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(5, dt.DataTypeReference.DataType.Precision);
            Assert.AreEqual(0, dt.DataTypeReference.DataType.Scale);

            dt = Parse("decimal(5, 3)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Decimal, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(5, dt.DataTypeReference.DataType.Precision);
            Assert.AreEqual(3, dt.DataTypeReference.DataType.Scale);

            dt = Parse("smallmoney");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.SmallMoney, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("money");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Money, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("numeric");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Decimal, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("real");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Real, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("float");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Float, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("date");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Date, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("time");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Time, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("smalldatetime");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.SmallDateTime, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("datetime");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.DateTime, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("datetime2");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.DateTime2, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("datetimeoffset");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.DateTimeOffset, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("char(10)");
            Assert.AreEqual(SqlDbType.Char, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("varchar(10)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.VarChar, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("varchar(max)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.VarChar, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("text");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Text, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("nvarchar(10)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.NVarChar, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("nvarchar(max)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.NVarChar, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("ntext");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.NText, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            //dt = Parse("xml");
            //Assert.AreEqual(SqlDbType.Xml, dt.DataTypeReference.DataType.SqlDbType);
            //Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("binary(10)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Binary, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("varbinary(10)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.VarBinary, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("varbinary(max)");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.VarBinary, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("image");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Image, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            //dt = Parse("variant");
            //Assert.AreEqual(SqlDbType.Variant, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("timestamp");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.Timestamp, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("uniqueidentifier");
            Assert.IsFalse(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual(SqlDbType.UniqueIdentifier, dt.DataTypeReference.DataType.SqlDbType);
        }

        [TestMethod]
        public void UdtTest()
        {
            var sql = "ClrUDT";
            var dt = Parse(sql);
            Assert.IsTrue(dt.DataTypeReference.IsUserDefined);
        }

        [TestMethod]
        public void UdtWithSchemaNameTest()
        {
            var sql = "dbo.ClrUDT";
            var dt = Parse(sql);
            Assert.IsTrue(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual("dbo", dt.DataTypeReference.SchemaName);
            Assert.AreEqual("ClrUDT", dt.DataTypeReference.DatabaseObjectName);

            sql = "dbo.int";
            dt = Parse(sql);
            Assert.IsTrue(dt.DataTypeReference.IsUserDefined);
            Assert.AreEqual("dbo", dt.DataTypeReference.SchemaName);
            Assert.AreEqual("int", dt.DataTypeReference.DatabaseObjectName);
        }
        
        [TestMethod]
        [ExpectedException(typeof(Jhu.Graywulf.Parsing.ParserException))]
        public void UdtNameWithTooManyPartsTest()
        {
            // This is not standard T-SQL which doesn't support cross-database UDTs
            var sql = "test.database.schema.ClrUDT";
            var dt = Parse(sql);
        }
    }
}
