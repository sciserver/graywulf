using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.SqlParser
{
    [TestClass]
    public class TableVariableTest
    {

        [TestMethod]
        public void DeclareTableVariableTest()
        {
            // Only test simple cases here, table definition
            // is tested thoroughly in CreateTableTest

            var sql = @"DECLARE @test TABLE (ID int)";
            new SqlParser().Execute<DeclareTableStatement>(sql);
        }

        [TestMethod]
        public void SelectFromVariableTest()
        {
            var sql = @"SELECT * FROM @test";
            new SqlParser().Execute<SelectStatement>(sql);
        }
    }
}
