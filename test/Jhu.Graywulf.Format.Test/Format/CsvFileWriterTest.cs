using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Jhu.Graywulf.Format;
using System.Xml;

namespace Jhu.Graywulf.Format
{
    [TestClass]
    public class CsvFileWriterTest
    {
        [TestMethod]
        public void SimpleWriterTest()
        {
            var w = new StringWriter();

            using (var cn = new SqlConnection(Jhu.Graywulf.Test.Constants.TestConnectionString))
            {
                cn.Open();

                //using (var cmd = new SqlCommand("SELECT SampleData.* FROM SampleData", cn))
                using (var cmd = new SqlCommand(" SELECT top 2 ra,dec from Frame ", cn))
                {
                    using (var dr = cmd.ExecuteReader())
                    {

                        var csv = new CsvFile(w);
                        csv.WriteFromDataReader(dr);
                    }
                }
            }
            Assert.AreEqual(
@"#ra,dec
336.514598443829,-0.931640678911959
336.514598443829,-0.931640678911959
", w.ToString());

//            Assert.AreEqual( 
//@"#float,double,decimal,nvarchar(50),bigint,int,tinyint,smallint,bit,ntext,char,datetime,guid
//1.234568,1.23456789,1.2346,""this is text"",123456789,123456,123,12345,True,""this is unicode text ő"",""A"",08/17/2012 00:00:00,68652251-c9e4-4630-80be-88b96d3258ce
//",
//                w.ToString());

        }

        [TestMethod]
        public void CompressedWriterTest()
        {
            var uri = new Uri("file:///CsvFileTest_CompressedWriterTest.csv.gz");

            using (var csv = new CsvFile(uri, DataFileMode.Write))
            {
                csv.Compression = CompressionMethod.GZip;

                using (var cn = new SqlConnection(Jhu.Graywulf.Test.Constants.TestConnectionString))
                {
                    cn.Open();

                    //using (var cmd = new SqlCommand("SELECT SampleData.* FROM SampleData", cn))
                    using (var cmd = new SqlCommand(" SELECT top 2 ra,dec from Frame ", cn))
                    {
                        using (var dr = cmd.ExecuteReader())
                        {
                            csv.WriteFromDataReader(dr);
                        }
                    }
                }
            }

            Assert.IsTrue(File.Exists(uri.PathAndQuery));
            File.Delete(uri.PathAndQuery);
        }
    }
}
