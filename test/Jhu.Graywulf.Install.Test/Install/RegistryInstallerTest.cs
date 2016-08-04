using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry
{
    [TestClass]
    public class RegistryInstallerTest
    {
        [TestMethod]
        public void FullInstallTest()
        {
            var dbi = new RegistryInstaller(Jhu.Graywulf.Test.AppSettings.RegistryTestConnectionString);
            dbi.DropDatabase(false);
            dbi.CreateDatabase();
            dbi.CreateSchema();

            using (var context = ContextManager.Instance.CreateContext(
                Jhu.Graywulf.Test.AppSettings.RegistryTestConnectionString,
                ConnectionMode.AutoOpen, 
                TransactionMode.AutoCommit))
            {
                // Create a cluster
                var ci = new ClusterInstaller(context);
                var cluster = ci.Install();

                cluster.LoadDomains(true);
                var domain = cluster.Domains[Constants.SystemDomainName];

                // Create a federation
                var fi = new FederationInstaller(domain);
                var federation = fi.Install("Test federation");
            }
        }
    }
}
