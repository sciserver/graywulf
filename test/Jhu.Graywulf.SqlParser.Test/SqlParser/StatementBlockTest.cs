using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.SqlParser
{
    [TestClass]
    public class StatementBlockTest
    {
        private StatementBlock Parse(string query)
        {
            var p = new SqlParser();
            return (StatementBlock)p.Execute(new StatementBlock(), query);
        }

        [TestMethod]
        public void SingleStatementTest()
        {
            var sql = @"
DECLARE @test int
";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void SingleStatementWithSemicolonTest()
        {
            var sql = @"
DECLARE @test int;
";
            var sb = Parse(sql);

            sql = @"
DECLARE @test int ;
";
        }

        [TestMethod]
        public void MultipleStatementTest()
        {
            var sql = @"
DECLARE @test1 int
DECLARE @test2 int

DECLARE @test3 int DECLARE @test4 int
";

            var sb = Parse(sql);
        }

        [TestMethod]
        public void MultipleStatementSemicolonTest()
        {
            var sql = @"
DECLARE @test1 int;
DECLARE @test2 int ;

DECLARE @test3 int ;DECLARE @test4 int
DECLARE @test3 int; DECLARE @test4 int
DECLARE @test3 int;DECLARE @test4 int
";

            var sb = Parse(sql);
        }

        [TestMethod]
        public void EmptyStatementTest()
        {
            var sql = "; ;";
            var sb = Parse(sql);
        }
    }
}
