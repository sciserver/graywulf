using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ConvertFunctionCallTest
    {
        [TestMethod]
        public void ParseConvertTest()
        {
            var sql = "TRY_CONVERT(float,a)";
            var exp = new SqlParser().Execute<Expression>(sql);

            sql = "CONVERT ( float , a )";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CONVERT(decimal(1), a)";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CONVERT(decimal(1, 2), a)";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CONVERT(nvarchar(max), a)";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CONVERT(dbo.udt, a)";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CONVERT(dbo.udt, a + b)";
            exp = new SqlParser().Execute<Expression>(sql);
        }


    }
}
