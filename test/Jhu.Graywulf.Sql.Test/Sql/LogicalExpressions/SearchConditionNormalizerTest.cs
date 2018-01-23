using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
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

        private List<WhereClause> GenerateWhereClauseByTable(string sql)
        {
            var details = Parse(sql);
            var res = new List<WhereClause>();

            var scn = new SearchConditionNormalizer();
            scn.CollectConditions(details.ParsingTree);

            foreach (var key in details.SourceTables.Keys)
            {
                res.Add(scn.GenerateWhereClauseSpecificToTable(details.SourceTables[key][0].DatabaseObject));
            }

            return res;
        }

        private List<WhereClause> GenerateWhereClauseByTableReference(string sql)
        {
            var details = Parse(sql);
            var res = new List<WhereClause>();

            var scn = new SearchConditionNormalizer();
            scn.CollectConditions(details.ParsingTree);

            foreach (var key in details.SourceTables.Keys)
            {
                foreach (var tr in details.SourceTables[key])
                {
                    res.Add(scn.GenerateWhereClauseSpecificToTable(tr));
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
            Assert.AreEqual("WHERE Name = 'test'", w[0].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("WHERE Name = 'test'", w[0].Value);
        }

        [TestMethod]
        public void SubqueryWherePredicateTest1()
        {
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
            Assert.AreEqual("WHERE Name = 'test'", w[0].Value);

            w = GenerateWhereClauseByTable(sql);

            Assert.AreEqual(1, w.Count);
            Assert.AreEqual("WHERE Name = 'test'", w[0].Value);
        }
    }
}
