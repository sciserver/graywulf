using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Web.Services.CodeGen;

namespace Jhu.Graywulf.Web.Api.V1
{
    [TestClass]
    public class GenerateProxyTest
    {
        [TestMethod]
        public void ReflectServicesTest()
        {
            var r = new RestServiceReflector();
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.IAuthService), "http://localhost/Api/V1/Service.svc");
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.IDataService), "http://localhost/Api/V1/Service.svc");
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.IJobsService), "http://localhost/Api/V1/Service.svc");
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.IManageService), "http://localhost/Api/V1/Service.svc");
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.ISchemaService), "http://localhost/Api/V1/Service.svc");

            Assert.AreEqual(5, r.Api.ServiceContracts.Count);
        }

        [TestMethod]
        public void GenerateSwaggerJsonTest()
        {
            var r = new RestServiceReflector();
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.IAuthService), "http://localhost/Api/V1/Service.svc");
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.IDataService), "http://localhost/Api/V1/Service.svc");
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.IJobsService), "http://localhost/Api/V1/Service.svc");
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.IManageService), "http://localhost/Api/V1/Service.svc");
            r.ReflectServiceContract(typeof(Jhu.Graywulf.Web.Api.V1.ISchemaService), "http://localhost/Api/V1/Service.svc");

            var cg = new SwaggerJsonGenerator(r.Api);
            var code = cg.Execute();
        }
    }
}
