using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.QueryGeneration.SqlServer
{
    [TestClass]
    public class SqlServerQueryGeneratorTest : SqlServerTestBase
    {
        [TestMethod]
        public void GenerateCreateDestinationTableQueryTest()
        {
            var source = IOTestDataset.Tables[null, null, "SampleData"];
            var dest = new Table(source)
            {
                SchemaName = Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName,
                TableName = "destination"
            };
            dest.CopyColumns(source.Columns.Values);

            var cg = new SqlServerQueryGenerator();
            var res = cg.GenerateCreateTableScript(dest, false, false);

            Assert.AreEqual(@"CREATE TABLE [Graywulf_IO_Test].[dbo].[destination] (
[tinyint] tinyint NULL,
[bigint] bigint NULL,
[guid] uniqueidentifier NULL,
[bit] bit NULL,
[float] real NULL,
[decimal] money NULL,
[int] int NOT NULL,
[nvarchar(50)] nvarchar(50) NULL,
[char] char(1) NULL,
[smallint] smallint NULL,
[double] float NULL,
[datetime] datetime NULL,
[ntext] nvarchar(max) NULL )
", res);
        }


        [TestMethod]
        public void GenerateCreatePrimaryKeyScriptTest()
        {
            var ds = SchemaTestDataset;
            var t = SchemaTestDataset.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithPrimaryKey"];
            var cg = new SqlServerQueryGenerator();

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
            var ds = SchemaTestDataset;
            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithPrimaryKey"];
            var cg = new SqlServerQueryGenerator();

            var sql = cg.GenerateDropPrimaryKeyScript(t);

            var gt =
@"ALTER TABLE [Graywulf_Schema_Test].[dbo].[TableWithPrimaryKey]
DROP CONSTRAINT [PK_TableWithPrimaryKey]";

            Assert.AreEqual(gt, sql);
        }
    }
}
