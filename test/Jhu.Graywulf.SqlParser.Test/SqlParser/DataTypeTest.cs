using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.SqlParser
{
    [TestClass]
    public class DataTypeTest
    {
        private DataType Parse(string query)
        {
            var p = new SqlParser();
            return (DataType)p.Execute(new DataType(), query);
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
            var sql = "nvarchar(10)";
            var sb = Parse(sql);

            sql = "nvarchar (10)";
            sb = Parse(sql);

            sql = "nvarchar ( 10 )";
            sb = Parse(sql);
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
    }
}
