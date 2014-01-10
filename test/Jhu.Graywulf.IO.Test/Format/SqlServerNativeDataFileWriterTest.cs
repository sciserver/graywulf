using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.Format
{
    [TestClass]
    public class SqlServerNativeDataFileWriterTest
    {
        [TestMethod]
        public void SimpleWriterTest()
        {
            var uri = new Uri("SqlServerNativeDataFileWriterTest_SimpleWriterTest.dat.zip", UriKind.Relative);

            using (var nat = new SqlServerNativeDataFile(uri, DataFileMode.Write))
            {
                using (var cn = new SqlConnection(Jhu.Graywulf.Test.AppSettings.IOTestConnectionString))
                {
                    cn.Open();

                    using (var cmd = new SqlCommand("SELECT * FROM SampleData_NumericTypes", cn))
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
        public void SimpleWriterNullsTest()
        {
            var uri = new Uri("SqlServerNativeDataFileWriterTest_SimpleWriterNullsTest.dat.zip", UriKind.Relative);

            using (var nat = new SqlServerNativeDataFile(uri, DataFileMode.Write))
            {
                using (var cn = new SqlConnection(Jhu.Graywulf.Test.AppSettings.IOTestConnectionString))
                {
                    cn.Open();

                    using (var cmd = new SqlCommand("SELECT * FROM SampleData_NumericTypes_Null", cn))
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
            var uri = new Uri("SqlServerNativeDataFileWriterTest_SimpleWriterAllTypesTest.dat.zip", UriKind.Relative);

            using (var nat = new SqlServerNativeDataFile(uri, DataFileMode.Write))
            {
                using (var cn = new SqlConnection(Jhu.Graywulf.Test.AppSettings.IOTestConnectionString))
                {
                    cn.Open();

                    using (var cmd = new SqlCommand("SELECT * FROM SampleData_AllTypes", cn))
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
            var uri = new Uri("SqlServerNativeDataFileWriterTest_SimpleWriterAllTypesNullableTest.dat.zip", UriKind.Relative);

            using (var nat = new SqlServerNativeDataFile(uri, DataFileMode.Write))
            {
                using (var cn = new SqlConnection(Jhu.Graywulf.Test.AppSettings.IOTestConnectionString))
                {
                    cn.Open();

                    using (var cmd = new SqlCommand("SELECT * FROM SampleData_AllTypes_Nullable", cn))
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
