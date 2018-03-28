using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    [TestClass]
    public class SearchConditionNormalizerTest
    {
        protected SchemaManager CreateSchemaManager()
        {
            var sm = new SqlServerSchemaManager();
            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);

            sm.Datasets[ds.Name] = ds;

            return sm;
        }

        protected QueryDetails Parse(string query)
        {
            var p = new SqlParser();
            var script = p.Execute<StatementBlock>(query);

            SqlNameResolver nr = new SqlNameResolver();
            nr.DefaultTableDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName;
            nr.DefaultFunctionDatasetName = Jhu.Graywulf.Test.Constants.CodeDatasetName;
            nr.SchemaManager = CreateSchemaManager();
            var details = nr.Execute(script);

            return details;
        }

        private List<BooleanExpression> GenerateWhereClauseByTable(string sql)
        {
            var details = Parse(sql);
            var res = new List<BooleanExpression>();

            var scn = new SearchConditionNormalizer();
            scn.CollectConditions(details.ParsingTree);

            foreach (var key in details.SourceTableReferences.Keys)
            {
                var trs = details.SourceTableReferences[key];
                var where = scn.GenerateWherePredicatesSpecificToTable(trs);
                res.Add(where);
            }

            return res;
        }

        private List<BooleanExpression> GenerateWhereClauseByTableReference(string sql)
        {
            var details = Parse(sql);
            var res = new List<BooleanExpression>();

            var scn = new SearchConditionNormalizer();
            scn.CollectConditions(details.ParsingTree);

            foreach (var key in details.SourceTableReferences.Keys)
            {
                foreach (var tr in details.SourceTableReferences[key])
                {
                    res.Add(scn.GenerateWherePredicatesSpecificToTable(tr));
                }
            }

            return res;
        }
        
        [TestMethod]
        public void SimpleWherePredicateTest()
        {
            var sql = "SELECT * FROM Author WHERE Name = 'test'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Name = 'test')", w[0].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Name = 'test')", w[0].Value);
        }

        [TestMethod]
        public void SubqueryWherePredicateTest1()
        {
            // Where clauses are always taken from the query which directly references the table.
            // Outer queries referencing the table via a subquery don't count.

            var sql = "SELECT * FROM (SELECT * FROM Author) a WHERE Name = 'test'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);
        }

        [TestMethod]
        public void SubqueryWherePredicateTest2()
        {
            var sql = "SELECT * FROM (SELECT * FROM Author WHERE Name = 'test') a";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Name = 'test')", w[0].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Name = 'test')", w[0].Value);
        }

        [TestMethod]
        public void SubqueryWherePredicateTest3()
        {
            var sql =
@"SELECT * 
FROM (SELECT * FROM Author WHERE Name = 'test') a
CROSS JOIN (SELECT * FROM Author WHERE Name = 'test2') b";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual("(Name = 'test')", w[0].Value);
            Assert.AreEqual("(Name = 'test2')", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Name = 'test2') OR (Name = 'test')", w[0].Value);
        }

        [TestMethod]
        public void MultipleSelectWherePredicateTest()
        {
            var sql =
@"SELECT * FROM Author WHERE Name = 'test1' OR Name = 'test3'
SELECT * FROM Author WHERE Name = 'test2' AND Name = 'test4'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual("(Name = 'test1' OR Name = 'test3')", w[0].Value);
            Assert.AreEqual("(Name = 'test2' AND Name = 'test4')", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Name = 'test2' AND Name = 'test4') OR (Name = 'test1' OR Name = 'test3')", w[0].Value);
        }

        [TestMethod]
        public void UnionWherePredicateTest()
        {
            var sql =
@"SELECT * FROM Author WHERE Name = 'test1' OR Name = 'test3'
UNION
SELECT * FROM Author WHERE Name = 'test2' AND Name = 'test4'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual("(Name = 'test1' OR Name = 'test3')", w[0].Value);
            Assert.AreEqual("(Name = 'test2' AND Name = 'test4')", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Name = 'test2' AND Name = 'test4') OR (Name = 'test1' OR Name = 'test3')", w[0].Value);
        }

        [TestMethod]
        public void CteWherePredicateTest1()
        {
            var sql =
@"
WITH a AS
(SELECT * FROM Author WHERE Name = 'test1' OR Name = 'test3'),
b AS
(SELECT * FROM Author WHERE Name = 'test2' AND Name = 'test4')
SELECT * FROM a CROSS JOIN b";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual("(Name = 'test1' OR Name = 'test3')", w[0].Value);
            Assert.AreEqual("(Name = 'test2' AND Name = 'test4')", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(Name = 'test2' AND Name = 'test4') OR (Name = 'test1' OR Name = 'test3')", w[0].Value);
        }

        [TestMethod]
        public void CteWherePredicateTest2()
        {
            var sql =
@"
WITH a AS
(SELECT * FROM Author WHERE Name = 'test1' OR Name = 'test3'),
b AS
(SELECT * FROM Author WHERE Name = 'test2' AND Name = 'test4')
SELECT * FROM a CROSS JOIN b CROSS JOIN Author c
WHERE c.Name = 'test5'";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(3, w.Count);
            Assert.AreEqual("(Name = 'test1' OR Name = 'test3')", w[0].Value);
            Assert.AreEqual("(Name = 'test2' AND Name = 'test4')", w[1].Value);
            Assert.AreEqual("(c.Name = 'test5')", w[2].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("(c.Name = 'test5') OR (Name = 'test2' AND Name = 'test4') OR (Name = 'test1' OR Name = 'test3')", w[0].Value);
        }

        [TestMethod]
        public void NoPredicateTest1()
        {
            var sql = "SELECT name FROM Author";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);
        }

        [TestMethod]
        public void NoPredicateTest2()
        {
            var sql = 
@"SELECT Title FROM Book
SELECT Title FROM Book WHERE Year > 2000";

            var w = GenerateWhereClauseByTableReference(sql);

            Assert.AreEqual(2, w.Count);
            Assert.AreEqual(null, w[0]);
            Assert.AreEqual("(Year > 2000)", w[1].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual(null, w[0]);
        }
    }
}
