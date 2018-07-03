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

        [TestMethod]
        public void CreateFromPredicateTest()
        {
            var pre = new SqlParser().Execute<Predicate>("a = b");

            var be = BooleanExpression.Create(false, pre);
            Assert.AreEqual("a = b", be.Value);

            be = BooleanExpression.Create(true, pre);
            Assert.AreEqual("NOT a = b", be.Value);
        }

        [TestMethod]
        public void CreateFromBooleanExpressionsTest()
        {
            var be1 = new SqlParser().Execute<BooleanExpression>("a = b");
            var be2 = new SqlParser().Execute<BooleanExpression>("c < d");

            var be = BooleanExpression.Create(be1, be2, LogicalOperator.CreateAnd());
            Assert.AreEqual("a = b AND c < d", be.Value);
        }

        [TestMethod]
        public void CreateFromBooleanExpressionsTest2()
        {
            var be1 = new SqlParser().Execute<BooleanExpressionBrackets>("(a = b)");
            var be2 = new SqlParser().Execute<BooleanExpression>("c < d");

            var be = BooleanExpression.Create(be1, be2, LogicalOperator.CreateAnd());
            Assert.AreEqual("(a = b) AND c < d", be.Value);
        }
    }
}
