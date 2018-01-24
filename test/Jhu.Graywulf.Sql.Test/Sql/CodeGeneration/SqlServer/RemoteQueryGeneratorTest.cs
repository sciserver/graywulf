using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.LogicalExpressions;

namespace Jhu.Graywulf.Sql.CodeGeneration.SqlServer
{
    [TestClass]
    public class RemoteQueryGeneratorTest : SqlServerCodeGeneratorTestBase
    {
        private string GenerateCode(string query, bool resolveAliases, bool resolveNames, bool substituteStars)
        {
            var ss = CreateSelect(query);
            var w = new StringWriter();

            var cg = new SqlServerCodeGenerator();
            cg.TableNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.TableAliasRendering = resolveAliases ? AliasRendering.Always : AliasRendering.Never;
            cg.ColumnNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.ColumnAliasRendering = resolveAliases ? AliasRendering.Always : AliasRendering.Never;
            cg.FunctionNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.Execute(w, ss);

            return w.ToString();
        }

        [TestMethod]
        public void WithoutResolvedNamesTest()
        {
            var sql =
@"SELECT Title, Name
FROM Book
INNER JOIN BookAuthor ON BookAuthor.BookID = Book.ID AND Book.ID = 6
INNER JOIN Author ON Author.ID = BookAuthor.AuthorID
WHERE Author.ID = 3";

            Assert.AreEqual(sql, GenerateCode(sql, false, false, false));
        }

        [TestMethod]
        public void WithResolvedNamesTest()
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
                GenerateCode(sql, false, true, false));
        }

        [TestMethod]
        public void MultipleTableOccuranceTest()
        {
            var sql =
@"SELECT b1.Title, b2.Title
FROM Book b1, Book b2
WHERE b1.ID = 1 AND b2.ID = 2";

            var res = GenerateCode(sql, true, true, false);

            Assert.AreEqual(
@"SELECT [b1].[Title] AS [b1_Title], [b2].[Title] AS [b2_Title]
FROM [Graywulf_Schema_Test].[dbo].[Book] [b1], [Graywulf_Schema_Test].[dbo].[Book] [b2]
WHERE [b1].[ID] = 1 AND [b2].[ID] = 2", res);

        }

        #region Most restrictive remote query generation

        private string[] GenerateMostRestrictiveTableQueryTestHelper(string sql, ColumnContext columnContext, int top)
        {
            var query = Parse(sql);
            var cg = new RemoteQueryGenerator();
            var res = cg.Execute(query, columnContext, top);

            return res.Values.ToArray();
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_SimpleTest()
        {
            var sql =
@"SELECT b.Title
FROM Book b
WHERE b.ID = 1";

            var gt =
@"SELECT 
[ID], [Title]
FROM [Graywulf_Schema_Test].[dbo].[Book] 
WHERE ([ID] = 1)
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_MultipleAliasesTest()
        {
            var sql =
@"SELECT a.Title, b.ID
FROM Book a CROSS JOIN Book b
WHERE b.ID = 1 AND a.ID IN (3, 4)";

            var gt =
@"SELECT 
[ID], [Title]
FROM [Graywulf_Schema_Test].[dbo].[Book] 
WHERE ([ID] = 1) OR ([ID] IN (3, 4))
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_MultipleSelectsTest()
        {
            var sql =
@"SELECT a.Title FROM Book a
WHERE a.ID IN (3, 4)

SELECT b.Year FROM Book b
WHERE b.Title = 'Test'";

            var gt =
@"SELECT 
[ID], [Title], [Year]
FROM [Graywulf_Schema_Test].[dbo].[Book] 
WHERE ([Title] = 'Test') OR ([ID] IN (3, 4))
";

            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        [TestMethod]
        public void GenerateMostRestrictiveTableQuery_UnionTest()
        {
            var sql =
@"SELECT Title, ID
FROM Book
WHERE ID IN (2, 3)
UNION
SELECT Title, ID + 1
FROM Book
WHERE ID = 1";

            var gt =
@"SELECT 
[ID], [Title]
FROM [Graywulf_Schema_Test].[dbo].[Book] 
WHERE ([ID] = 1) OR ([ID] IN (2, 3))
";
            var res = GenerateMostRestrictiveTableQueryTestHelper(sql, ColumnContext.Default, 0);

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(gt, res[0]);
        }

        #endregion

        /* TODO: rewrite this
        [TestMethod]
        public void GenerateCreateDestinationTableQueryTest()
        {
            DataTable schema;

            using (var cn = new SqlConnection(Jhu.Graywulf.Test.AppSettings.IOTestConnectionString))
            {
                cn.Open();

                var sql = "SELECT * FROM SampleData";
                using (var cmd = new SqlCommand(sql, cn))
                {
                    using (var dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                    {
                        schema = dr.GetSchemaTable();
                    }
                }
            }

            var dest = new Table()
            {
                SchemaName = Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName,
                TableName = "destination"
            };

            var cg = new SqlServerCodeGenerator();
            var res = cg.GenerateCreateDestinationTableQuery(schema, dest);

            Assert.AreEqual(@"CREATE TABLE [dbo].[destination] ([float] real  NULL,
[double] float  NULL,
[decimal] money  NULL,
[nvarchar(50)] nvarchar(50)  NULL,
[bigint] bigint  NULL,
[int] int NOT NULL,
[tinyint] tinyint  NULL,
[smallint] smallint  NULL,
[bit] bit  NULL,
[ntext] nvarchar(max)  NULL,
[char] char(1)  NULL,
[datetime] datetime  NULL,
[guid] uniqueidentifier  NULL)", res);
        }*/

        [TestMethod]
        public void GenerateCreatePrimaryKeyScriptTest()
        {
            var ds = CreateTestDataset();
            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithPrimaryKey"];
            var cg = new SqlServerCodeGenerator();

            var sql = cg.GenerateCreatePrimaryKeyScript(t);

            var gt =
@"ALTER TABLE [Graywulf_Schema_Test].[dbo].[TableWithPrimaryKey]
ADD CONSTRAINT [PK_TableWithPrimaryKey] PRIMARY KEY CLUSTERED (
[ID]
 )
";

            Assert.AreEqual(gt, sql);
        }

        [TestMethod]
        public void GenerateDropPrimaryKeyScriptTest()
        {
            var ds = CreateTestDataset();
            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithPrimaryKey"];
            var cg = new SqlServerCodeGenerator();

            var sql = cg.GenerateDropPrimaryKeyScript(t);

            var gt =
@"ALTER TABLE [Graywulf_Schema_Test].[dbo].[TableWithPrimaryKey]
DROP CONSTRAINT [PK_TableWithPrimaryKey]";

            Assert.AreEqual(gt, sql);
        }
    }
}
