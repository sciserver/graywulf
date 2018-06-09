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
        public void IsNullableTypeTest()
        {
            Type elementType;

            Assert.IsFalse(RestServiceReflector.IsNullableType(typeof(int), out elementType));
            Assert.IsTrue(RestServiceReflector.IsNullableType(typeof(int?), out elementType));
        }

        [TestMethod]
        public void IsArrayTypeTest()
        {
            Type elementType;

            Assert.IsFalse(RestServiceReflector.IsArrayType(typeof(int), out elementType));
            Assert.IsFalse(RestServiceReflector.IsArrayType(typeof(int?), out elementType));
            Assert.IsTrue(RestServiceReflector.IsArrayType(typeof(int[]), out elementType));
            Assert.IsFalse(RestServiceReflector.IsArrayType(typeof(string), out elementType));
            Assert.IsTrue(RestServiceReflector.IsArrayType(typeof(IEnumerable<int>), out elementType));
            Assert.IsTrue(RestServiceReflector.IsArrayType(typeof(IList<int>), out elementType));
            Assert.IsTrue(RestServiceReflector.IsArrayType(typeof(List<int>), out elementType));
        }

        [TestMethod]
        public void IsCollectionTypeTest()
        {
            Type keyType;
            Type elementType;

            Assert.IsFalse(RestServiceReflector.IsDictionaryType(typeof(int), out keyType, out elementType));
            Assert.IsFalse(RestServiceReflector.IsDictionaryType(typeof(int[]), out keyType, out elementType));
            Assert.IsTrue(RestServiceReflector.IsDictionaryType(typeof(Dictionary<string, int>), out keyType, out elementType));
        }

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
        public void GenerateSwaggerJsonTest()
        {
            var r = new RestServiceReflector();
            r.ReflectServiceContract(typeof(ITestService), "http://localhost/TestService.svc");
            var cg = new SwaggerJsonGenerator(r.Api);
            var code = cg.Execute();
        }
    }
}
