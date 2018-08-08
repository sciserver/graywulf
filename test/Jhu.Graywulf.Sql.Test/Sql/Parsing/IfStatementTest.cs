using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class IfStatementTest
    {
        private IfStatement Parse(string query)
        {
            var p = new SqlParser();
            return p.Execute<IfStatement>(query);
        }

        [TestMethod]
        public void SimpleIfTest()
        {
            var sql = "IF 1 = 2 SELECT 1";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void SimpleIfElseTest()
        {
            var sql = "IF 1 = 2 BREAK ELSE CONTINUE";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void IfWithBracketsTest()
        {
            var sql = "IF 1 = 2 BREAK";
            var sb = Parse(sql);

            sql = "IF(1 = 2)BREAK";
            sb = Parse(sql);

            sql = "IF (1 = 2)BREAK";
            sb = Parse(sql);

            sql = "IF(1 = 2) BREAK";
            sb = Parse(sql);
        }

        [TestMethod]
        public void IfWithElseSeparatorTest()
        {
            var sql = "IF 1 = 2 BREAK ELSE CONTINUE";
            var sb = Parse(sql);

            sql = "IF 1 = 2 BREAK;ELSE CONTINUE";
            sb = Parse(sql);

            sql = "IF 1 = 2 BREAK ;ELSE CONTINUE";
            sb = Parse(sql);

            sql = "IF 1 = 2 BREAK; ELSE CONTINUE";
            sb = Parse(sql);
        }

        [TestMethod]
        public void IfWithBeginEndTest()
        {
            var sql = 
@"IF 1 = 2 
BEGIN
    CONTINUE
END
ELSE BEGIN BREAK END";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void NestedIfTest()
        {
            var sql =
@"IF 1 = 2
    IF 2 = 3
        BREAK
    ELSE
        IF 3 = 4
            CONTINUE
        ELSE
            CONTINUE";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void NestedIfWithBeginEndTest()
        {
            var sql =
@"IF 1 = 2 
BEGIN
    DECLARE @a int
    IF 1 = 2
    BEGIN
        SELECT 1
        -- this is a comment
    END
END";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void StatementAfterIfTest()
        {
            var sql =
@"IF 1 = 2 
BEGIN
    DECLARE @a int
    IF 1 = 2
    BEGIN
        SELECT 1
        -- this is a comment
    END
END

PRINT ''
";
            new SqlParser().Execute<StatementBlock>(sql);
        }
    }
}
