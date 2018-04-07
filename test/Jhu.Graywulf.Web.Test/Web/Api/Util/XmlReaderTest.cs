using System;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Web.Api.Util
{
    [TestClass]
    public class XmlReaderTest : Jhu.Graywulf.Test.TestClassBase
    {
        private XmlReader XmlReader
        {
            get
            {
                var fn = GetTestFilePath("modules/graywulf/test/files/xmlreadertest.xml");
                var xml = File.ReadAllText(fn);
                return new XmlReader(xml);
            }
        }

        [TestMethod]
        public void GetXmlInnerTextTest()
        {
            var xr = XmlReader;
            var val = xr.GetXmlInnerText("ArrayOfParameter/Parameter/Value/SqlQueryParameters/QueryString");
            var gt = "SELECT TOP 10 p.* INTO SqlQueryTest_AliasSelectStarQueryTest FROM SDSSDR13:PhotoObj p";

            Assert.AreEqual(gt, val);
        }

        [TestMethod]
        public void GetAsDictionaryTest()
        {
            var xr = XmlReader;
            var val = xr.GetAsDictionary("ArrayOfParameter/Parameter/Value/SqlQueryParameters/OutputTables/Table");

            Assert.AreEqual(6, val.Count);
            Assert.AreEqual("MYDB_1768162722", val["DatabaseName"]);
        }

        [TestMethod]
        public void EnumerateAsDictionaryTest()
        {
            var xr = XmlReader;
            var val = xr.EnumerateAsDictionary("ArrayOfParameter/Parameter/Value/SqlQueryParameters/OutputTables/Table").ToArray();

            Assert.AreEqual(2, val.Length);
            Assert.AreEqual(6, val[0].Count);
            Assert.AreEqual("MYDB_1768162722", val[0]["DatabaseName"]);
            Assert.AreEqual("MYDB", val[0]["DatasetName"]);
        }
    }
}
