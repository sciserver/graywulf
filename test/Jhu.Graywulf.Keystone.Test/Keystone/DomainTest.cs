using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Keystone
{
    [TestClass]
    public class DomainTest : KeystoneTestBase
    {
        [TestMethod]
        public void ManipulateDomainTest()
        {
            PurgeTestEntities();

            // Create a domain
            var domain = CreateTestDomain();

            // Get the domain by id
            domain = Client.GetDomainByID(domain.ID);

            // Rename it to something else
            domain.Name = "test_domain2";
            domain = Client.UpdateDomain(domain);

            // Disable it
            domain.Enabled = false;
            domain = Client.UpdateDomain(domain);
            Assert.IsFalse(domain.Enabled.Value);

            // Enable it again
            domain.Enabled = true;
            domain = Client.UpdateDomain(domain);
            Assert.IsTrue(domain.Enabled.Value);

            // Delete domain
            Client.DeleteDomain(domain.ID);

            PurgeTestEntities();
        }
    }
}
