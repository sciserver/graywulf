using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.SqlParser
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
    }
}
