using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class IifFunctionCallTest
    {
        [TestMethod]
        public void IifTest()
        {
            var sql = "IIF(a < b, a + b)";
            var exp = new SqlParser().Execute<Expression>(sql);

            sql = "IIF(a < b OR c > d, a + b * c, d - e / f)";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "IIF ( a < b OR c > d , a + b * c , d - e / f )";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "IIF(a < b OR c > d,a + b * c,d - e / f)";
            exp = new SqlParser().Execute<Expression>(sql);
        }


    }
}
