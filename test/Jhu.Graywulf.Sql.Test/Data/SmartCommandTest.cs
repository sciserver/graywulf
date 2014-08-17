using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
                using (var cmd = new SmartCommand(cn.CreateCommand()))
                {
                    cmd.CommandText = sql;
                    cmd.RecordsCounted = false;

                    using (var dr = cmd.ExecuteReader())
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
                using (var cmd = IOTestDataset.CreateCommand(sql, cn))
                {
                    cmd.RecordsCounted = true;

                    using (var dr = cmd.ExecuteReader())
                    {
                        Assert.AreEqual(1, dr.RecordCount);
                    }
                }
            }
        }
    }
}
