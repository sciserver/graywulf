using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Keystone
{
    [TestClass]
    public class TrustTest : KeystoneTestBase
    {
        [TestMethod]
        public void CreateTrustTest()
        {
            // Purge test users
            PurgeTestUsers();

            // Create new users
            var user1 = CreateTestUser("test1");
            var user2 = CreateTestUser("test2");

            // Make user1 trust user2
            var trust = new Trust()
            {
                ExpiresAt = DateTime.Now.AddDays(2),
                Impersonation = true,
                TrustorUserID = user1.ID,
                TrusteeUserID = user2.ID
            };

            trust = Client.CreateTrust(trust);
        }
    }
}
