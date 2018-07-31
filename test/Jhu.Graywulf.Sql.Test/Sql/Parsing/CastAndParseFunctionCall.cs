using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class CastAndParseFunctionCallTest
    {
        [TestMethod]
        public void ParseCastTest()
        {
            var sql = "CAST([a]AS[float])";
            var exp = new SqlParser().Execute<Expression>(sql);
            Assert.IsInstanceOfType(exp.FindDescendant<Operand>().Stack.First, typeof(CastAndParseFunctionCall));

            sql = "TRY_CAST(a AS float)";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CAST ( a AS float )";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CAST(a AS decimal(1))";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CAST(a AS decimal(1, 2))";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CAST(a AS nvarchar(max))";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CAST(a AS dbo.udt)";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "CAST(a + b AS dbo.udt)";
            exp = new SqlParser().Execute<Expression>(sql);
        }

        [TestMethod]
        public void ParseParseTest()
        {
            var sql = "PARSE(''AS[float])";
            var exp = new SqlParser().Execute<Expression>(sql);
            Assert.IsInstanceOfType(exp.FindDescendant<Operand>().Stack.First, typeof(CastAndParseFunctionCall));

            sql = "TRY_PARSE(''AS[float])";
            exp = new SqlParser().Execute<Expression>(sql);
            Assert.IsInstanceOfType(exp.FindDescendant<Operand>().Stack.First, typeof(CastAndParseFunctionCall));

            sql = "PARSE ( '' AS float )";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "PARSE(''AS[float]USING'')";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "PARSE ( '' AS float USING '' )";
            exp = new SqlParser().Execute<Expression>(sql);

            sql = "PARSE ( '' + a AS float USING '' )";
            exp = new SqlParser().Execute<Expression>(sql);
        }
    }
}
