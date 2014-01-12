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
                var ci = new ClusterInstaller(context);

                ci.Install();
            }

            dbi.DropDatabase(true);
        }
    }
}
