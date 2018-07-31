using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class AggregateFunctionCallTest
    {
        [TestMethod]
        public void ParseAllAndDistinctTest()
        {
            var sql = "AVG(ALL a)";
            var exp = new SqlParser().Execute<Expression>(sql);

            sql = "AVG(ALL[a])";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "AVG ( ALL a )";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "AVG(DISTINCT a)";
            exp = new SqlParser().Execute<Expression>(sql);
        }
    }
}
