using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class ThrowStatementTest
    {
        private ThrowStatement Parse(string query)
        {
            var p = new SqlParser();
            return (ThrowStatement)p.Execute(new ThrowStatement(), query);
        }

        [TestMethod]
        public void SimpleThrowTest()
        {
            var sql = "THROW";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void FullThrowTest()
        {
            var sql = "THROW 123, 'message', 123";
            var sb = Parse(sql);

            sql = "THROW 123 , 'message' , 123";
            sb = Parse(sql);

            sql = "THROW 123,'message',123";
            sb = Parse(sql);
        }

        [TestMethod]
        public void StatementAfterThrowTest()
        {
            var sql = @"THROW 123, 'message', 123 PRINT ''";
            new SqlParser().Execute<StatementBlock>(sql);
        }
    }
}
