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
        public void NullTest()
        {
            var dt = Parse("int NULL");
            Assert.AreEqual(SqlDbType.Int, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsNullable);

            dt = Parse("[int]NULL");
            Assert.AreEqual(SqlDbType.Int, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsNullable);
        }

        [TestMethod]
        public void NotNullTest()
        {
            var dt = Parse("int NOT NULL");
            Assert.AreEqual(SqlDbType.Int, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsFalse(dt.DataTypeReference.DataType.IsNullable);

            dt = Parse("[int]NOT NULL");
            Assert.AreEqual(SqlDbType.Int, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsFalse(dt.DataTypeReference.DataType.IsNullable);
        }

        [TestMethod]
        public void AllSystemTypesTest()
        {
            var dt = Parse("tinyint");
            Assert.AreEqual(SqlDbType.TinyInt, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("smallint");
            Assert.AreEqual(SqlDbType.SmallInt, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("int");
            Assert.AreEqual(SqlDbType.Int, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("bigint");
            Assert.AreEqual(SqlDbType.BigInt, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("decimal");
            Assert.AreEqual(SqlDbType.Decimal, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(18, dt.DataTypeReference.DataType.Precision);
            Assert.AreEqual(0, dt.DataTypeReference.DataType.Scale);

            dt = Parse("decimal(5)");
            Assert.AreEqual(SqlDbType.Decimal, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(5, dt.DataTypeReference.DataType.Precision);
            Assert.AreEqual(0, dt.DataTypeReference.DataType.Scale);

            dt = Parse("decimal(5, 3)");
            Assert.AreEqual(SqlDbType.Decimal, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(5, dt.DataTypeReference.DataType.Precision);
            Assert.AreEqual(3, dt.DataTypeReference.DataType.Scale);

            dt = Parse("smallmoney");
            Assert.AreEqual(SqlDbType.SmallMoney, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("money");
            Assert.AreEqual(SqlDbType.Money, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("numeric");
            Assert.AreEqual(SqlDbType.Decimal, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("real");
            Assert.AreEqual(SqlDbType.Real, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("float");
            Assert.AreEqual(SqlDbType.Float, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("date");
            Assert.AreEqual(SqlDbType.Date, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("time");
            Assert.AreEqual(SqlDbType.Time, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("smalldatetime");
            Assert.AreEqual(SqlDbType.SmallDateTime, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("datetime");
            Assert.AreEqual(SqlDbType.DateTime, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("datetime2");
            Assert.AreEqual(SqlDbType.DateTime2, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("datetimeoffset");
            Assert.AreEqual(SqlDbType.DateTimeOffset, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("char(10)");
            Assert.AreEqual(SqlDbType.Char, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("varchar(10)");
            Assert.AreEqual(SqlDbType.VarChar, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("varchar(max)");
            Assert.AreEqual(SqlDbType.VarChar, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("text");
            Assert.AreEqual(SqlDbType.Text, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("nvarchar(10)");
            Assert.AreEqual(SqlDbType.NVarChar, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("nvarchar(max)");
            Assert.AreEqual(SqlDbType.NVarChar, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("ntext");
            Assert.AreEqual(SqlDbType.NText, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            //dt = Parse("xml");
            //Assert.AreEqual(SqlDbType.Xml, dt.DataTypeReference.DataType.SqlDbType);
            //Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("binary(10)");
            Assert.AreEqual(SqlDbType.Binary, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("varbinary(10)");
            Assert.AreEqual(SqlDbType.VarBinary, dt.DataTypeReference.DataType.SqlDbType);
            Assert.AreEqual(10, dt.DataTypeReference.DataType.Length);

            dt = Parse("varbinary(max)");
            Assert.AreEqual(SqlDbType.VarBinary, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            dt = Parse("image");
            Assert.AreEqual(SqlDbType.Image, dt.DataTypeReference.DataType.SqlDbType);
            Assert.IsTrue(dt.DataTypeReference.DataType.IsMaxLength);

            //dt = Parse("variant");
            //Assert.AreEqual(SqlDbType.Variant, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("timestamp");
            Assert.AreEqual(SqlDbType.Timestamp, dt.DataTypeReference.DataType.SqlDbType);

            dt = Parse("uniqueidentifier");
            Assert.AreEqual(SqlDbType.UniqueIdentifier, dt.DataTypeReference.DataType.SqlDbType);
        }
    }
}
