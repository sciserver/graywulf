﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class TableVariableTest
    {
        [TestMethod]
        public void SelectFromVariableTest()
        {
            var sql = @"SELECT * FROM @test";
            new SqlParser().Execute<SelectStatement>(sql);

            sql = @"SELECT * FROM @test t";
            new SqlParser().Execute<SelectStatement>(sql);

            sql = @"SELECT * FROM @test AS t";
            new SqlParser().Execute<SelectStatement>(sql);
        }
    }
}
