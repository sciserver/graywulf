using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Util
{
    [TestClass]
    public class ServiceControllerTest
    {
        [TestMethod]
        public void WaitForServiceTest()
        {
            ServiceHelper.WaitForService("WAS", 1000, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceProcess.TimeoutException))]
        public void WaitForStoppedServiceTest()
        {
            ServiceHelper.WaitForService("TapiSrv", 1000, 5);
        }
    }
}
