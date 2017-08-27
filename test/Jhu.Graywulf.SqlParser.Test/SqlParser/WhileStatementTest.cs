using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class WhileStatementTest
    {
        private WhileStatement Parse(string query)
        {
            var p = new SqlParser();
            return (WhileStatement)p.Execute(new WhileStatement(), query);
        }

        [TestMethod]
        public void SimpleWhileTest()
        {
            var sql = "WHILE 1 = 2 SELECT 1";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void SimpleWhileWithBracketsTest()
        {
            var sql = "WHILE 1 = 2 BREAK";
            var sb = Parse(sql);

            sql = "WHILE(1 = 2)BREAK";
            sb = Parse(sql);

            sql = "WHILE (1 = 2)BREAK";
            sb = Parse(sql);

            sql = "WHILE(1 = 2) BREAK";
            sb = Parse(sql);
        }

        [TestMethod]
        public void WhileWithBeginEndTest()
        {
            var sql = 
@"WHILE 1 = 2 
BEGIN
    SELECT 1
END";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void NestedWhileWithBeginEndTest()
        {
            var sql =
@"WHILE 1 = 2 
BEGIN
    DECLARE @a int
    WHILE 1 = 2
    BEGIN
        SELECT 1
        -- this is a comment
    END
END";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void WhileWithBreakAndContinue()
        {
            var sql =
@"WHILE 1 = 2 
BEGIN
    DECLARE @a int
    WHILE 1 = 2
        CONTINUE
    WHILE 1 = 2
        BREAK
    WHILE 1 = 2
        RETURN
END";
            var sb = Parse(sql);
        }

    }
}
