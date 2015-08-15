using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Jobs.Query
{
    [TestClass]
    public class SqlQueryCodeGeneratorTest : TestClassBase
    {
        protected SqlParser.SqlParser Parser
        {
            get { return new SqlParser.SqlParser(); }
        }

        protected SelectStatement Parse(string sql)
        {
            return (SelectStatement)Parser.Execute(sql);
        }

        protected void RemoveExtraTokensHelper(string sql, string gt)
        {
            var ss = Parse(sql);
            var cg = new SqlQueryCodeGenerator();
            CallMethod(cg, "RemoveExtraTokens", ss);
            Assert.AreEqual(gt, ss.ToString());
        }

        protected void RewriteQueryHelper(string sql, string gt, bool partitioningKeyFrom, bool partitioningKeyTo)
        {
            var ss = Parse(sql);
            var partition = new SqlQueryPartition()
            {
                PartitioningKeyFrom = partitioningKeyFrom ? (IComparable)(1.0) : null,
                PartitioningKeyTo = partitioningKeyTo ? (IComparable)(1.0) : null
            };
            var cg = new SqlQueryCodeGenerator(partition);
            CallMethod(cg, "RewriteForExecute", ss);
            CallMethod(cg, "RemoveExtraTokens", ss);
            Assert.AreEqual(gt, ss.ToString());
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void LeaveIntact()
        {
            var sql = "SELECT * FROM Table";
            var gt = "SELECT * FROM Table";

            RemoveExtraTokensHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveOrderByClause()
        {
            var sql = "SELECT * FROM Table ORDER BY column";
            var gt = "SELECT * FROM Table ";

            RemoveExtraTokensHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveOrderByClauseFromUnion()
        {
            var sql = "SELECT * FROM Table1 UNION SELECT * FROM Table2 ORDER BY column";
            var gt = "SELECT * FROM Table1 UNION SELECT * FROM Table2 ";

            RemoveExtraTokensHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveIntoClause()
        {
            var sql = "SELECT * INTO table1 FROM Table2";
            var gt = "SELECT *  FROM Table2";

            RemoveExtraTokensHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveTablePartitionClause()
        {
            var sql = "SELECT * FROM Table2 PARTITION BY id";
            var gt = "SELECT * FROM Table2 ";

            RemoveExtraTokensHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveAllExtraTokens()
        {
            var sql = "SELECT * FROM Table PARTITION BY id ORDER BY column";
            var gt = "SELECT * FROM Table  ";

            RemoveExtraTokensHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningFrom()
        {
            var sql = "SELECT * FROM Table PARTITION BY id";
            var gt = "SELECT * FROM Table  WHERE @keyFrom <= id";

            RewriteQueryHelper(sql, gt, true, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningFromWithWhere()
        {
            var sql = "SELECT * FROM Table PARTITION BY id WHERE x < 5";
            var gt = "SELECT * FROM Table  WHERE (@keyFrom <= id) AND (x < 5)";

            RewriteQueryHelper(sql, gt, true, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningTo()
        {
            var sql = "SELECT * FROM Table PARTITION BY id";
            var gt = "SELECT * FROM Table  WHERE id < @keyTo";

            RewriteQueryHelper(sql, gt, false, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBoth()
        {
            var sql = "SELECT * FROM Table PARTITION BY id";
            var gt = "SELECT * FROM Table  WHERE @keyFrom <= id AND id < @keyTo";

            RewriteQueryHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithWhere()
        {
            var sql = "SELECT * FROM Table PARTITION BY id WHERE x < 5";
            var gt = "SELECT * FROM Table  WHERE (@keyFrom <= id AND id < @keyTo) AND (x < 5)";

            RewriteQueryHelper(sql, gt, true, true);
        }
    }
}
