using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.RemoteService.Server;

namespace Jhu.Graywulf.RemoteService
{
    [TestClass]
    public class RemoteControlTest
    {
        [TestMethod]
        public void CallHelloTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost);

                Assert.AreEqual(typeof(Program).Assembly.FullName, sc.Hello());
            }
        }

        [TestMethod]
        public void CallWhoAmITest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost);

                string name, authtype;
                bool isauth;

                sc.WhoAmI(out name, out isauth, out authtype);

                // get current identity
                var id = WindowsIdentity.GetCurrent();

                Assert.AreEqual(id.Name, name);
                Assert.AreEqual(id.IsAuthenticated, isauth);
                Assert.AreEqual(id.AuthenticationType, authtype);

            }
        }

        [TestMethod]
        public void CallWhoAreYouTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost);

                string name, authtype;
                bool isauth;

                sc.WhoAreYou(out name, out isauth, out authtype);

                // get current identity
                var id = WindowsIdentity.GetCurrent();

                Assert.AreEqual(id.Name, name);
                Assert.AreEqual(id.IsAuthenticated, isauth);
                Assert.AreEqual(id.AuthenticationType, authtype);

            }
        }

        [TestMethod]
        public void GetServiceEndpointUriTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost);
                Assert.AreEqual(typeof(Program).Assembly.FullName, sc.Hello());

                var uri = sc.GetServiceEndpointUri(typeof(ICancelableDelay).AssemblyQualifiedName);

                Assert.AreEqual(
                    new Uri(String.Format(
                        "net.tcp://{0}:{1}/{2}",
                        RemoteServiceHelper.GetFullyQualifiedDnsName(Jhu.Graywulf.Test.Constants.Localhost),
                        RemoteServiceBase.Configuration.TcpPort,
                        typeof(CancelableDelay).FullName)),
                    uri);

            }
        }

        [TestMethod]
        public void QueryRegisteredServicesTest()
        {
            using (RemoteServiceTester.Instance.GetExclusiveToken())
            {
                // Make sure it's restarted
                RemoteServiceTester.Instance.EnsureRunning();
                RemoteServiceTester.Instance.Stop();

                RemoteServiceTester.Instance.EnsureRunning();

                var delay = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost);

                var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost);
                var ss = sc.QueryRegisteredServices();

                Assert.AreEqual(1, ss.Length);
            }
        }
    }
}
