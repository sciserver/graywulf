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
    public class RegistrySerializerTest
    {
        private void SaveRegistry(string filename)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);
                var entity = f.LoadEntity(EntityType.Cluster, Constants.ClusterName);
                var s = new RegistrySerializer(entity)
                {
                    Recursive = true,
                    ExcludeUserCreated = true,
                    EntityGroupMask = EntityGroup.Cluster
                };

                using (var outfile = new StreamWriter(filename))
                {
                    s.Serialize(outfile);
                }
            }
        }

        private void CleanUpTestRegistry()
        {
            using (var cn = new SqlConnection(Jhu.Graywulf.Test.AppSettings.RegistryTestConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand("dev.spCleanupEverything", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadTestRegistry(string filename, DuplicateMergeMethod duplicateMergeMethod)
        {
            using (var context = ContextManager.Instance.CreateContext(
                Jhu.Graywulf.Test.AppSettings.RegistryTestConnectionString,
                ConnectionMode.AutoOpen,
                TransactionMode.AutoCommit))
            {
                var s = new RegistryDeserializer(context)
                {
                    DuplicateMergeMethod = duplicateMergeMethod
                };

                using (var infile = new StreamReader(filename))
                {
                    s.Deserialize(infile);
                }
            }
        }

        [TestMethod]
        public void SaveRegistryTest()
        {
            using (RegistryTester.Instance.GetExclusiveToken())
            {
                // If this test fails with an XML serialization error, make sure
                // to modify EntityFactory.Registry
                SaveRegistry("RegistrySerializerTest_SaveRegistryTest.xml");
            }
        }

        [TestMethod]
        public void LoadRegistryTest()
        {
            using (RegistryTester.Instance.GetExclusiveToken())
            {
                var filename = "RegistrySerializerTest_LoadRegistryTest.xml";
                SaveRegistry(filename);
                CleanUpTestRegistry();
                LoadTestRegistry(filename, DuplicateMergeMethod.Ignore);
            }
        }

        [TestMethod]
        public void IgnoreDuplicateTest()
        {
            using (RegistryTester.Instance.GetExclusiveToken())
            {
                var filename = "EntityFactoryTest_IgnoreDuplicateTest.xml";
                SaveRegistry(filename);
                CleanUpTestRegistry();
                LoadTestRegistry(filename, DuplicateMergeMethod.Ignore);
                LoadTestRegistry(filename, DuplicateMergeMethod.Ignore);
            }
        }

        [TestMethod]
        public void UpdateDuplicateTest()
        {
            using (RegistryTester.Instance.GetExclusiveToken())
            {
                var filename = "EntityFactoryTest_UpdateDuplicateTest.xml";
                SaveRegistry(filename);
                CleanUpTestRegistry();
                LoadTestRegistry(filename, DuplicateMergeMethod.Ignore);
                LoadTestRegistry(filename, DuplicateMergeMethod.Update);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateNameException))]
        public void FailOnDuplicateTest()
        {
            using (RegistryTester.Instance.GetExclusiveToken())
            {
                var filename = "EntityFactoryTest_FailOnDuplicateTest.xml";
                SaveRegistry(filename);
                CleanUpTestRegistry();
                LoadTestRegistry(filename, DuplicateMergeMethod.Ignore);
                LoadTestRegistry(filename, DuplicateMergeMethod.Fail);
            }
        }
    }
}
