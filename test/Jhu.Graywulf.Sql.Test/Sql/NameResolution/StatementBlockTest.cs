using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class StatementBlockTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void CollectSourceTablesTest1()
        {
            // Test is tables are correctly collected from all select statements
            var sql =
@"SELECT * FROM Author
SELECT * FROM Book";

            var ss = Parse(sql);

            Assert.AreEqual(2, ss.SourceTableReferences.Count);
        }

        [TestMethod]
        public void CollectSourceTablesTest2()
        {
            // Test is tables are correctly collected from all select statements
            var sql = @"SELECT * FROM (SELECT * FROM Author) a";

            var ss = Parse(sql);

            Assert.AreEqual(1, ss.SourceTableReferences.Count);
        }

        [TestMethod]
        public void CollectSourceTablesTest3()
        {
            // Test is tables are correctly collected from all select statements
            var sql = 
@"DECLARE @v int = (SELECT TOP 1 ID FROM Author)
SELECT * FROM Book";

            var ss = Parse(sql);

            Assert.AreEqual(2, ss.SourceTableReferences.Count);
        }

        [TestMethod]
        public void SourceTableColumnsTest1()
        {
            // Test is tables are correctly collected from all select statements
            var sql = @"SELECT ID FROM Author";
            var ss = Parse(sql);
            var tr = ss.SourceTableReferences.Values.First();

            Assert.AreEqual(1, ss.SourceTableReferences.Count);
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
            var ss = Parse(sql);
            var tr = ss.SourceTableReferences.Values.First();

            Assert.AreEqual(1, ss.SourceTableReferences.Count);
            Assert.AreEqual(2, tr.ColumnReferences.Count);
            Assert.AreEqual("ID", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.SelectList, tr.ColumnReferences[0].ColumnContext);
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
            var ss = Parse(sql);
            var tr = ss.SourceTableReferences.Values.First();

            Assert.AreEqual(1, ss.SourceTableReferences.Count);
            Assert.AreEqual(2, tr.ColumnReferences.Count);
            Assert.AreEqual("ID", tr.ColumnReferences[0].ColumnName);
            Assert.AreEqual(ColumnContext.PrimaryKey | ColumnContext.SelectList | ColumnContext.Where, tr.ColumnReferences[0].ColumnContext);
            Assert.AreEqual("Name", tr.ColumnReferences[1].ColumnName);
            Assert.AreEqual(ColumnContext.SelectList, tr.ColumnReferences[1].ColumnContext);
        }
    }
}
