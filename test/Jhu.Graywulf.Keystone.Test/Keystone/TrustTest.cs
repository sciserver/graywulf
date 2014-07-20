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
            PurgeTestEntities();

            // Create a project and a role
            var role = CreateTestRole();
            var project = CreateTestProject();

            // Create new users
            var user1 = CreateTestUser("test1");
            var user2 = CreateTestUser("test2");

            // Associate users with project via the role
            Client.GrantRoleToUserOnProject(project.ID, user1.ID, role.ID);
            Client.GrantRoleToUserOnProject(project.ID, user2.ID, role.ID);

            // Create token for user1
            var token1 = Client.Authenticate("default", "test1", "alma");

            // Make user1 trust user2
            var trust = new Trust()
            {
                ExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime(),
                Impersonation = true,
                TrustorUserID = user1.ID,
                TrusteeUserID = user2.ID,
                RemainingUses = 5,
                ProjectID = project.ID,
                Roles = new [] { role }
            };

            Client.UserAuthToken = token1.ID;
            trust = Client.Create(trust);

            // Try to impersonate user with trust
            var token2 = Client.Authenticate("default", "test2", "alma");

            var token3 = Client.Authenticate(token2, trust);

        }
    }
}
