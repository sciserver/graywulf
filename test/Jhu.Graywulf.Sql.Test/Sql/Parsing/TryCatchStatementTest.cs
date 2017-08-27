using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class TryCatchStatementTest
    {
        private TryCatchStatement Parse(string query)
        {
            var p = new SqlParser();
            return (TryCatchStatement)p.Execute(new TryCatchStatement(), query);
        }

        [TestMethod]
        public void SimpleTryCatchTest()
        {
            var sql = 
@"BEGIN TRY
    SELECT 1
END TRY
BEGIN CATCH
    SELECT 2
END CATCH";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void EmptyCatchTest()
        {
            var sql =
@"BEGIN TRY
    SELECT 1
END TRY
BEGIN CATCH END CATCH";
            var sb = Parse(sql);
        }

        [TestMethod]
        public void SemicolonsTest()
        {
            // TODO: This isn't actually correct but StatementBlock allows
            // empty statements and we simply allow a statement block between
            // the try and catch blocks

            var sql =
@"BEGIN TRY
    ;
END TRY
BEGIN CATCH 
    ;
END CATCH";
            var sb = Parse(sql);
        }
    }
}
