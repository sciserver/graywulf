using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryTraversal;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class QuerySpecificationTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void QuerySpecificationSourceTablesTest()
        {
            string sql;
            QuerySpecification qs;

            sql = "SELECT 1";
            qs = ParseAndResolveNames<QuerySpecification>(sql);
            Assert.AreEqual(0, qs.SourceTableReferences.Count);

            sql = "SELECT * FROM Author";
            qs = ParseAndResolveNames<QuerySpecification>(sql);
            Assert.AreEqual(1, qs.SourceTableReferences.Count);
            Assert.AreEqual("Author", qs.SourceTableReferences["Author"].DatabaseObjectName);

            
            sql = "SELECT * FROM Book, Author";
            qs = ParseAndResolveNames<QuerySpecification>(sql);
            Assert.AreEqual(2, qs.SourceTableReferences.Count);
            Assert.AreEqual("Book", qs.SourceTableReferences["Book"].DatabaseObjectName);
            Assert.AreEqual("Author", qs.SourceTableReferences["Author"].DatabaseObjectName);

            sql = "SELECT * FROM Book, Author a, Author b";
            qs = ParseAndResolveNames<QuerySpecification>(sql);
            Assert.AreEqual(3, qs.SourceTableReferences.Count);
            Assert.AreEqual("Book", qs.SourceTableReferences["Book"].DatabaseObjectName);
            Assert.AreEqual("Author", qs.SourceTableReferences["a"].DatabaseObjectName);
            Assert.AreEqual("Author", qs.SourceTableReferences["b"].DatabaseObjectName);

            sql = "SELECT * FROM Author CROSS JOIN Book";
            qs = ParseAndResolveNames<QuerySpecification>(sql);
            Assert.AreEqual(2, qs.SourceTableReferences.Count);
            Assert.AreEqual("Author", qs.SourceTableReferences["Author"].DatabaseObjectName);
            Assert.AreEqual("Book", qs.SourceTableReferences["Book"].DatabaseObjectName);

            sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID";
            qs = ParseAndResolveNames<QuerySpecification>(sql);
            Assert.AreEqual(3, qs.SourceTableReferences.Count);
            Assert.AreEqual("Author", qs.SourceTableReferences["Author"].DatabaseObjectName);
            Assert.AreEqual("Book", qs.SourceTableReferences["Book"].DatabaseObjectName);
            Assert.AreEqual("BookAuthor", qs.SourceTableReferences["BookAuthor"].DatabaseObjectName);
        }

        [TestMethod]
        public void EnumerateSourcesTables_SubqueryTest()
        {
            string sql;
            QueryDetails qd;

            sql = "SELECT * FROM Author";
            qd = ParseAndResolveNames(sql);
            Assert.AreEqual(1, qd.SourceTableReferences.Count);
            Assert.AreEqual("Author", qd.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Author"][0].DatabaseObjectName);

            
            sql = "SELECT * FROM (SELECT * FROM Author) a";
            qd = ParseAndResolveNames(sql);
            Assert.AreEqual(1, qd.SourceTableReferences.Count);
            Assert.AreEqual("Author", qd.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Author"][0].DatabaseObjectName);
        }

        [TestMethod]
        public void EnumerateSourcesTables_SemiJoinTest()
        {
            string sql;
            QueryDetails qd;

            sql = "SELECT * FROM Author WHERE ID IN (SELECT * FROM Book)";
            qd = ParseAndResolveNames(sql);
            Assert.AreEqual(2, qd.SourceTableReferences.Count);
            Assert.AreEqual("Author", qd.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Author"][0].DatabaseObjectName);
            Assert.IsTrue(qd.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Author"][0].TableContext.HasFlag(TableContext.TableOrView));
            Assert.AreEqual("Book", qd.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Book"][0].DatabaseObjectName);
            Assert.IsTrue(qd.SourceTableReferences["Table|TEST|Graywulf_Schema_Test|dbo|Book"][0].TableContext.HasFlag(TableContext.TableOrView));
        }
    }
}
