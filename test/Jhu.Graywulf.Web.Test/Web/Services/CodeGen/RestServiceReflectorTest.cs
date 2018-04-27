using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    [TestClass]
    public class RestServiceReflectorTest
    {
        [TestMethod]
        public void ReflectServiceTest()
        {
            var r = new RestServiceReflector();
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Services.ITestService), "http://localhost/Api/V1/Service.svc");

            Assert.AreEqual(1, r.Api.ServiceContracts.Count);
        }

        [TestMethod]
        public void GenerateJavascriptClientTest()
        {
            var r = new RestServiceReflector();
            r.ReflectServiceContract(typeof(ITestService), "http://localhost/TestService.svc");
            var cg = new JavascriptProxyGenerator(r.Api);
            var code = cg.Execute();
        }

        [TestMethod]
        public void GenerateTest()
        {
            var r = new RestServiceReflector();
            r.ReflectServiceContract(typeof(ITestService), "http://localhost/TestService.svc");
            var cg = new SwaggerYamlGenerator(r.Api);
            var code = cg.Execute();
        }
    }
}
