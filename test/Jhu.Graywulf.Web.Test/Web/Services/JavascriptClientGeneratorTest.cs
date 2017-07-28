using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Web.Services
{
    [TestClass]
    public class JavascriptClientGeneratorTest
    {
        [TestMethod]
        public void GenerateTest()
        {
            var cg = new JavascriptProxyGenerator(typeof(ITestService), "http://localhost/TestService.svc");
            var code = cg.Execute();
        }
    }
}
