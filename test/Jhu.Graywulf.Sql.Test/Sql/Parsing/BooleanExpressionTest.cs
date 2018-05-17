using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class BooleanExpressionTest
    {
        private BooleanExpression Parse(string query)
        {
            var p = new SqlParser();
            return p.Execute<BooleanExpression>(query);
        }

        [TestMethod]
        public void BooleanOperatorsTest()
        {
            var sql = "ID=6 AND Data=10";
            var sb = Parse(sql);

            sql = "(ID=6)AND(Data=10)";
            sb = Parse(sql);

            sql = "(ID=6)OR(Data=10)";
            sb = Parse(sql);

            sql = "(ID=6)OR(Data=10)";
            sb = Parse(sql);
        }

        [TestMethod]
        public void BooleanOperatorsNotTest()
        {
            var sql = "ID=6 AND NOT Data=10";
            var sb = Parse(sql);

            sql = "(ID=6)AND(NOT(Data=10))";
            sb = Parse(sql);

            sql = "NOT (ID=6)OR(Data=10)";
            sb = Parse(sql);
        }

        [TestMethod]
        public void BetweenTest()
        {
            var sql = "ID BETWEEN 6 AND 10";
            var sb = Parse(sql);

            sql = "(ID)BETWEEN(6)AND(10)";
            sb = Parse(sql);

            sql = "ID NOT BETWEEN 6 AND 10";
            sb = Parse(sql);

            sql = "(ID)NOT BETWEEN(6)AND(10)";
            sb = Parse(sql);

            sql = "ID BETWEEN 6 AND 10 AND Title = ''";
            sb = Parse(sql);

            sql = "(ID)BETWEEN(6)AND(10)AND(Title='')";
            sb = Parse(sql);

            sql = "ID NOT BETWEEN 6 AND 10 AND Title = ''";
            sb = Parse(sql);

            sql = "(ID)NOT BETWEEN(6)AND(10)AND(Title='')";
            sb = Parse(sql);
        }
    }
}
