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
    public class SqlQueryColumnListGeneratorTest : SqlQueryCodeGeneratorTest
    {
        #region Column list functions

        private Dictionary<string, Column> GetColumnListTestHelper(string sql, ColumnContext context)
        {
            var q = CreateQuery(sql);
            var cg = new SqlQueryCodeGenerator(q);
            var ts = q.SelectStatement.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();
            var columns = new SqlQueryColumnListGenerator(ts.TableReference, context);

            return columns.GetColumnList();
        }

        [TestMethod]
        public void GetColumnListPrimaryKeysTest()
        {
            var sql = @"
SELECT *
FROM TEST:SDSSDR7PhotoObjAll p";

            var cols = GetColumnListTestHelper(sql, ColumnContext.PrimaryKey);
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

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default | ColumnContext.PrimaryKey);
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

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
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

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
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

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
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

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
            Assert.AreEqual(23, cols.Count);
        }

        #endregion
        #region Propagated column list functions

        private string GeneratePropagatedColumnListTestHelper(string sql, string tableAlias, ColumnListType type)
        {
            var q = CreateQuery(sql);
            var cg = new SqlQueryCodeGenerator(q);
            var ts = q.SelectStatement.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();
            var columnlist = new SqlQueryColumnListGenerator(ts.TableReference)
            {
                TableAlias = tableAlias,
                ListType = type,
            };

            return columnlist.GetColumnListString();
        }

        [TestMethod]
        public void GeneratePropagatedColumnListForSelectNoAliasTest()
        {
            var sql = @"
SELECT objid
FROM TEST:SDSSDR7PhotoObjAll p
WHERE ra > 5 AND dec < 3";

            var gt = "[_TEST_dbo_SDSSDR7PhotoObjAll_p_objId], [_TEST_dbo_SDSSDR7PhotoObjAll_p_ra], [_TEST_dbo_SDSSDR7PhotoObjAll_p_dec]";

            var res = GeneratePropagatedColumnListTestHelper(sql, "", ColumnListType.ForSelectWithEscapedNameNoAlias);

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

            var res = GeneratePropagatedColumnListTestHelper(sql, "", ColumnListType.ForSelectWithOriginalName);

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

            var res = GeneratePropagatedColumnListTestHelper(sql, "", ColumnListType.ForSelectWithEscapedName);

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

    }
}
