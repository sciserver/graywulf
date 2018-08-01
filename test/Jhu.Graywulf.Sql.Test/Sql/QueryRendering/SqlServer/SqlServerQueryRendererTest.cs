using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryRendering;
using Jhu.Graywulf.Sql.QueryRendering.SqlServer;

namespace Jhu.Graywulf.Sql.QueryRendering.SqlServer
{
    [TestClass]
    public class SqlServerQueryRendererTest : SqlServerTestBase
    {
        [TestMethod]
        public void RenderWithoutResolvedNamesTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID AND Book.ID = 6
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Author.ID = 3";

            Assert.AreEqual(sql, RenderQuery(sql, false, false, false));
        }

        [TestMethod]
        public void RenderWithResolvedNamesTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID AND Book.ID = 6
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Author.ID = 3";

            Assert.AreEqual(
@"SELECT [Graywulf_Schema_Test].[dbo].[Book].[Title], [Graywulf_Schema_Test].[dbo].[Author].[Name]
FROM [Graywulf_Schema_Test].[dbo].[Book]
INNER JOIN [Graywulf_Schema_Test].[dbo].[BookAuthor] ON [Graywulf_Schema_Test].[dbo].[BookAuthor].[BookID] = [Graywulf_Schema_Test].[dbo].[Book].[ID] AND [Graywulf_Schema_Test].[dbo].[Book].[ID] = 6
INNER JOIN [Graywulf_Schema_Test].[dbo].[Author] ON [Graywulf_Schema_Test].[dbo].[Author].[ID] = [Graywulf_Schema_Test].[dbo].[BookAuthor].[AuthorID]
WHERE [Graywulf_Schema_Test].[dbo].[Author].[ID] = 3",
                RenderQuery(sql, false, true, false));
        }

        [TestMethod]
        public void MultipleTableOccuranceTest()
        {
            var sql =
@"SELECT b1.Title, b2.Title
FROM Book b1, Book b2
WHERE b1.ID = 1 AND b2.ID = 2";

            var res = RenderQuery(sql, true, true, false);

            Assert.AreEqual(
@"SELECT [b1].[Title], [b2].[Title]
FROM [Graywulf_Schema_Test].[dbo].[Book] [b1], [Graywulf_Schema_Test].[dbo].[Book] [b2]
WHERE [b1].[ID] = 1 AND [b2].[ID] = 2", res);

        }
    }
}
