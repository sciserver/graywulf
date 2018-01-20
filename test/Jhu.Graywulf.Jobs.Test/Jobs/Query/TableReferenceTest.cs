using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Jobs.Query
{
    [TestClass]
    public class TableReferenceTest : SqlQueryTestBase
    {
        // TODO: rewrite all these to use a TEST database instead of SkyQuery stuff

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
            InitializeJobTests();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            CleanupJobTests();
            StopLogger();
        }

        #region Source table tests

        [TestMethod]
        public void CollectSourceTablesTest1()
        {
            // Test if tables are correctly collected from all select statements
            var sql =
@"SELECT * FROM TEST:CatalogA a
SELECT * FROM TEST:CatalogB b";
            var q = CreateQuery(sql);

            Assert.AreEqual(2, q.SourceTables.Count);
        }

        [TestMethod]
        public void CollectSourceTablesTest2()
        {
            // Test is tables are correctly collected from all select statements
            var sql = @"SELECT * FROM (SELECT * FROM TEST:CatalogA) a";
            var q = CreateQuery(sql);

            Assert.AreEqual(1, q.SourceTables.Count);
        }

        [TestMethod]
        public void CollectSourceTablesTest3()
        {
            // Test is tables are correctly collected from all select statements
            var sql =
@"DECLARE @v int = (SELECT TOP 1 objid FROM TEST:CatalogA)
SELECT * FROM TEST:CatalogB";
            var q = CreateQuery(sql);

            Assert.AreEqual(2, q.SourceTables.Count);
        }

        [TestMethod]
        public void SourceTableColumnsTest1()
        {
            // Test is tables are correctly collected from all select statements
            var sql = @"SELECT objid FROM TEST:CatalogA";
            var q = CreateQuery(sql);
            var tr = q.SourceTables.Values.First()[0];

            Assert.AreEqual(1, q.SourceTables.Count);
            Assert.AreEqual(12, tr.ColumnReferences.Count);
            Assert.AreEqual("objId", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.SelectList, tr.ColumnReferences[0].ColumnContext);
            Assert.AreEqual("ra", tr.ColumnReferences[1].ColumnName);
            Assert.AreEqual(ColumnContext.None, tr.ColumnReferences[1].ColumnContext);
        }

        [TestMethod]
        public void SourceTableColumnsTest2()
        {
            // Test is tables are correctly collected from all select statements
            var sql =
@"SELECT objid FROM TEST:CatalogA
SELECT ra FROM TEST:CatalogA";
            var q = CreateQuery(sql);
            var tr = q.SourceTables.Values.First()[0];

            Assert.AreEqual(1, q.SourceTables.Count);
            Assert.AreEqual(12, tr.ColumnReferences.Count);
            Assert.AreEqual("objId", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.SelectList, tr.ColumnReferences[0].ColumnContext);
            Assert.AreEqual("ra", tr.ColumnReferences[1].ColumnName);
            Assert.AreEqual(ColumnContext.SelectList, tr.ColumnReferences[1].ColumnContext);
        }

        [TestMethod]
        public void SourceTableColumnsTest3()
        {
            // Test is tables are correctly collected from all select statements
            var sql =
@"SELECT objid FROM TEST:CatalogA
SELECT ra FROM TEST:CatalogA WHERE objid = 2";
            var q = CreateQuery(sql);
            var tr = q.SourceTables.Values.First()[0];

            Assert.AreEqual(1, q.SourceTables.Count);
            Assert.AreEqual(12, tr.ColumnReferences.Count);
            Assert.AreEqual("objId", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.SelectList | ColumnContext.Where, tr.ColumnReferences[0].ColumnContext);
            Assert.AreEqual("ra", tr.ColumnReferences[1].ColumnName);
            Assert.AreEqual(ColumnContext.SelectList, tr.ColumnReferences[1].ColumnContext);
        }

        #endregion
        #region Output table tests

        [TestMethod]
        public void CollectOutputTablesTest1()
        {
            var sql =
@"SELECT TOP 1 * 
INTO MYDB:newtable
FROM TEST:CatalogA";
            var q = CreateQuery(sql);

            Assert.AreEqual(1, q.OutputTables.Count);
        }

        #endregion
        #region Column list functions

        private List<Column> GetColumnListTestHelper(string sql, ColumnContext context)
        {
            var q = CreateQuery(sql);
            var cg = new SqlQueryCodeGenerator(q);
            var ts = q.ParsingTree.FindDescendantRecursive<SelectStatement>().QueryExpression.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();
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
