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
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.Query
{
    [TestClass]
    public class SqlQueryColumnListGeneratorTest : SqlQueryCodeGeneratorTest
    {
        #region Propagated column list functions

        private string GeneratePropagatedColumnListTestHelper(string sql, string tableAlias, ColumnContext columnContext, ColumnListType listType)
        {
            var q = CreateQuery(sql);
            var cg = new SqlQueryCodeGenerator(q);
            var ts = q.SelectStatement.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();
            var columnlist = new SqlServerColumnListGenerator(ts.TableReference, columnContext, listType);
            columnlist.TableAlias = tableAlias;

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

            var res = GeneratePropagatedColumnListTestHelper(sql, "", ColumnContext.Default, ColumnListType.ForSelectWithEscapedNameNoAlias);

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

            var res = GeneratePropagatedColumnListTestHelper(sql, "", ColumnContext.Default, ColumnListType.ForSelectWithOriginalName);

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

            var res = GeneratePropagatedColumnListTestHelper(sql, "", ColumnContext.Default, ColumnListType.ForSelectWithEscapedName);

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

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnContext.Default, ColumnListType.ForCreateTableWithOriginalName);

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

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnContext.Default, ColumnListType.ForCreateView);

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

            var res = GeneratePropagatedColumnListTestHelper(sql, null, ColumnContext.Default, ColumnListType.ForInsert);

            Assert.AreEqual(gt, res);
        }

        #endregion

    }
}
