﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class SystemVariableTest : SqlNameResolverTestBase
    {        
        [TestMethod]
        public void SetFromQueryTest()
        {
            var sql = "SELECT @@VERSION";
            var ss = ParseAndResolveNames<SelectStatement>(sql);
        }
    }
}
