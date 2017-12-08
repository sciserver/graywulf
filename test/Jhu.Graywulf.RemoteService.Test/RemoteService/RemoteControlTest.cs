using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService.Server;

namespace Jhu.Graywulf.RemoteService
{
    [TestClass]
    public class RemoteControlTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.Stop();
                StopLogger();
            }
        }

        [TestMethod]
        public void CallHelloTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost))
                {
                    Assert.AreEqual(typeof(Program).Assembly.FullName, sc.Value.Hello());
                }
            }
        }

        [TestMethod]
        public void CallWhoAmITest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost))
                {
                    string name, authtype;
                    bool isauth;

                    sc.Value.WhoAmI(out name, out isauth, out authtype);

                    // get current identity
                    var id = WindowsIdentity.GetCurrent();

                    Assert.AreEqual(id.Name, name);
                    Assert.AreEqual(id.IsAuthenticated, isauth);
                    Assert.AreEqual(id.AuthenticationType, authtype);
                }
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

                sc.Value.WhoAreYou(out name, out isauth, out authtype);

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

                using (var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost))
                {
                    Assert.AreEqual(typeof(Program).Assembly.FullName, sc.Value.Hello());

                    var uri = sc.Value.GetServiceEndpointUri(typeof(ICancelableDelay).AssemblyQualifiedName);

                    Assert.AreEqual(
                        new Uri(String.Format(
                            "net.tcp://{0}:{1}/{2}",
                            DnsHelper.GetFullyQualifiedDnsName(Jhu.Graywulf.Test.Constants.Localhost),
                            RemoteServiceBase.Configuration.Endpoint.TcpPort,
                            typeof(CancelableDelay).FullName)),
                        uri);
                }
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

                using (var cancellationContext = new CancellationContext())
                {
                    var delay = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, false);

                    using (var sc = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost))
                    {
                        var ss = sc.Value.QueryRegisteredServices();
                        Assert.AreEqual(1, ss.Length);
                    }
                }
            }
        }
    }
}
