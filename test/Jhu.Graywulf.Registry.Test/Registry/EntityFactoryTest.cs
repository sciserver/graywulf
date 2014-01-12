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
        private void SaveRegistry(string filename)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);
                var entity = f.LoadEntity(Constants.ClusterName);

                // *** TODO: create mask from input parameters
                var mask = new HashSet<EntityType>()
                {
                    EntityType.JobInstance,
                    EntityType.DatabaseInstanceFileGroup,
                    EntityType.DatabaseInstanceFile,
                };

                using (var outfile = new StreamWriter(filename))
                {
                    f.Serialize(entity, outfile, mask);
                }
            }
        }

        [TestMethod]
        public void SaveRegistryTest()
        {
            SaveRegistry("EntityFactoryTest_SaveRegistryTest.xml");
        }

        [TestMethod]
        public void LoadRegistryTest()
        {
            var filename = "EntityFactoryTest_LoadRegistryTest.xml";

            SaveRegistry(filename);

            ContextManager.Instance.ConnectionString = "Data Source=gwen1;Initial Catalog=Graywulf_Registry_Test;Integrated Security=true";

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);

                using (var infile = new StreamReader(filename))
                {
                    f.Deserialize(infile);
                }
            }
        }

    }
}
