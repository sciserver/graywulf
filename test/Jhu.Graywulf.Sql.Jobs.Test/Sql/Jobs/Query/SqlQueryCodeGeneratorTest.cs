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
using Jhu.Graywulf.Sql.QueryGeneration.SqlServer;
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

            var qg = new SqlQueryCodeGenerator(q);
            var ss = q.QueryDetails.ParsingTree.FindDescendantRecursive<SelectStatement>();
            var qs = ss.QueryExpression.EnumerateQuerySpecifications().First();
            var ts = qs.SourceTableReferences.Values.First().TableSource;

            // Column to compute the statistics on, not partitioning!
            var stat = new TableStatistics(ts)
            {
                BinCount = 200,
                KeyColumn = Expression.Create(new ColumnReference(null, null, "dec", new DataTypeReference(DataTypes.SqlFloat)), 0),
                KeyColumnDataType = DataTypes.SqlFloat
            };

            q.TableStatistics.Add(ts.UniqueKey, stat);

            var cmd = qg.GetTableStatisticsCommand(ts, null);

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
        
       
        
    }
}
