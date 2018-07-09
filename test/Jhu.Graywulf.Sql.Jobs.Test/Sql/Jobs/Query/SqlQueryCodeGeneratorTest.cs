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
                KeyColumn = Expression.Create(new ColumnReference(null, null, "dec", new DataTypeReference(DataTypes.SqlFloat))),
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


        [TestMethod]
        public void PartitionVariablesTest()
        {
            var sql = @"SELECT @@PARTCOUNT, @@PARTID";
            var gt = @"SELECT @__partCount, @__partId";

            RewriteQueryHelper(sql, gt);
        }

        #endregion
    }
}
