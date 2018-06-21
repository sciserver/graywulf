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
using Jhu.Graywulf.Sql.Jobs.Query;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    [TestClass]
    public class SqlQueryCodeGeneratorTest : SqlQueryTestBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            StopLogger();
        }

        protected virtual SqlParser Parser
        {
            get { return new SqlParser(); }
        }

        protected virtual StatementBlock Parse(string sql)
        {
            return Parser.Execute<StatementBlock>(sql);
        }

        private SqlQueryCodeGenerator CodeGenerator
        {
            get
            {
                var partition = new SqlQueryPartition()
                {
                    CodeDataset = new Graywulf.Sql.Schema.SqlServer.SqlServerDataset()
                    {
                        Name = Jhu.Graywulf.Registry.Constants.CodeDbName,
                        ConnectionString = "data source=localhost;initial catalog=SkyQuery_Code",
                    },
                    TemporaryDataset = new Graywulf.Sql.Schema.SqlServer.SqlServerDataset()
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
            var script = Parse(sql);
            var ss = script.FindDescendantRecursive<SelectStatement>();
            CallMethod(CodeGenerator, "RemoveNonStandardTokens", ss);
            Assert.AreEqual(gt, CodeGenerator.Execute(script));
        }

        protected void RewriteQueryHelper(string sql, string gt)
        {
            RewriteQueryHelper(sql, gt);
        }

        protected void RewriteQueryHelper(string sql, string gt, bool partitioningKeyMin, bool partitioningKeyMax)
        {
            var script = Parse(sql);
            var ss = script.FindDescendantRecursive<SelectStatement>();
            var partition = new SqlQueryPartition()
            {
                CodeDataset = new Graywulf.Sql.Schema.SqlServer.SqlServerDataset()
                {
                    Name = "CODE",
                    ConnectionString = "data source=localhost;initial catalog=SkyQuery_Code",
                },
                TemporaryDataset = new Graywulf.Sql.Schema.SqlServer.SqlServerDataset()
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
            Assert.AreEqual(gt, CodeGenerator.Execute(script));
        }

        #region Simple code rewrite functions

        [TestMethod]
        [TestCategory("Parsing")]
        public void LeaveIntact()
        {
            var sql = "SELECT * FROM Table1";
            var gt = "SELECT * FROM Table1";

            RemoveExtraTokensHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RewriteIntoClause()
        {
            var sql = "SELECT * INTO Table2 FROM Table1 ORDER BY column1";
            var gt =
@"BEGIN
PRINT '<graywulf><destinationName>Table2</destinationName></graywulf>';
SELECT *  FROM Table1 
END";

            RewriteQueryHelper(sql, gt, false, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RewriteIntoClause2()
        {
            var sql = "SELECT * INTO dbo.Table2 FROM Table1 ORDER BY column1";
            var gt =
@"BEGIN
PRINT '<graywulf><destinationSchema>dbo</destinationSchema><destinationName>Table2</destinationName></graywulf>';
SELECT *  FROM Table1 
END";

            RewriteQueryHelper(sql, gt, false, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveOrderByClause()
        {
            var sql = "SELECT * FROM Table1 ORDER BY column1";
            var gt = "SELECT * FROM Table1 ";

            RemoveExtraTokensHelper(sql, gt);
        }
        
        [TestMethod]
        [TestCategory("Parsing")]
        public void RemoveOrderByClauseFromUnion()
        {
            var sql = "SELECT * FROM Table1 UNION SELECT * FROM Table2 ORDER BY column1";
            var gt = "SELECT * FROM Table1 UNION SELECT * FROM Table2 ";

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
            var sql = "SELECT * FROM Table1 PARTITION BY id ORDER BY column1";
            var gt = "SELECT * FROM Table1  ";

            RemoveExtraTokensHelper(sql, gt);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningFrom()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id";
            var gt = @"SELECT * FROM Table1 
WHERE @__partKeyMin <= id";

            RewriteQueryHelper(sql, gt, true, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningFromWithWhere()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id WHERE x < 5";
            var gt = "SELECT * FROM Table1  WHERE (@__partKeyMin <= id) AND (x < 5)";

            RewriteQueryHelper(sql, gt, true, false);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningTo()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id";
            var gt = @"SELECT * FROM Table1 
WHERE id < @__partKeyMax";

            RewriteQueryHelper(sql, gt, false, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBoth()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id";
            var gt = @"SELECT * FROM Table1 
WHERE @__partKeyMin <= id AND id < @__partKeyMax";

            RewriteQueryHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithWhere()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id WHERE x < 5";
            var gt = @"SELECT * FROM Table1  WHERE (@__partKeyMin <= id AND id < @__partKeyMax) AND (x < 5)";

            RewriteQueryHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithJoin()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id CROSS JOIN Table2";
            var gt = @"SELECT * FROM Table1  CROSS JOIN Table2
WHERE @__partKeyMin <= id AND id < @__partKeyMax";

            RewriteQueryHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithWhereAndJoin()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id CROSS JOIN Table2 WHERE x < 5";
            var gt = "SELECT * FROM Table1  CROSS JOIN Table2 WHERE (@__partKeyMin <= id AND id < @__partKeyMax) AND (x < 5)";

            RewriteQueryHelper(sql, gt, true, true);
        }

        #endregion

        // TODO: add remote table name substitution tests

        #region Statistics query tests

        // TODO: propagate up to test base class
        protected string GetStatisticsQuery(string sql)
        {
            var q = CreateQuery(sql);
            q.Parameters.ExecutionMode = ExecutionMode.SingleServer;

            var cg = new SqlQueryCodeGenerator(q)
            {
                TableNameRendering = CodeGeneration.NameRendering.FullyQualified,
                ColumnNameRendering = CodeGeneration.NameRendering.FullyQualified,
                DataTypeNameRendering = CodeGeneration.NameRendering.FullyQualified,
                FunctionNameRendering = CodeGeneration.NameRendering.FullyQualified,
            };
            var ts = q.QueryDetails.ParsingTree.FindDescendantRecursive<SelectStatement>().QueryExpression.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();

            // Column to compute the statistics on, not partitioning!
            var stat = new TableStatistics(ts)
            {
                BinCount = 200,
                KeyColumn = Expression.Create(new ColumnReference("dec", DataTypes.SqlFloat)),
                KeyColumnDataType = DataTypes.SqlFloat
            };

            q.TableStatistics.Add(ts.UniqueKey, stat);

            var cmd = cg.GetTableStatisticsCommand(ts, null);

            return cmd.CommandText;
        }

        [TestMethod]
        public void GetTableStatisticsCommandTest()
        {
            var sql = @"
SELECT objID
FROM TEST:SDSSDR7PhotoObjAll
WHERE ra > 2";

            var gt = @"SELECT ROW_NUMBER() OVER (ORDER BY [dec]) AS [rn], [dec] AS [key]
INTO [Graywulf_Temp].[dbo].[temp_stat_TEST_SkyNode_TEST_dbo_SDSSDR7PhotoObjAll]
FROM [SkyNode_TEST].[dbo].[SDSSDR7PhotoObjAll]
WHERE ([SkyNode_TEST].[dbo].[SDSSDR7PhotoObjAll].[ra] > 2);";

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

            var gt = @"SELECT ROW_NUMBER() OVER (ORDER BY [dec]) AS [rn], [dec] AS [key]
INTO [Graywulf_Temp].[dbo].[temp_stat_TEST_SkyNode_TEST_dbo_SDSSDR7PhotoObjAll_p]
FROM [SkyNode_TEST].[dbo].[SDSSDR7PhotoObjAll] AS [p]
WHERE ([p].[ra] > 2);";

            var res = GetStatisticsQuery(sql);
            Assert.IsTrue(res.Contains(gt));
        }
        
        // TODO mode tests

        #endregion
        
        #region Column name escaping

        [TestMethod]
        public void EscapeColumnNameTest1()
        {
            var tr = new TableReference()
            {
                DatasetName = "TEST",
                SchemaName = "sch",
                DatabaseObjectName = "table",
                Alias = "a",
            };

            var gt = "a_col";

            var columns = new SqlServerColumnListGenerator();
            var res = (string)CallMethod(columns, "EscapeColumnName", tr, "col");

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void EscapeColumnNameTest2()
        {
            var tr = new TableReference()
            {
                DatasetName = "TEST",
                SchemaName = "sch",
                DatabaseObjectName = "table",
            };

            var gt = "TEST_sch_table_col";

            var columns = new SqlServerColumnListGenerator();
            var res = (string)CallMethod(columns, "EscapeColumnName", tr, "col");

            Assert.AreEqual(gt, res);
        }

        #endregion
        #region System variables

        private void RewriteScriptHelper(string sql, string gt)
        {
            var qd = new QueryDetails()
            {
                ParsingTree = Parse(sql)
            };
            var partition = new SqlQueryPartition()
            {
                CodeDataset = new Graywulf.Sql.Schema.SqlServer.SqlServerDataset()
                {
                    Name = "CODE",
                    ConnectionString = "data source=localhost;initial catalog=SkyQuery_Code",
                },
                TemporaryDataset = new Graywulf.Sql.Schema.SqlServer.SqlServerDataset()
                {
                    Name = "TEMP",
                    ConnectionString = "data source=localhost;initial catalog=Graywulf_Temp",
                },
                //PartitioningKeyMin = partitioningKeyMin ? (IComparable)(1.0) : null,
                //PartitioningKeyMax = partitioningKeyMax ? (IComparable)(1.0) : null
            };

            var cg = new SqlQueryCodeGenerator(partition);
            var sq = (IO.Tasks.SourceQuery)CallMethod(cg, "OnGetExecuteQuery", qd);
            var res = CodeGenerator.Execute(qd.ParsingTree);
            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void PartitionVariablesTest()
        {
            var sql = @"SELECT @@PARTCOUNT, @@PARTID";
            var gt = @"SELECT @__partCount, @__partId";

            RewriteScriptHelper(sql, gt);
        }

        #endregion
    }
}
