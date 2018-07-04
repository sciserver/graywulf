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
    public class DeclareTableTest
    {
        private DeclareTableStatement Parse(string query)
        {
            var p = new SqlParser();
            return p.Execute<DeclareTableStatement>(query);
        }

        [TestMethod]
        public void DeclareTableStatementTest()
        {
            // Only test simple cases here, table definition
            // is tested thoroughly in CreateTableTest

            var sql = @"DECLARE @test TABLE (ID int)";
            var exp = new SqlParser().Execute<DeclareTableStatement>(sql);
            Assert.AreEqual("@test", exp.VariableReference.VariableName);

            sql = @"DECLARE@test TABLE(ID int)";
            exp = new SqlParser().Execute<DeclareTableStatement>(sql);
            Assert.AreEqual("@test", exp.VariableReference.VariableName);

            sql = @"DECLARE @test AS TABLE (ID int)";
            exp = new SqlParser().Execute<DeclareTableStatement>(sql);
            Assert.AreEqual("@test", exp.VariableReference.VariableName);

            sql = @"DECLARE@test AS TABLE(ID int)";
            exp = new SqlParser().Execute<DeclareTableStatement>(sql);
        }
    }
}
