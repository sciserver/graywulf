using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry.Test
{
    [TestClass]
    public class XmlSerializationTest
    {
        private void TrySerialize(object o)
        {
            var ms = new System.IO.MemoryStream();
            var ser = new XmlSerializer(o.GetType());
            ser.Serialize(ms, o);
        }

        [TestMethod]
        public void SerializeClusterTest()
        {
            TrySerialize(new Cluster());
        }

        [TestMethod]
        public void SerializeDatabaseDefinitionTest()
        {
            TrySerialize(new DatabaseDefinition());
        }

        [TestMethod]
        public void SerializeDatabaseInstanceTest()
        {
            TrySerialize(new DatabaseInstance());
        }

        [TestMethod]
        public void SerializeDatabaseInstanceFileTest()
        {
            TrySerialize(new DatabaseInstanceFile());
        }

        [TestMethod]
        public void SerializeDatabaseInstanceFileGroupTest()
        {
            TrySerialize(new DatabaseInstanceFileGroup());
        }

        [TestMethod]
        public void SerializeDatabaseVersionTest()
        {
            TrySerialize(new DatabaseVersion());
        }

        [TestMethod]
        public void SerializeDeploymentPackageTest()
        {
            TrySerialize(new DeploymentPackage());
        }

        [TestMethod]
        public void SerializeDiskVolumeTest()
        {
            TrySerialize(new DiskVolume());
        }

        [TestMethod]
        public void SerializeDomainTest()
        {
            TrySerialize(new Domain());
        }

        [TestMethod]
        public void SerializeFederationTest()
        {
            TrySerialize(new Federation());
        }

        [TestMethod]
        public void SerializeFileGroupTest()
        {
            TrySerialize(new FileGroup());
        }

        [TestMethod]
        public void SerializeJobDefinitionTest()
        {
            TrySerialize(new JobDefinition());
        }

        [TestMethod]
        public void SerializeJobInstanceTest()
        {
            TrySerialize(new JobInstance());
        }

        [TestMethod]
        public void SerializeMachineTest()
        {
            TrySerialize(new Machine());
        }

        [TestMethod]
        public void SerializeMachineRoleTest()
        {
            TrySerialize(new MachineRole());
        }

        [TestMethod]
        public void SerializePartitionTest()
        {
            TrySerialize(new Partition());
        }

        [TestMethod]
        public void SerializeQueueDefinitionTest()
        {
            TrySerialize(new QueueDefinition());
        }

        [TestMethod]
        public void SerializeQueueInstanceTest()
        {
            TrySerialize(new QueueInstance());
        }

        [TestMethod]
        public void SerializeRemoteDatabaseTest()
        {
            TrySerialize(new RemoteDatabase());
        }

        [TestMethod]
        public void SerializeServerInstanceTest()
        {
            TrySerialize(new ServerInstance());
        }

        [TestMethod]
        public void SerializeServerVersionTest()
        {
            TrySerialize(new ServerVersion());
        }

        [TestMethod]
        public void SerializeSliceTest()
        {
            TrySerialize(new Slice());
        }

        [TestMethod]
        public void SerializeUserTest()
        {
            TrySerialize(new User());
        }

        [TestMethod]
        public void SerializeUserDatabaseInstanceTest()
        {
            TrySerialize(new UserDatabaseInstance());
        }

        [TestMethod]
        public void SerializeUserGroupTest()
        {
            TrySerialize(new UserGroup());
        }
    }
}
