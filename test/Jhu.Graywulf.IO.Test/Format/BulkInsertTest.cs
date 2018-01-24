using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Data.SqlClient;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Format
{
    [TestClass]
    public class BulkInsertTest : TestClassBase
    {
        private FileDataReader OpenSimpleReader(string csv)
        {
            var f = new DelimitedTextDataFile(new StringReader(csv))
            {
                GenerateIdentityColumn = false,
            };
            var b = new DelimitedTextDataFileBlock(f);

            b.Columns.Add(new Column("one", typeof(int), 4));
            b.Columns.Add(new Column("two", typeof(int), 4));
            b.Columns.Add(new Column("three", typeof(int), 4));

            f.AppendBlock(b);

            var cmd = new FileCommand(f);

            return cmd.ExecuteReader();
        }

        private void ExecuteBulkInsert(FileDataReader dr)
        {
            var cstr = Jhu.Graywulf.Test.AppSettings.IOTestConnectionString;
            var dropsql = "IF OBJECT_ID('testtable','U') IS NOT NULL DROP TABLE testtable";
            var createsql = "CREATE TABLE testtable (one int, two int, three int)";

            using (var cn = new SqlConnection(cstr))
            {
                cn.Open();

                // Drop table first
                using (var cmd = new SqlCommand(dropsql, cn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create empty table
                using (var cmd = new SqlCommand(createsql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
                    
                // Run bulk-insert
                var bcp = new SqlBulkCopy(cn);
                bcp.DestinationTableName = "testtable";
                bcp.WriteToServer(dr);

                // Drop table
                using (var cmd = new SqlCommand(dropsql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        public void SimpleBulkInsertTest()
        {
            var csv =
@"1,2,3
4,5,6";

            ExecuteBulkInsert(OpenSimpleReader(csv));
        }
    }
}
