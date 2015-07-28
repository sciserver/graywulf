using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                var entity = f.LoadEntity(EntityType.Cluster, Constants.ClusterName);

                using (var outfile = new StreamWriter(filename))
                {
                    f.Serialize(entity, outfile, EntityGroup.All, false);
                }
            }
        }

        [TestMethod]
        public void SaveRegistryTest()
        {
            // If this test fails with an XML serialization error, make sure
            // to modify EntityFactory.Registry

            SaveRegistry("EntityFactoryTest_SaveRegistryTest.xml");
        }

        [TestMethod]
        public void LoadRegistryTest()
        {
            var filename = "EntityFactoryTest_LoadRegistryTest.xml";

            SaveRegistry(filename);

            // Clean up registry test DB before load
            using (var cn = new SqlConnection(Jhu.Graywulf.Test.AppSettings.RegistryTestConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand("dev.spCleanupEverything", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }

            ContextManager.Instance.ConnectionString = Jhu.Graywulf.Test.AppSettings.RegistryTestConnectionString;

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {

                var f = new EntityFactory(context);

                using (var infile = new StreamReader(filename))
                {
                    f.Deserialize(infile, true);
                }
            }
        }

    }
}
