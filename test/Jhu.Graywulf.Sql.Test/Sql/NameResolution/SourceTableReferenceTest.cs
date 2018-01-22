using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class SourceTableReferenceTest : SqlNameResolverTestBase
    {

        #region Source table tests

        [TestMethod]
        public void CollectSourceTablesTest1()
        {
            // Test if tables are correctly collected from all select statements
            var sql =
@"SELECT * FROM Author a
SELECT * FROM Book b";
            var details = Parse(sql);

            Assert.AreEqual(2, details.SourceTables.Count);
        }

        [TestMethod]
        public void CollectSourceTablesTest2()
        {
            // Test is tables are correctly collected from all select statements
            var sql = @"SELECT * FROM (SELECT * FROM Author) a";
            var details = Parse(sql);

            Assert.AreEqual(1, details.SourceTables.Count);
        }

        [TestMethod]
        public void CollectSourceTablesTest3()
        {
            // Test is tables are correctly collected from all select statements
            var sql =
@"DECLARE @v int = (SELECT TOP 1 ID FROM Author)
SELECT * FROM Book";
            var details = Parse(sql);

            Assert.AreEqual(2, details.SourceTables.Count);
        }

        [TestMethod]
        public void SourceTableColumnsTest1()
        {
            // Test is tables are correctly collected from all select statements
            var sql = @"SELECT ID FROM Author";
            var details = Parse(sql);
            var tr = details.SourceTables.Values.First()[0];

            Assert.AreEqual(1, details.SourceTables.Count);
            Assert.AreEqual(2, tr.ColumnReferences.Count);
            Assert.AreEqual("ID", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.SelectList, tr.ColumnReferences[0].ColumnContext);
            Assert.AreEqual("Name", tr.ColumnReferences[1].ColumnName);
            Assert.AreEqual(ColumnContext.None, tr.ColumnReferences[1].ColumnContext);
        }

        [TestMethod]
        public void SourceTableColumnsTest2()
        {
            // Test is tables are correctly collected from all select statements
            var sql =
@"SELECT ID FROM Author
SELECT Name FROM Author";
            var details = Parse(sql);

            var tr = details.SourceTables.Values.First()[0];
            Assert.AreEqual(1, details.SourceTables.Count);
            Assert.AreEqual(2, tr.ColumnReferences.Count);
            Assert.AreEqual("ID", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.SelectList, tr.ColumnReferences[0].ColumnContext);

            tr = details.SourceTables.Values.First()[1];
            Assert.AreEqual("Name", tr.ColumnReferences[1].ColumnName);
            Assert.AreEqual(ColumnContext.SelectList, tr.ColumnReferences[1].ColumnContext);
        }

        [TestMethod]
        public void SourceTableColumnsTest3()
        {
            // Test is tables are correctly collected from all select statements
            var sql =
@"SELECT ID FROM Author
SELECT Name FROM Author WHERE ID = 2";
            var details = Parse(sql);

            var tr = details.SourceTables.Values.First()[0];
            Assert.AreEqual(1, details.SourceTables.Count);
            Assert.AreEqual(2, tr.ColumnReferences.Count);
            Assert.AreEqual("ID", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.SelectList, tr.ColumnReferences[0].ColumnContext);

            tr = details.SourceTables.Values.First()[1];
            Assert.AreEqual("ID", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.Where, tr.ColumnReferences[0].ColumnContext);
            Assert.AreEqual("Name", tr.ColumnReferences[1].ColumnName);
            Assert.AreEqual(ColumnContext.SelectList, tr.ColumnReferences[1].ColumnContext);
        }

        #endregion
        #region Output table tests

        [TestMethod]
        public void CollectOutputTablesTest1()
        {
            var sql =
@"SELECT TOP 1 * 
INTO newtable
FROM Author";
            var details = Parse(sql);

            Assert.AreEqual(1, details.OutputTables.Count);
        }

        #endregion
        #region Column list functions

        private List<Column> GetColumnListTestHelper(string sql, ColumnContext context)
        {
            var details = Parse(sql);
            var ts = details.ParsingTree.FindDescendantRecursive<SelectStatement>().QueryExpression.EnumerateQuerySpecifications().First().EnumerateSourceTables(false).First();
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
FROM Author a";

            var cols = GetColumnListTestHelper(sql, ColumnContext.PrimaryKey);
            Assert.AreEqual(1, cols.Count);
            Assert.AreEqual("ID", cols[0].Name);
        }

        [TestMethod]
        public void GetColumnListPrimaryKeyReferencedTest()
        {
            var sql = @"
SELECT Title
FROM Book b
WHERE Year > 2000";

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default | ColumnContext.PrimaryKey);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("ID", cols[0].Name);
            Assert.AreEqual("Title", cols[1].Name);
            Assert.AreEqual("Year", cols[2].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedInSelectTest()
        {
            var sql = @"
SELECT ID, Title, Year
FROM Book b";

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("ID", cols[0].Name);
            Assert.AreEqual("Title", cols[1].Name);
            Assert.AreEqual("Year", cols[2].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedInWhereTest()
        {
            var sql = @"
SELECT ID
FROM Book b
WHERE Title = 'test' AND Year > 2000";

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("ID", cols[0].Name);
            Assert.AreEqual("Title", cols[1].Name);
            Assert.AreEqual("Year", cols[2].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedInJoinTest()
        {
            var sql = @"
SELECT a.Title, a.Year
FROM Book a
INNER JOIN Book b ON a.ID = b.ID";

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
            Assert.AreEqual(3, cols.Count);
            Assert.AreEqual("ID", cols[0].Name);
            Assert.AreEqual("Title", cols[1].Name);
            Assert.AreEqual("Year", cols[2].Name);
        }

        [TestMethod]
        public void GetColumnListReferencedSelectStarTest()
        {
            var sql = @"
SELECT *
FROM Book";

            var cols = GetColumnListTestHelper(sql, ColumnContext.Default);
            Assert.AreEqual(3, cols.Count);
        }

        #endregion
    }
}
