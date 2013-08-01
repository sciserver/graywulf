using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Web.Test
{
    [TestClass]
    public class VirtualPathProviderTest
    {
        [TestMethod]
        public void TestDiscoverFiles()
        {
            var vpp = new VirtualPathProvider(new string[] { "~/Layout/" });

            var files = vpp.GetFiles();

            Assert.IsTrue(vpp.FileExists("~/Layout/Basic.master"));
        }
    }
}
