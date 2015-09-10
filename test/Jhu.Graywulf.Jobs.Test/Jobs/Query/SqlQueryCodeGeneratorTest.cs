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
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.Query
{
    [TestClass]
    public class SqlQueryCodeGeneratorTest : SqlQueryTestBase
    {
        protected virtual SqlParser.SqlParser Parser
        {
            get { return new SqlParser.SqlParser(); }
        }

        protected virtual SelectStatement Parse(string sql)
        {
            return (SelectStatement)Parser.Execute(sql);
        }

        private SqlQueryCodeGenerator CodeGenerator
        {
            get
            {
                var partition = new SqlQueryPartition()
                {
                    CodeDataset = new Graywulf.Schema.SqlServer.SqlServerDataset()
                    {
                        Name = Jhu.Graywulf.Registry.Constants.CodeDbName,
                        ConnectionString = "data source=localhost;initial catalog=SkyQuery_Code",
                    },
                    TemporaryDataset = new Graywulf.Schema.SqlServer.SqlServerDataset()
                    {
                        Name = Jhu.Graywulf.Registry.Constants.TempDbName,
                        ConnectionString = "data source=localhost;initial catalog=Graywulf_Temp",
                    }
                };

                return new SqlQueryCodeGenerator(partition);
            }
        }

        protected void RemoveExtraTokensHelper(string sql, string gt)
        {
            var ss = Parse(sql);
            CallMethod(CodeGenerator, "RemoveNonStandardTokens", ss);
            Assert.AreEqual(gt, ss.ToString());
        }

        protected void RewriteQueryHelper(string sql, string gt, bool partitioningKeyMin, bool partitioningKeyMax)
        {
            var ss = Parse(sql);
            var partition = new SqlQueryPartition()
            {
                CodeDataset = new Graywulf.Schema.SqlServer.SqlServerDataset()
                {
                    Name = "CODE",
                    ConnectionString = "data source=localhost;initial catalog=SkyQuery_Code",
                },
                TemporaryDataset = new Graywulf.Schema.SqlServer.SqlServerDataset()
                {
                    Name = "TEMP",
                    ConnectionString = "data source=localhost;initial catalog=Graywulf_Temp",
                },
                PartitioningKeyMin = partitioningKeyMin ? (IComparable)(1.0) : null,
                PartitioningKeyMax = partitioningKeyMax ? (IComparable)(1.0) : null
            };

            var cg = new SqlQueryCodeGenerator(partition);
            CallMethod(cg, "RewriteForExecute", ss);
            CallMethod(cg, "RemoveNonStandardTokens", ss);
            Assert.AreEqual(gt, ss.ToString());
        }

        #region Simple code rewrite functions

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
            var gt = "SELECT * FROM Table  WHERE @keyMin <= id";

            RewriteQueryHelper(sql, gt, true, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningFromWithWhere()
        {
            var sql = "SELECT * FROM Table PARTITION BY id WHERE x < 5";
            var gt = "SELECT * FROM Table  WHERE (@keyMin <= id) AND (x < 5)";

            RewriteQueryHelper(sql, gt, true, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningTo()
        {
            var sql = "SELECT * FROM Table PARTITION BY id";
            var gt = "SELECT * FROM Table  WHERE id < @keyMax";

            RewriteQueryHelper(sql, gt, false, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBoth()
        {
            var sql = "SELECT * FROM Table PARTITION BY id";
            var gt = "SELECT * FROM Table  WHERE @keyMin <= id AND id < @keyMax";

            RewriteQueryHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithWhere()
        {
            var sql = "SELECT * FROM Table PARTITION BY id WHERE x < 5";
            var gt = "SELECT * FROM Table  WHERE (@keyMin <= id AND id < @keyMax) AND (x < 5)";

            RewriteQueryHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithJoin()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id CROSS JOIN Table2";
            var gt = "SELECT * FROM Table1  CROSS JOIN Table2 WHERE @keyMin <= id AND id < @keyMax";

            RewriteQueryHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithWhereAndJoin()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id CROSS JOIN Table2 WHERE x < 5";
            var gt = "SELECT * FROM Table1  CROSS JOIN Table2 WHERE (@keyMin <= id AND id < @keyMax) AND (x < 5)";

            RewriteQueryHelper(sql, gt, true, true);
        }

        #endregion

        // TODO: add remote table name substitution tests

        #region Statistics query tests

        // TODO: propagate up to test base class
        protected string GetStatisticsQuery(string sql)
        {
            var q = CreateQuery(sql);
            var cg = new SqlQueryCodeGenerator(q);
            var ts = q.SelectStatement.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();

            // Column to compute the statistics on, not partitioning!
            ts.TableReference.Statistics = new Graywulf.SqlParser.TableStatistics()
            {
                BinCount = 200,
                KeyColumn = Expression.Create(new ColumnReference("dec", DataTypes.SqlFloat)),
                KeyColumnDataType = DataTypes.SqlFloat
            };

            var cmd = cg.GetTableStatisticsCommand(ts);

            return cmd.CommandText;
        }

        [TestMethod]
        public void GetTableStatisticsCommandTest()
        {
            var sql = @"
SELECT objID
FROM TEST:SDSSDR7PhotoObjAll
WHERE ra > 2";

            var gt = @"INSERT [Graywulf_Temp].[dbo].[test__stat_TEST_dbo_SDSSDR7PhotoObjAll] WITH(TABLOCKX)
SELECT ROW_NUMBER() OVER (ORDER BY [dec]), [dec]
FROM [SkyNode_Test].[dbo].[SDSSDR7PhotoObjAll]
WHERE [SkyNode_Test].[dbo].[SDSSDR7PhotoObjAll].[ra] > 2;";

            var res = GetStatisticsQuery(sql);
            Assert.IsTrue(res.Contains(gt));
        }

        [TestMethod]
        public void GetTableStatisticsJoinTest()
        {
            var sql = @"
SELECT p.objID
FROM TEST:SDSSDR7PhotoObjAll p
INNER JOIN TEST:SDSSDR7PhotoObjAll b ON p.objID = b.objID
WHERE p.ra > 2";

            var gt = @"INSERT [Graywulf_Temp].[dbo].[test__stat_TEST_dbo_SDSSDR7PhotoObjAll_p] WITH(TABLOCKX)
SELECT ROW_NUMBER() OVER (ORDER BY [dec]), [dec]
FROM [SkyNode_Test].[dbo].[SDSSDR7PhotoObjAll] AS [p]
WHERE [p].[ra] > 2;";

            var res = GetStatisticsQuery(sql);
            Assert.IsTrue(res.Contains(gt));
        }
        
        // TODO mode tests

        #endregion
        
        #region Column name escaping

        [TestMethod]
        public void EscapeColumnNameTest()
        {
            var tr = new TableReference()
            {
                DatasetName = "TEST",
                SchemaName = "sch",
                DatabaseObjectName = "table",
                Alias = "a",
            };

            var gt = "TEST_sch_table_a_col";

            var res = (string)CallMethod(CodeGenerator.CreateColumnListGenerator(), "EscapeColumnName", tr, "col");

            Assert.AreEqual(gt, res);
        }

        #endregion
    }
}
