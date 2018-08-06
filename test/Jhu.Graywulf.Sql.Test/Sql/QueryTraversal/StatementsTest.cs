using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    [TestClass]
    public class StatementsTest
    {
        private string Execute(string code)
        {
            var exp = new SqlParser().Execute<StatementBlock>(code);
            return new TestVisitorSink().Execute(exp);
        }

        [TestMethod]
        public void SelectStatementTest()
        {
            var sql =
@"SELECT TOP 10 PERCENT ID AS Col1, Col2 = Data, AVG(Data2)
INTO outtable
FROM Tab1 AS t WITH(TABLOCKX)
INNER LOOP JOIN Tab2 t2 WITH(TABLOCK) ON t.ID = t2.ID
WHERE ID IN (1, 2, 4)
GROUP BY ID, Data
HAVING AVG(Data2) > 10
UNION ALL
SELECT DISTINCT ID, Data FROM Tab2
";

            var gt = "FROM <table> INNER LOOP JOIN <table> ON t . ID = t2 . ID WHERE ID IN ( 1 , 2 , 4 ) GROUP BY ID , Data HAVING AVG ( Data2 ) > 10 SELECT TOP 10 PERCENT ID AS Col1 , Col2 = Data , AVG ( Data2 ) INTO <table> UNION ALL FROM <table> SELECT DISTINCT ID , Data ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void SelectAssignVariableTest()
        {
            var sql = @"SELECT @v = ID FROM tab";
            var gt = "FROM <table> SELECT @v = ID ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void InsertStatementWithColumnListTest()
        {
            var sql =
@"INSERT newtable
(col1, col2)
SELECT 1, 2
";

            var gt = "INSERT <table> ( col1 , col2 ) SELECT 1 , 2 ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void InsertStatementWithColumnListAndValuesTest()
        {
            var sql =
@"INSERT newtable
(col1, col2)
VALUES
(1, 2), (3, 4)
";

            var gt = "INSERT <table> ( col1 , col2 ) VALUES ( 1 , 2 ) , ( 3 , 4 ) ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void InsertStatementWithSelectTest()
        {
            var sql =
@"INSERT newtable
SELECT TOP 10 PERCENT ID AS Col1, Col2 = Data, AVG(Data2)
FROM Tab1 AS t WITH(TABLOCKX)
INNER LOOP JOIN Tab2 t2 WITH(TABLOCK) ON t.ID = t2.ID
WHERE ID IN (1, 2, 4)
GROUP BY ID, Data
HAVING AVG(Data2) > 10
UNION ALL
SELECT DISTINCT ID, Data FROM Tab2
";

            var gt = "INSERT <table> FROM <table> INNER LOOP JOIN <table> ON t . ID = t2 . ID WHERE ID IN ( 1 , 2 , 4 ) GROUP BY ID , Data HAVING AVG ( Data2 ) > 10 SELECT TOP 10 PERCENT ID AS Col1 , Col2 = Data , AVG ( Data2 ) UNION ALL FROM <table> SELECT DISTINCT ID , Data ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DeleteStatementTest()
        {
            var sql = "DELETE mytable";
            var gt = "DELETE <table> ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DeleteStatementWithQueryTest()
        {
            var sql =
@"DELETE newtable
FROM Tab1 AS t WITH(TABLOCKX)
INNER LOOP JOIN Tab2 t2 WITH(TABLOCK) ON t.ID = t2.ID
WHERE ID IN (1, 2, 4)";

            var gt = "FROM <table> INNER LOOP JOIN <table> ON t . ID = t2 . ID DELETE <table> WHERE ID IN ( 1 , 2 , 4 ) ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void UpdateStatementTest()
        {
            var sql = 
@"UPDATE mytable
SET col1 = 1,
    col2 = 2";

            var gt = "UPDATE <table> SET col1 = 1 , col2 = 2 ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void UpdateStatementWithQueryTest()
        {
            var sql =
@"UPDATE mytable
SET col1 = 1,
    col2 = 2
FROM mytable a INNER JOIN othertable b ON a.ID = b.ID
WHERE a.Data > 5";

            var gt = "FROM <table> INNER JOIN <table> ON a . ID = b . ID WHERE a . Data > 5 UPDATE <table> SET col1 = 1 , col2 = 2 ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void LabelTest()
        {
            var sql = @"mylabel:";

            var gt = @"mylabel : ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void LabelAndGotoTest()
        {
            var sql =
@"mylabel:
GOTO mylabel";

            var gt =
@"mylabel : GOTO mylabel ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void BeginEndTest()
        {
            var sql =
@"BEGIN
-- this is just a comment
PRINT 'hello world'
END";

            var gt = "BEGIN PRINT 'hello world' END ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }
        [TestMethod]
        public void WhileStatementTest()
        {
            var sql =
@"WHILE 1 = 2
BEGIN
    PRINT 'never'
END
";
            var gt = "WHILE 1 = 2 BEGIN PRINT 'never' END ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }


        [TestMethod]
        public void IfStatementTest()
        {
            var sql =
@"IF 1 = 2
BEGIN
    PRINT 'true'
END ELSE BEGIN
    PRINT 'false'
END
";
            var gt = "IF 1 = 2 BEGIN PRINT 'true' END ELSE BEGIN PRINT 'false' END ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void IfStatementWithEverythingTest()
        {
            var sql =
@"IF x < ANY (SELECT TOP 10 ID FROM tab1)
BEGIN
    SET @var = 5
END ELSE BEGIN
    SELECT @var = ID FROM tab1
END
";
            var gt = "IF x < ANY <subquery> BEGIN SET @var = 5 END ELSE BEGIN FROM <table> SELECT @var = ID END ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void TryCatchStatementTest()
        {
            var sql =
@"BEGIN TRY
    THROW 51000, 'error', 1;
END TRY
BEGIN CATCH
    PRINT 'error'
END CATCH
";

            var gt = "BEGIN TRY THROW 51000 , 'error' , 1 END TRY BEGIN CATCH PRINT 'error' END CATCH ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void ThrowStatementTest()
        {
            var sql = "THROW 51000, 'error', 1;";

            var gt = "THROW 51000 , 'error' , 1 ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void PrintStatementTest()
        {
            var sql = "PRINT 'hello'";
            var gt = "PRINT 'hello' ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DeclareCursorStatementTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SetCursorStatementTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CursorOperationrStatementTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void FetchStatementTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeclareVariableStatementTest()
        {
            var sql = "DECLARE @v int = 1, @w float = (SELECT TOP 1 Data FROM tab)";
            var gt = "DECLARE @v int = 1 , @w float = <subquery> ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void SetVariableStatementTest()
        {
            var sql = "SET @v = 1 + (SELECT TOP 1 Data FROM tab)";
            var gt = "SET @v = 1 + <subquery> ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DeclareTableStatementTest()
        {
            var sql =
@"DECLARE @t AS TABLE
(
    ID int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    Data float NULL DEFAULT 0,
    Data2 real,
    INDEX IX_t ( Data, Data2 )
)";

            var gt = "";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }
    }
}
