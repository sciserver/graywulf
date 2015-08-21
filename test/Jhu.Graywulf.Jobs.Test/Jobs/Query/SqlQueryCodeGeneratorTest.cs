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
            CallMethod(CodeGenerator, "RemoveNonStandardTokens", ss, CommandMethod.SelectInto);
            Assert.AreEqual(gt, ss.ToString());
        }

        protected void RewriteQueryHelper(string sql, string gt, bool partitioningKeyFrom, bool partitioningKeyTo)
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
                PartitioningKeyFrom = partitioningKeyFrom ? (IComparable)(1.0) : null,
                PartitioningKeyTo = partitioningKeyTo ? (IComparable)(1.0) : null
            };

            var cg = new SqlQueryCodeGenerator(partition);
            CallMethod(cg, "RewriteForExecute", ss);
            CallMethod(cg, "RemoveNonStandardTokens", ss, CommandMethod.SelectInto);
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

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithJoin()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id CROSS JOIN Table2";
            var gt = "SELECT * FROM Table1  CROSS JOIN Table2 WHERE @keyFrom <= id AND id < @keyTo";

            RewriteQueryHelper(sql, gt, true, true);
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void AppendPartitioningBothWithWhereAndJoin()
        {
            var sql = "SELECT * FROM Table1 PARTITION BY id CROSS JOIN Table2 WHERE x < 5";
            var gt = "SELECT * FROM Table1  CROSS JOIN Table2 WHERE (@keyFrom <= id AND id < @keyTo) AND (x < 5)";

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
                KeyColumn = new ColumnReference("dec", DataTypes.SqlFloat)
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
        #region Column list functions

        private Dictionary<string, Column> GetColumnListTestHelper(string sql, ColumnListInclude include)
        {
            var q = CreateQuery(sql);
            var cg = new SqlQueryCodeGenerator(q);
            var ts = q.SelectStatement.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();

            return cg.GetColumnList(ts, include);
        }

        [TestMethod]
        public void GetColumnListPrimaryKeysTest()
        {
            var sql = @"
SELECT *
FROM TEST:SDSSDR7PhotoObjAll p";

            var cols = GetColumnListTestHelper(sql, ColumnListInclude.PrimaryKey);
            Assert.AreEqual(1, cols.Count);
            Assert.AreEqual("objId", cols["objId"].Name);
        }

        [TestMethod]
        public void GetColumnListPrimaryKeyReferencedTest()
        {
            var sql = @"
SELECT dec
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 2";

            var cols = GetColumnListTestHelper(sql, ColumnListInclude.PrimaryKey | ColumnListInclude.Referenced);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("objId", cols["objId"].Name);
            Assert.AreEqual("ra", cols["ra"].Name);
            Assert.AreEqual("dec", cols["dec"].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedInSelectTest()
        {
            var sql = @"
SELECT objid, ra, dec
FROM TEST:SDSSDR7PhotoObjAll p";

            var cols = GetColumnListTestHelper(sql, ColumnListInclude.Referenced);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("objId", cols["objId"].Name);
            Assert.AreEqual("ra", cols["ra"].Name);
            Assert.AreEqual("dec", cols["dec"].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedInWhereTest()
        {
            var sql = @"
SELECT objid
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 5 AND dec < 3";

            var cols = GetColumnListTestHelper(sql, ColumnListInclude.Referenced);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("objId", cols["objId"].Name);
            Assert.AreEqual("ra", cols["ra"].Name);
            Assert.AreEqual("dec", cols["dec"].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedInJoinTest()
        {
            var sql = @"
SELECT p.ra, p.dec
FROM TEST:SDSSDR7PhotoObjAll p
INNER JOIN TEST:SDSSDR7PhotoObjAll b ON p.objID = b.objID";

            var cols = GetColumnListTestHelper(sql, ColumnListInclude.Referenced);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("objId", cols["objId"].Name);
            Assert.AreEqual("ra", cols["ra"].Name);
            Assert.AreEqual("dec", cols["dec"].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedSelectStarTest()
        {
            var sql = @"
SELECT *
FROM TEST:SDSSDR7PhotoObjAll p";

            var cols = GetColumnListTestHelper(sql, ColumnListInclude.Referenced);
            Assert.AreEqual(23, cols.Count);
        }

        #endregion
        #region Propagated column list functions

        private string GeneratePropagatedColumnListTestHelper(string sql, string tableAlias, ColumnListType type)
        {
            var q = CreateQuery(sql);
            var cg = new SqlQueryCodeGenerator(q);
            var ts = q.SelectStatement.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();
            var include = ColumnListInclude.Referenced;
            var nullType = ColumnListNullType.Nothing;
            var leadingComma = false;

            return cg.GeneratePropagatedColumnList(ts, tableAlias, include, type, nullType, leadingComma);
        }

        [TestMethod]
        public void GeneratePropagatedColumnListForSelectNoAliasTest()
        {
            var sql = @"
SELECT objid
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 5 AND dec < 3";

            var gt = "[_TEST_dbo_SDSSDR7PhotoObjAll_p_objId], [_TEST_dbo_SDSSDR7PhotoObjAll_p_ra], [_TEST_dbo_SDSSDR7PhotoObjAll_p_dec]";

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnListType.ForSelectNoAlias);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void GeneratePropagatedColumnListForSelectWithOriginalNameTest()
        {
            var sql = @"
SELECT objid
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 5 AND dec < 3";

            var gt = "[objId] AS [_TEST_dbo_SDSSDR7PhotoObjAll_p_objId], [ra] AS [_TEST_dbo_SDSSDR7PhotoObjAll_p_ra], [dec] AS [_TEST_dbo_SDSSDR7PhotoObjAll_p_dec]";

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnListType.ForSelectWithOriginalName);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void GeneratePropagatedColumnListForSelectWithEscapedNameTest()
        {
            var sql = @"
SELECT objid
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 5 AND dec < 3";

            var gt = "[_TEST_dbo_SDSSDR7PhotoObjAll_p_objId] AS [_TEST_dbo_SDSSDR7PhotoObjAll_p_objId], [_TEST_dbo_SDSSDR7PhotoObjAll_p_ra] AS [_TEST_dbo_SDSSDR7PhotoObjAll_p_ra], [_TEST_dbo_SDSSDR7PhotoObjAll_p_dec] AS [_TEST_dbo_SDSSDR7PhotoObjAll_p_dec]";

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnListType.ForSelectWithEscapedName);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void GeneratePropagatedColumnListForCreateTableTest()
        {
            var sql = @"
SELECT objid
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 5 AND dec < 3";

            var gt = "[_TEST_dbo_SDSSDR7PhotoObjAll_p_objId] bigint, [_TEST_dbo_SDSSDR7PhotoObjAll_p_ra] float, [_TEST_dbo_SDSSDR7PhotoObjAll_p_dec] float";

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnListType.ForCreateTable);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void GeneratePropagatedColumnListForCreateViewTest()
        {
            var sql = @"
SELECT objid
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 5 AND dec < 3";

            var gt = "[_TEST_dbo_SDSSDR7PhotoObjAll_p_objId], [_TEST_dbo_SDSSDR7PhotoObjAll_p_ra], [_TEST_dbo_SDSSDR7PhotoObjAll_p_dec]";

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnListType.ForCreateView);

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void GeneratePropagatedColumnListForInsertTest()
        {
            var sql = @"
SELECT objid
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 5 AND dec < 3";

            var gt = "[_TEST_dbo_SDSSDR7PhotoObjAll_p_objId], [_TEST_dbo_SDSSDR7PhotoObjAll_p_ra], [_TEST_dbo_SDSSDR7PhotoObjAll_p_dec]";

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnListType.ForInsert);

            Assert.AreEqual(gt, res);
        }

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

            var res = (string)CallMethod(CodeGenerator, "EscapeColumnName", tr, "col");

            Assert.AreEqual(gt, res);
        }

        #endregion
    }
}
