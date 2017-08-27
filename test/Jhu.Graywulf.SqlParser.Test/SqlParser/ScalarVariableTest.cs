using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ScalarVariableTest
    {

        [TestMethod]
        public void DeclareSingleVariableTest()
        {
            var sql = @"DECLARE @test int";
            new SqlParser().Execute<DeclareVariableStatement>(sql);

            sql = @"DECLARE @test nvarchar(20)";
            new SqlParser().Execute<DeclareVariableStatement>(sql);

            sql = @"DECLARE @test int = 83";
            new SqlParser().Execute<DeclareVariableStatement>(sql);

            sql = @"DECLARE @test nvarchar(max) = 'value'";
            new SqlParser().Execute<DeclareVariableStatement>(sql);
        }

        [TestMethod]
        public void DeclareMultipleVariablesTest()
        {
            var sql = @"DECLARE @test1 int, @test2 float";
            new SqlParser().Execute<DeclareVariableStatement>(sql);

            sql = @"DECLARE @test1 int , @test2 float";
            new SqlParser().Execute<DeclareVariableStatement>(sql);

            sql = @"DECLARE @test1 int,@test2 float";
            new SqlParser().Execute<DeclareVariableStatement>(sql);

            sql = @"DECLARE @test1 int = 8, @test2 float = 12.343";
            new SqlParser().Execute<DeclareVariableStatement>(sql);
        }
        
        [TestMethod]
        public void SetVariableExpressionTest()
        {
            var sql = "SET @test = 1";
            new SqlParser().Execute<SetVariableStatement>(sql);

            sql = "SET @test = 1 + 2 + @test";
            new SqlParser().Execute<SetVariableStatement>(sql);
        }

        [TestMethod]
        public void SetVariableQueryTest()
        {
            var sql = "SET @test = (SELECT 1)";
            new SqlParser().Execute<SetVariableStatement>(sql);

            sql = "SET @test = 1 + (SELECT 2) + @test";
            new SqlParser().Execute<SetVariableStatement>(sql);
        }
    }
}
