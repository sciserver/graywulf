using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry
{
    [TestClass]
    public class InstallerTest
    {
        [TestMethod]
        public void FullInstallTest()
        {
            // Change context connection string to test
            ContextManager.Instance.ConnectionString = Jhu.Graywulf.Test.AppSettings.RegistryTestConnectionString;

            var dbi = new DBInstaller();

            dbi.DropDatabase(true);

            dbi.CreateDatabase();

            dbi.CreateSchema();

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
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

            //dbi.DropDatabase(true);
        }
    }
}
