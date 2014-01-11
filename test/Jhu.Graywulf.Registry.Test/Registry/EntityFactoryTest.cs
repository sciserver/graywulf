using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    [TestClass]
    public class EntityFactoryTest
    {

        [TestMethod]
        public void SaveRegistryTest()
        {

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);
                var entity = f.LoadEntity(Constants.ClusterName);

                // *** TODO: create mask from input parameters
                var mask = EntityType.AllCluster | EntityType.AllDomain | EntityType.AllFederation | EntityType.AllLayout | EntityType.AllSecurity | EntityType.AllJobs;
                //mask = (EntityType)((int)mask & ~(0x0400));  // Leave out job instances

                using (var outfile = new StreamWriter("EntityFactoryTest_SaveRegistryTest.xml"))
                {
                    f.Serialize(entity, outfile, mask);
                }
            }

        }

    }
}
