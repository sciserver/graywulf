using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Metadata
{
    [TestClass]
    public class ParserTest
    {
        const string TestConnectionString = "Data Source=localhost; Initial Catalog=Graywulf_Metadata_Test;Integrated Security=true;";

        private string TestFile
        {
            get
            {
                return Path.Combine(
                    Path.GetDirectoryName(Environment.GetEnvironmentVariable("SolutionPath")),
                    @"graywulf\test\files\metadata.sql");
            }
        }

        [TestMethod]
        public void ExtractXmlTest()
        {
            var sql = File.ReadAllText(TestFile);
            var p = new Parser();
            var xml = p.Parse(sql);

            var formatted = XElement.Parse(xml.OuterXml).ToString();

        }

        [TestMethod]
        public void GeneratorTest()
        {
            var sql = File.ReadAllText(TestFile);
            var p = new Parser();
            var xml = p.Parse(sql);

            var formatted = XElement.Parse(xml.OuterXml).ToString();

            var g = new Generator(new System.Data.SqlClient.SqlConnectionStringBuilder(TestConnectionString));

            g.LoadXml(xml);
            g.CreateMetadata();
        }
    }
}
