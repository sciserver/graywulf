using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.QueryGeneration.SqlServer
{
    [TestClass]
    public class SqlServerCodeGeneratorTest : SqlServerTestBase
    {
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
