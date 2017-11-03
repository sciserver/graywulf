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
    }
}
