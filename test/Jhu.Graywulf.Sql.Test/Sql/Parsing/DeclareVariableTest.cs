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
    public class DeclareVariableTest
    {
        private DeclareVariableStatement Parse(string query)
        {
            var p = new SqlParser();
            return p.Execute<DeclareVariableStatement>(query);
        }

        [TestMethod]
        public void SingleVariableTest()
        {
            var sql = "DECLARE@var[int]";
            var exp = Parse(sql);

            sql = "DECLARE @var int";
            exp = Parse(sql);

            sql = "DECLARE @var AS[int]";
            exp = Parse(sql);

            sql = "DECLARE @var AS int";
            exp = Parse(sql);
        }

        [TestMethod]
        public void SingleVariableWithValueTest()
        {
            var sql = "DECLARE@var[int]=2";
            var exp = Parse(sql);

            sql = "DECLARE @var int = 2";
            exp = Parse(sql);
        }

        [TestMethod]
        public void MultipleVariablesTest()
        {
            var sql = "DECLARE@var[int],@var[float]";
            var exp = Parse(sql);

            sql = "DECLARE @var int , @var float";
            exp = Parse(sql);
        }

        [TestMethod]
        public void MultipleVariablesWithValueTest()
        {
            var sql = "DECLARE@var[int]=2,@var[float]=2.2";
            var exp = Parse(sql);

            sql = "DECLARE @var int = 2 , @var float = 2.2";
            exp = Parse(sql);
        }

        [TestMethod]
        public void StatementAfterDeclareVariableTest()
        {
            var sql =
@"DECLARE @var int = 2 , @var float = 2.2
PRINT ''
";
            new SqlParser().Execute<StatementBlock>(sql);
        }
    }
}
