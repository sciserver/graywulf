using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Components
{
    [TestClass]
    public class AcceptHeaderParserTest
    {
        [TestMethod]
        public void ParseTest()
        {
            var str = "application/vnd.wap.wmlscriptc, text/vnd.wap.wml, application/vnd.wap.xhtml+xml, application/xhtml+xml, text/html, multipart/mixed, */*";

            var mimes = AcceptHeaderParser.Parse(str);

            Assert.AreEqual(7, mimes.Length);

            Assert.AreEqual("application/vnd.wap.wmlscriptc", mimes[0].MimeType);
            Assert.AreEqual("application/vnd.wap.xhtml+xml", mimes[2].MimeType);

            str = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8,application/json";

            mimes = AcceptHeaderParser.Parse(str);

            Assert.AreEqual(5, mimes.Length);

            Assert.AreEqual("application/xml", mimes[3].MimeType);
            Assert.AreEqual(0.9, mimes[3].Quality);
        }
    }
}
