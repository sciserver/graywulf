using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.RemoteService
{
    [TestClass]
    public class RemoteControlTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var co = RemoteServiceHelper.GetControlObject(Jhu.Graywulf.Test.Constants.Localhost);

                var str = co.Hello();
            }
        }

        [TestMethod]
        public void TestMethod2()
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
        public void TestMethod3()
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
    }
}
