using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Data;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.Format
{
    [TestClass]
    public class SqlServerNativeDataFileWriterTest : Jhu.Graywulf.Test.TestClassBase
    {
        [TestMethod]
        public void SimpleWriterTest()
        {
            var uri = GetTestFilename(".dat.zip");

            using (var nat = new SqlServerNativeDataFile(uri, DataFileMode.Write))
            {
                using (var cn = IOTestDataset.OpenConnection())
                {
                    using (var cmd = new SmartCommand(IOTestDataset, cn.CreateCommand()))
                    {
                        cmd.CommandText = "SELECT * FROM SampleData_NumericTypes";

                        using (var dr = cmd.ExecuteReader())
                        {
                            nat.WriteFromDataReader(dr);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SimpleWriterNullsTest()
        {
            var uri = GetTestFilename(".dat.zip");

            using (var nat = new SqlServerNativeDataFile(uri, DataFileMode.Write))
            {
                using (var cn = IOTestDataset.OpenConnection())
                {
                    using (var cmd = IOTestDataset.CreateCommand("SELECT * FROM SampleData_NumericTypes_Null", cn))
                    {
                        using (var dr = cmd.ExecuteReader())
                        {
                            nat.WriteFromDataReader(dr);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SimpleWriterAllTypesTest()
        {
            var uri = GetTestFilename(".dat.zip");

            using (var nat = new SqlServerNativeDataFile(uri, DataFileMode.Write))
            {
                using (var cn = IOTestDataset.OpenConnection())
                {
                    using (var cmd = IOTestDataset.CreateCommand("SELECT * FROM SampleData_AllTypes", cn))
                    {
                        using (var dr = cmd.ExecuteReader())
                        {
                            nat.WriteFromDataReader(dr);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SimpleWriterAllTypesNullableTest()
        {
            var uri = GetTestFilename(".dat.zip");

            using (var nat = new SqlServerNativeDataFile(uri, DataFileMode.Write))
            {
                using (var cn = IOTestDataset.OpenConnection())
                {
                    using (var cmd = IOTestDataset.CreateCommand("SELECT * FROM SampleData_AllTypes_Nullable", cn))
                    {
                        using (var dr = cmd.ExecuteReader())
                        {
                            nat.WriteFromDataReader(dr);
                        }
                    }
                }
            }
        }
    }
}
