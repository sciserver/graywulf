using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class GroupByTest
    {
        private Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<Expression>(query);
        }

        // *** TODO: write tests

    }
}
