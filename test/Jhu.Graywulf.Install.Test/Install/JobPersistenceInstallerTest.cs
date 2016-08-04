using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Install
{
    [TestClass]
    public class JobPersistenceInstallerTest
    {
        [TestMethod]
        public void CreateDBTest()
        {
            var dbi = new JobPersistenceInstaller(Jhu.Graywulf.Test.AppSettings.JobPersistenceTestConnectionString);
            dbi.DropDatabase(false);
            dbi.CreateDatabase();
            dbi.CreateSchema();
        }
    }
}
