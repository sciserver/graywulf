using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class WindowedFunctionCallTest
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

        [TestMethod]
        public void RankingFunctionCallNoArgumentTest()
        {
            var sql = "ROW_NUMBER ( ) OVER ( )";
            new SqlParser().Execute<WindowedFunctionCall>(sql);

            sql = "ROW_NUMBER()OVER()";
            new SqlParser().Execute<WindowedFunctionCall>(sql);
        }

        [TestMethod]
        public void RankingFunctionWithArgumentsTest()
        {
            var sql = "NTILE ( 8 ) OVER ( )";
            new SqlParser().Execute<WindowedFunctionCall>(sql);

            sql = "NTILE(8)OVER()";
            new SqlParser().Execute<WindowedFunctionCall>(sql);
        }

        [TestMethod]
        public void PartitionByTest()
        {
            var sql = "ROW_NUMBER () OVER ( PARTITION BY ID )";
            new SqlParser().Execute<WindowedFunctionCall>(sql);

            sql = "ROW_NUMBER () OVER ( PARTITION BY ID )";
            new SqlParser().Execute<WindowedFunctionCall>(sql);

            sql = "ROW_NUMBER () OVER (PARTITION BY[ID])";
            new SqlParser().Execute<WindowedFunctionCall>(sql);
        }

        [TestMethod]
        public void OrderByTest()
        {
            var sql = "ROW_NUMBER () OVER ( ORDER BY ID )";
            new SqlParser().Execute<WindowedFunctionCall>(sql);

            sql = "ROW_NUMBER () OVER ( ORDER BY ID, ID2 )";
            new SqlParser().Execute<WindowedFunctionCall>(sql);

            sql = "ROW_NUMBER () OVER (ORDER BY[ID],[ID2])";
            new SqlParser().Execute<WindowedFunctionCall>(sql);

            sql = "ROW_NUMBER () OVER ( ORDER BY ID ASC, ID2 DESC)";
            new SqlParser().Execute<WindowedFunctionCall>(sql);
        }

        [TestMethod]
        public void PartitionByOrderByTest()
        {
            var sql = "ROW_NUMBER () OVER ( PARTITION BY ID1 ORDER BY ID2, ID3 )";
            new SqlParser().Execute<WindowedFunctionCall>(sql);
        }

        [TestMethod]
        public void FullSelectTest()
        {
            var sql =
@"WITH q AS
(
    SELECT ID, ROW_NUMBER() OVER (ORDER BY ID ASC)
)
SELECT * FROM q WHERE rn BETWEEN 100 AND 200
ORDER BY ID DESC";
            new SqlParser().Execute<SelectStatement>(sql);
        }
    }
}
