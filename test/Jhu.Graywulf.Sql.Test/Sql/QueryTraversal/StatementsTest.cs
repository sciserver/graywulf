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

            var gt = "FROM Tab1 INNER LOOP JOIN Tab2 ON t . ID = t2 . ID WHERE ID IN ( 1 , 2 , 4 ) GROUP BY ID , Data HAVING AVG ( Data2 ) > 10 SELECT TOP 10 PERCENT ID AS Col1 , Col2 = Data , AVG ( Data2 ) INTO outtable UNION ALL FROM Tab2 SELECT DISTINCT ID , Data ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void SelectAssignVariableTest()
        {
            var sql = @"SELECT @v = ID FROM tab";
            var gt = "FROM tab SELECT @v = ID ";

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

            var gt = "INSERT newtable ( col1 , col2 ) SELECT 1 , 2 ";

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

            var gt = "INSERT newtable ( col1 , col2 ) VALUES ( 1 , 2 ) , ( 3 , 4 ) ";

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

            var gt = "INSERT newtable FROM Tab1 INNER LOOP JOIN Tab2 ON t . ID = t2 . ID WHERE ID IN ( 1 , 2 , 4 ) GROUP BY ID , Data HAVING AVG ( Data2 ) > 10 SELECT TOP 10 PERCENT ID AS Col1 , Col2 = Data , AVG ( Data2 ) UNION ALL FROM Tab2 SELECT DISTINCT ID , Data ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DeleteStatementTest()
        {
            var sql = "DELETE mytable";
            var gt = "DELETE mytable ";

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

            var gt = "FROM Tab1 INNER LOOP JOIN Tab2 ON t . ID = t2 . ID DELETE newtable WHERE ID IN ( 1 , 2 , 4 ) ";

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

            var gt = "UPDATE mytable SET col1 = 1 , col2 = 2 ";

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

            var gt = "FROM mytable INNER JOIN othertable ON a . ID = b . ID WHERE a . Data > 5 UPDATE mytable SET col1 = 1 , col2 = 2 ";

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
            var gt = "IF x < ANY <subquery> BEGIN SET @var = 5 END ELSE BEGIN FROM tab1 SELECT @var = ID END ";

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
    INDEX IX_t ( Data, Data2 DESC )
)";

            var gt = "DECLARE @t AS TABLE ( ID int NOT NULL IDENTITY ( 1 , 1 ) PRIMARY KEY , Data float NULL DEFAULT 0 , Data2 real , INDEX IX_t ( Data , Data2 DESC ) ) ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void CreateTableStatementTest()
        {
            var sql =
@"CREATE TABLE test
(
    ID int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    Data float NULL DEFAULT 0,
    Data2 real CONSTRAINT UQ_test_Data2 UNIQUE,
    INDEX IX_t ( Data, Data2 ASC )
)";

            var gt = "CREATE TABLE test ( ID int NOT NULL IDENTITY ( 1 , 1 ) PRIMARY KEY , Data float NULL DEFAULT 0 , Data2 real CONSTRAINT UQ_test_Data2 UNIQUE , INDEX IX_t ( Data , Data2 ASC ) ) ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DropTableStatementTest()
        {
            var sql = "DROP TABLE tab";
            var gt = "DROP TABLE tab ";
            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void TruncateTableStatementTest()
        {
            var sql = "TRUNCATE TABLE tab";
            var gt = "TRUNCATE TABLE tab ";
            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void CreateIndexStatementTest()
        {
            var sql =
@"CREATE UNIQUE NONCLUSTERED INDEX IX_tab ON tab
(
    ID ASC, Data
)
INCLUDE
(
    Data1, Data2
)
";

            var gt = "CREATE UNIQUE NONCLUSTERED INDEX IX_tab ON tab ( ID ASC , Data ) INCLUDE ( Data1 , Data2 ) ";

            var res = Execute(sql);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void DropIndexStatementTest()
        {
            Assert.Inconclusive();
        }
    }
}
