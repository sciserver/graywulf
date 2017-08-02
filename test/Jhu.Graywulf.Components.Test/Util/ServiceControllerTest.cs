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
            ServiceControl.WaitForService("WAS", 1000, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceProcess.TimeoutException))]
        public void WaitForStoppedServiceTest()
        {
            ServiceControl.WaitForService("TapiSrv", 1000, 5);
        }
    }
}
