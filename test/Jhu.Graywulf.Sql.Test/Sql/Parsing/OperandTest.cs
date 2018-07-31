using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class OperandTest
    {
        [TestMethod]
        public void SpecialFunctionCallTypesTest()
        {
            var sql = "AVG(a)";
            var exp = new SqlParser().Execute<Expression>(sql);
            var fc = exp.FindDescendant<Operand>().Stack.First;
            Assert.IsInstanceOfType(fc, typeof(SystemFunctionCall));

            sql = "AVG(DISTINCT a)";
            exp = new SqlParser().Execute<Expression>(sql);
            fc = exp.FindDescendant<Operand>().Stack.First;
            Assert.IsInstanceOfType(fc, typeof(AggregateFunctionCall));

            sql = "AVG(ALL a)";
            exp = new SqlParser().Execute<Expression>(sql);
            fc = exp.FindDescendant<Operand>().Stack.First;
            Assert.IsInstanceOfType(fc, typeof(AggregateFunctionCall));

            sql = "AVG(ALL a) OVER (PARTITION BY b)";
            exp = new SqlParser().Execute<Expression>(sql);
            fc = exp.FindDescendant<Operand>().Stack.First;
            Assert.IsInstanceOfType(fc, typeof(AggregateFunctionCall));

            sql = "AVG(a) OVER (PARTITION BY b)";
            exp = new SqlParser().Execute<Expression>(sql);
            fc = exp.FindDescendant<Operand>().Stack.First;
            Assert.IsInstanceOfType(fc, typeof(WindowedFunctionCall));

            sql = "COUNT(*)";
            exp = new SqlParser().Execute<Expression>(sql);
            fc = exp.FindDescendant<Operand>().Stack.First;
            Assert.IsInstanceOfType(fc, typeof(StarFunctionCall));

            sql = "CONVERT(float,a)";
            exp = new SqlParser().Execute<Expression>(sql);
            Assert.IsInstanceOfType(exp.FindDescendant<Operand>().Stack.First, typeof(ConvertFunctionCall));

            sql = "PARSE(a AS float)";
            exp = new SqlParser().Execute<Expression>(sql);
            Assert.IsInstanceOfType(exp.FindDescendant<Operand>().Stack.First, typeof(CastAndParseFunctionCall));

            sql = "DATEADD(year, a, 2)";
            exp = new SqlParser().Execute<Expression>(sql);
            Assert.IsInstanceOfType(exp.FindDescendant<Operand>().Stack.First, typeof(DateFunctionCall));

            sql = "IIF(a > b, c, d)";
            exp = new SqlParser().Execute<Expression>(sql);
            Assert.IsInstanceOfType(exp.FindDescendant<Operand>().Stack.First, typeof(IifFunctionCall));
        }
    }
}
