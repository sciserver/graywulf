using System;
using System.Threading;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Data
{
    [TestClass]
    public class SmartDataReaderTest : TestClassBase
    {
        [TestMethod]
        public void LoadColumnsTest()
        {
            var sql = "SELECT * FROM SampleData";

            using (var cn = IOTestDataset.OpenConnection())
            {
                using (var cmd = new SmartCommand(IOTestDataset, cn.CreateCommand()))
                {
                    cmd.CommandText = sql;
                    cmd.RecordsCounted = true;

                    using (var dr = cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None).Result)
                    {
                        Assert.AreEqual(1, dr.RecordCount);
                        Assert.AreEqual(13, dr.Columns.Count);
                    }
                }
            }
        }

        [TestMethod]
        public void ColumnTypesTest()
        {
            var ds = SchemaTestDataset;
            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Sql.Schema.SqlServer.Constants.DefaultSchemaName, "TableWithAllTypes"];
            var codegen = Jhu.Graywulf.Sql.QueryGeneration.QueryGeneratorFactory.CreateQueryGenerator(ds);
            var sql = codegen.GenerateSelectStarQuery(t, 100);

            using (var cn = t.Dataset.OpenConnection())
            {
                using (var cmd = new SmartCommand(ds, cn.CreateCommand()))
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, CancellationToken.None).Result)
                    {
                        var columns = dr.Columns;

                        Assert.IsTrue(columns.Count == 31);

                        Assert.IsTrue(columns[0].DataType.TypeNameWithLength == "bigint");
                        Assert.IsTrue(columns[1].DataType.TypeNameWithLength == "decimal");                   // precision?
                        Assert.IsTrue(columns[2].DataType.TypeNameWithLength == "bit");
                        Assert.IsTrue(columns[3].DataType.TypeNameWithLength == "smallint");
                        Assert.IsTrue(columns[4].DataType.TypeNameWithLength == "decimal");
                        Assert.IsTrue(columns[5].DataType.TypeNameWithLength == "smallmoney");
                        Assert.IsTrue(columns[6].DataType.TypeNameWithLength == "int");
                        Assert.IsTrue(columns[7].DataType.TypeNameWithLength == "tinyint");
                        Assert.IsTrue(columns[8].DataType.TypeNameWithLength == "money");
                        Assert.IsTrue(columns[9].DataType.TypeNameWithLength == "float");
                        Assert.IsTrue(columns[10].DataType.TypeNameWithLength == "real");
                        Assert.IsTrue(columns[11].DataType.TypeNameWithLength == "date");
                        Assert.IsTrue(columns[12].DataType.TypeNameWithLength == "datetimeoffset");     // precision?
                        Assert.IsTrue(columns[13].DataType.TypeNameWithLength == "datetime2");               // precision?
                        Assert.IsTrue(columns[14].DataType.TypeNameWithLength == "smalldatetime");
                        Assert.IsTrue(columns[15].DataType.TypeNameWithLength == "datetime");
                        Assert.IsTrue(columns[16].DataType.TypeNameWithLength == "time");                         // precision?
                        Assert.IsTrue(columns[17].DataType.TypeNameWithLength == "char(10)");
                        Assert.IsTrue(columns[18].DataType.TypeNameWithLength == "varchar(10)");
                        Assert.IsTrue(columns[19].DataType.TypeNameWithLength == "varchar(max)");
                        Assert.IsTrue(columns[20].DataType.TypeNameWithLength == "text");
                        Assert.IsTrue(columns[21].DataType.TypeNameWithLength == "nchar(10)");
                        Assert.IsTrue(columns[22].DataType.TypeNameWithLength == "nvarchar(10)");
                        Assert.IsTrue(columns[23].DataType.TypeNameWithLength == "nvarchar(max)");
                        Assert.IsTrue(columns[24].DataType.TypeNameWithLength == "ntext");
                        Assert.IsTrue(columns[25].DataType.TypeNameWithLength == "binary(10)");
                        Assert.IsTrue(columns[26].DataType.TypeNameWithLength == "varbinary(10)");
                        Assert.IsTrue(columns[27].DataType.TypeNameWithLength == "varbinary(max)");
                        Assert.IsTrue(columns[28].DataType.TypeNameWithLength == "image");
                        Assert.IsTrue(columns[29].DataType.TypeNameWithLength == "timestamp");
                        Assert.IsTrue(columns[30].DataType.TypeNameWithLength == "uniqueidentifier");
                    }
                }
            }
        }
    }
}
