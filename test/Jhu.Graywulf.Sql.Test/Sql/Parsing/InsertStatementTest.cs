using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class InsertStatementTest
    {
        [TestMethod]
        public void SimpleInsertTest()
        {
            var sql = @"INSERT test VALUES (1, DEFAULT)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT INTO test VALUES (1, DEFAULT)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT[test]VALUES (1, DEFAULT)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT INTO[test]VALUES (1, DEFAULT)";
            new SqlParser().Execute<InsertStatement>(sql);
        }

        [TestMethod]
        public void InsertMultipleValuesTest()
        {
            var sql = @"INSERT test VALUES (1, DEFAULT), (2, DEFAULT)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT INTO test VALUES (1, DEFAULT),(2, DEFAULT)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT[test]VALUES (1, DEFAULT) , (2, NULL),(3, 4)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT INTO[test]VALUES (1, NULL)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT INTO[test]VALUES (1, NULL,(SELECT 2))";
            new SqlParser().Execute<InsertStatement>(sql);
        }

        [TestMethod]
        public void InsertWithColumnsTest()
        {
            var sql = @"INSERT test (a, b) VALUES (1, DEFAULT)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT test ( a , b ) VALUES ( 1, DEFAULT )";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT[test](a,b)VALUES(1,DEFAULT)";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT[test](a,b)VALUES(1,DEFAULT,(SELECT 1))";
            new SqlParser().Execute<InsertStatement>(sql);
        }
        
        [TestMethod]
        public void DefaultValuesTest()
        {
            var sql = @"INSERT test DEFAULT VALUES";
            new SqlParser().Execute<InsertStatement>(sql);
        }

        [TestMethod]
        public void InsertFromSelectTest()
        {
            var sql = 
@"INSERT test
SELECT ID FROM test2";
            new SqlParser().Execute<InsertStatement>(sql);

            sql =
@"INSERT test
SELECT ID FROM test2
CROSS JOIN test3";
            new SqlParser().Execute<InsertStatement>(sql);

            sql =
@"INSERT test
(ID)
SELECT ID, NULL FROM test2
CROSS JOIN test3
UNION SELECT * FROM test4";
            new SqlParser().Execute<InsertStatement>(sql);
        }
        
        [TestMethod]
        public void InsertWithTopExpression2Test()
        {
            var sql =
@"INSERT
INTO test
SELECT TOP 100 ID FROM test2 ORDER BY ID";
            new SqlParser().Execute<InsertStatement>(sql);
        }

        [TestMethod]
        public void InsertWithQueryHintTest()
        {
            var sql =
@"INSERT
INTO test
SELECT TOP 100 ID FROM test2 ORDER BY ID
OPTION (TEST)";
            new SqlParser().Execute<InsertStatement>(sql);
        }

        [TestMethod]
        public void TableHintTest()
        {
            var sql = @"INSERT test WITH(TABLOCKX) SELECT ID FROM test2";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT test WITH ( NOLOCK , TABLOCKX ) SELECT ID FROM test2";
            new SqlParser().Execute<InsertStatement>(sql);

            sql = @"INSERT test WITH(NOLOCK,TABLOCKX)SELECT ID FROM test2";
            new SqlParser().Execute<InsertStatement>(sql);
        }

        [TestMethod]
        public void CteTest()
        {
            var sql = 
@"WITH q AS
(
    SELECT * FROM Table1
)
INSERT test
SELECT * FROM q";

            new SqlParser().Execute<InsertStatement>(sql);
        }
    }
}
