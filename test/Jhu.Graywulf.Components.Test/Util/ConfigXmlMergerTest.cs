using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Util
{
    [TestClass]
    public class ConfigXmlMergerTest
    {
        [TestMethod]
        public void TestMerge()
        {
            var xml1 = new XmlDocument();
            xml1.LoadXml(@"
<configuration>
  <connectionStrings>
    <!-- Graywulf registry, log and workflow persistance database location-->
    <add name=""Jhu.Graywulf.Registry"" connectionString=""Data Source=localhost;Initial Catalog=Graywulf;MultipleActiveResultsets=true;Integrated Security=true""/>
  </connectionStrings>
</configuration>");

            var xml2 = new XmlDocument();
            xml2.LoadXml(@"
<configuration>
  <connectionStrings>
    <!-- Graywulf registry, log and workflow persistance database location-->
    <add name=""Jhu.Graywulf.Test"" connectionString=""Data Source=localhost;Initial Catalog=Graywulf;MultipleActiveResultsets=true;Integrated Security=true""/>
  </connectionStrings>
</configuration>");

            ConfigXmlMerger.Merge(xml1, xml2);

            Assert.AreEqual(2, xml1.SelectNodes("/configuration/connectionStrings/add").Count);

        }

        [TestMethod]
        public void TestOverwrite()
        {
            var xml1 = new XmlDocument();
            xml1.LoadXml(@"
<configuration>
  <system.net>
    <mailSettings>
      <smtp deliveryMethod=""Network"">
        <network host=""localhost"" port=""25"" defaultCredentials=""false"" userName="""" password=""""/>
      </smtp>
      <!--<smtp deliveryMethod=""SpecifiedPickupDirectory"">
        <specifiedPickupDirectory pickupDirectoryLocation=""""/>
      </smtp>-->
    </mailSettings>
  </system.net>
</configuration>");

            var xml2 = new XmlDocument();
            xml2.LoadXml(@"
<configuration>
  <system.net>
    <mailSettings>
      <!--<smtp deliveryMethod=""Network"">
        <network host=""localhost"" port=""25"" defaultCredentials=""false"" userName="""" password=""""/>
      </smtp>-->
      <smtp deliveryMethod=""SpecifiedPickupDirectory"">
        <specifiedPickupDirectory pickupDirectoryLocation=""""/>
      </smtp>
    </mailSettings>
  </system.net>
</configuration>");

            ConfigXmlMerger.Merge(xml1, xml2);

            Assert.AreEqual(1, xml1.SelectNodes("/configuration/system.net/mailSettings/smtp").Count);

        }
    }
}
