using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.Test.Security
{
    [TestClass]
    public class AuthenticatorSettingsTest
    {
        [TestMethod]
        public void SerializeSettingsTest()
        {
            var oid = new OpenIDAuthenticator()
                {
                    AuthorityName = "Google",
                    AuthorityUrl = "https://www.google.com/accounts/o8/ud",
                    DisplayName = "GoogleID",
                    DiscoveryUrl = "https://www.google.com/accounts/o8/id"
                };

            var ks = new KeystoneAuthenticator()
            {
                AuthorityName = "Keystone",
                AuthorityUrl = "http://localhost:5000",
                DisplayName = "Keystone",
            };

            var p = new Parameter();
            p.Value = new Authenticator[] { oid };
        }
    }
}
