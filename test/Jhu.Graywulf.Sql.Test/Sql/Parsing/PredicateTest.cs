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
    public class PredicateTest : ParsingTestBase
    {
        private Predicate Parse(string query)
        {
            var p = new SqlParser();
            return p.Execute<Predicate>(query);
        }

        [TestMethod]
        public void ComparisonOperatorTest()
        {
            var sql = "a = b";
            var sb = Parse(sql);

            sql = "a=b";
            sb = Parse(sql);

            sql = "a>b";
            sb = Parse(sql);

            sql = "a<b";
            sb = Parse(sql);

            sql = "a<=b";
            sb = Parse(sql);

            sql = "a>=b";
            sb = Parse(sql);

            sql = "a<>b";
            sb = Parse(sql);

            sql = "a!<b";
            sb = Parse(sql);

            sql = "a!>b";
            sb = Parse(sql);

            sql = "a!=b";
            sb = Parse(sql);
        }

        [TestMethod]
        public void IsNotNullTest()
        {
            var sql = "a IS NULL";
            var sb = Parse(sql);

            sql = "(a)IS NULL";
            sb = Parse(sql);

            sql = "a IS NOT NULL";
            sb = Parse(sql);

            sql = "(a)IS NOT NULL";
            sb = Parse(sql);
        }

        [TestMethod]
        public void LikeTest()
        {
            var sql = "a LIKE 'alma'";
            var sb = Parse(sql);

            sql = "(a)LIKE'alma'";
            sb = Parse(sql);

            sql = "a + b LIKE c + d";
            sb = Parse(sql);

            sql = "a + b LIKE c + d ESCAPE 'x'";
            sb = Parse(sql);

            sql = "'a + b'LIKE'c + d'ESCAPE'x'";
            sb = Parse(sql);

            sql = "a NOT LIKE 'alma'";
            sb = Parse(sql);

            sql = "(a)NOT LIKE'alma'";
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
        }

        [TestMethod]
        public void AllSomeAnyComparisonTest()
        {
            var sql = "a = ALL (SELECT ID FROM test)";
            var sb = Parse(sql);

            sql = "a=ALL(SELECT ID FROM test)";
            sb = Parse(sql);
        
            sql = "a = SOME (SELECT ID FROM test)";
            sb = Parse(sql);

            sql = "a=SOME(SELECT ID FROM test)";
            sb = Parse(sql);

            sql = "a = ANY (SELECT ID FROM test)";
            sb = Parse(sql);

            sql = "a=ANY(SELECT ID FROM test)";
            sb = Parse(sql);
        }

        [TestMethod]
        public void ExistsTest()
        {
            var sql = "EXISTS (SELECT ID FROM test)";
            var sb = Parse(sql);

            sql = "EXISTS(SELECT ID FROM test)";
            sb = Parse(sql);
        }

        [TestMethod]
        public void InListTest()
        {
            var sql = "a IN (1)";
            var sb = Parse(sql);

            sql = "(a)IN(1)";
            sb = Parse(sql);

            sql = "(a)IN( 1 )";
            sb = Parse(sql);

            sql = "a IN (1, 2, 3)";
            sb = Parse(sql);

            sql = "(a)IN(1,2,3)";
            sb = Parse(sql);

            sql = "( a )IN( 1 , 2 , 3 )";
            sb = Parse(sql);
        }

        [TestMethod]
        public void InQueryTest()
        {
            var sql = "a IN (SELECT ID FROM test)";
            var sb = Parse(sql);

            sql = "(a)IN(SELECT ID FROM test)";
            sb = Parse(sql);
        }
    }
}
