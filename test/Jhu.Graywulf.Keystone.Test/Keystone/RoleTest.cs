using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Keystone
{
    [TestClass]
    public class RoleTest : KeystoneTestBase
    {
        [TestMethod]
        public void ManipulateRoleTest()
        {
            PurgeTestEntities();

            // Create a role within the default domain
            var role = CreateTestRole();

            // Get the role by id
            role = Client.GetRole(role.ID);

            // Rename it to something else
            role.Name = "test_role2";
            role = Client.Update(role);

            // Delete project
            Client.Delete(role);

            PurgeTestEntities();
        }
    }
}
