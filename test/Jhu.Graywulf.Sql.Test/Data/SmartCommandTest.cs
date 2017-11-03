using System;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Data
{
    [TestClass]
    public class SmartCommandTest : TestClassBase
    {
        [TestMethod]
        public void NoCountResultsTest()
        {
            var sql = "SELECT * FROM SampleData";

            using (var cn = IOTestDataset.OpenConnection())
            {
                using (var cmd = new SmartCommand(IOTestDataset, cn.CreateCommand()))
                {
                    cmd.CommandText = sql;
                    cmd.RecordsCounted = false;

                    using (var dr = cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None).Result)
                    {
                        Assert.AreEqual(-1, dr.RecordCount);
                    }
                }
            }
        }

        [TestMethod]
        public void CountResultsTest()
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
                    }
                }
            }
        }
    }
}
