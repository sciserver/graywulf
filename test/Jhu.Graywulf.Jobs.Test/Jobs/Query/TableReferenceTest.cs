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
    public class TableReferenceTest : SqlQueryTestBase
    {
        #region Column list functions

        private List<Column> GetColumnListTestHelper(string sql, ColumnContext context)
        {
            var q = CreateQuery(sql);
            var cg = new SqlQueryCodeGenerator(q);
            var ts = q.SelectStatement.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();
            var table = (IColumns)ts.TableReference.DatabaseObject;
            var cr = ts.TableReference.FilterColumnReferences(context);
            var cs = cr.Select(c => table.Columns[c.ColumnName]);

            return new List<Column>(cs);
        }

        [TestMethod]
        public void GetColumnListPrimaryKeysTest()
        {
            var sql = @"
SELECT *
FROM TEST:SDSSDR7PhotoObjAll p";

            var cols = GetColumnListTestHelper(sql, ColumnContext.PrimaryKey);
            Assert.AreEqual(1, cols.Count);
            Assert.AreEqual("objId", cols[0].Name);
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
            Assert.AreEqual("objId", cols[0].Name);
            Assert.AreEqual("ra", cols[1].Name);
            Assert.AreEqual("dec", cols[2].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedInSelectTest()
        {
            var sql = @"
SELECT objid, ra, dec
FROM TEST:SDSSDR7PhotoObjAll p";

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("objId", cols[0].Name);
            Assert.AreEqual("ra", cols[1].Name);
            Assert.AreEqual("dec", cols[2].Name);
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
            Assert.AreEqual("objId", cols[0].Name);
            Assert.AreEqual("ra", cols[1].Name);
            Assert.AreEqual("dec", cols[2].Name);
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
            Assert.AreEqual("objId", cols[0].Name);
            Assert.AreEqual("ra", cols[1].Name);
            Assert.AreEqual("dec", cols[2].Name);
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
    }
}
