using System;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.AccessControl
{
    [TestClass]
    public class EntityAclTest : TestClassBase
    {
        [TestMethod]
        public void EvaluateOwnerTest()
        {
            var acl = new EntityAcl()
            {
                Owner = "test"
            };

            acl.Grant(DefaultIdentity.Owner, "read");
            acl.Deny(DefaultIdentity.Owner, "write");

            var id = CreateTestPrincipal();

            var access = acl.EvaluateAccess(id);

            Assert.AreEqual(AccessType.Grant, access["read"]);
            Assert.AreEqual(AccessType.Deny, access["write"]);
        }

        [TestMethod]
        public void EvaluateUserPermissionTest()
        {
            var acl = new EntityAcl();

            acl.Grant("test", "read");
            acl.Deny("test", "write");

            var id = CreateTestPrincipal();

            var access = acl.EvaluateAccess(id);

            Assert.AreEqual(AccessType.Grant, access["read"]);
            Assert.AreEqual(AccessType.Deny, access["write"]);
        }

        [TestMethod]
        public void EvaluatePublicPermissionTest()
        {
            var acl = new EntityAcl();

            acl.Grant(DefaultIdentity.Public, "read");
            acl.Deny(DefaultIdentity.Public, "write");

            var id = CreateTestPrincipal();

            var access = acl.EvaluateAccess(id);

            Assert.AreEqual(AccessType.Grant, access["read"]);
            Assert.AreEqual(AccessType.Deny, access["write"]);
        }

        [TestMethod]
        public void EvaluateGuestPermissionTest()
        {
            var acl = new EntityAcl();

            acl.Grant(DefaultIdentity.Guest, "read");
            acl.Deny(DefaultIdentity.Guest, "write");

            var principal = CreateAnonPrincipal();

            var access = acl.EvaluateAccess(principal);

            Assert.AreEqual(AccessType.Grant, access["read"]);
            Assert.AreEqual(AccessType.Deny, access["write"]);
        }

        [TestMethod]
        public void EvaluateGroupPermissionTest()
        {
            var acl = new EntityAcl();

            acl.Grant("testgroup", "member", "read");
            acl.Deny("testgroup", "member", "write");

            var id = CreateTestPrincipal();

            id.Roles.Add("testgroup|member");

            var access = acl.EvaluateAccess(id);

            Assert.AreEqual(AccessType.Grant, access["read"]);
            Assert.AreEqual(AccessType.Deny, access["write"]);
        }

        [TestMethod]
        public void EvaluateDenyPrecedenceTest()
        {
            var acl = new EntityAcl();

            acl.Grant("testgroup", "member", "read");
            acl.Grant("testgroup", "member", "write");
            acl.Deny(DefaultIdentity.Guest, "write");

            var id = CreateTestPrincipal();

            id.Roles.Add("testgroup|member");

            var access = acl.EvaluateAccess(id);

            Assert.AreEqual(AccessType.Grant, access["read"]);
            Assert.AreEqual(AccessType.Deny, access["write"]);
        }

        [TestMethod]
        public void ClearAceTest()
        {
            var acl = new EntityAcl();

            acl.Grant("user", "read");
            acl.Grant("user", "write");
            acl.Grant("user", "delete");
            acl.Grant("group", "role", "read");
            acl.Grant("group", "role", "write");

            acl.Clear("group", "role", "read");
            Assert.AreEqual(4, acl.Count);

            acl.Clear("user", "read");
            Assert.AreEqual(3, acl.Count);

            acl.Clear("user");
            Assert.AreEqual(1, acl.Count);

            acl.Clear();
            Assert.AreEqual(0, acl.Count);
        }

        [TestMethod]
        public void CopyTest()
        {
            var a = new EntityAcl()
            {
                Owner = "a"
            };

            a.Grant("test", "read");
            a.Deny("test", "write");
            a.Grant("testgroup", "member", "read");
            a.Deny("testgroup", "member", "write");

            var b = new EntityAcl(a);

            Assert.AreEqual(a.Owner, b.Owner);

            // TODO: test evaluate
        }

        [TestMethod]
        public void BinarySerializationTest()
        {
            var ms = new MemoryStream();

            var acl = new EntityAcl()
            {
                Owner = "test",
            };

            acl.Grant(DefaultIdentity.Owner, DefaultAccess.All);
            acl.Grant("test", "create");
            acl.Grant("test", "write");
            acl.Deny("test", "read");
            acl.Grant("testgroup", "member", "read");

            var w = new BinaryWriter(ms);
            acl.ToBinary(w);

            ms.Seek(0, SeekOrigin.Begin);

            var r = new BinaryReader(ms);
            var acl2 = EntityAcl.FromBinary(r);

            Assert.AreEqual(acl.Owner, acl2.Owner);
            Assert.AreEqual(acl.Count, acl2.Count);

            var aacl = acl.ToArray();
            var aacl2 = acl2.ToArray();

            for (int i = 0; i < acl.Count; i++)
            {
                Assert.AreEqual(aacl[i].GetType(), aacl2[i].GetType());
                Assert.AreEqual(aacl[i].Name, aacl2[i].Name);
                Assert.AreEqual(aacl[i].Access, aacl2[i].Access);
                Assert.AreEqual(aacl[i].Type, aacl2[i].Type);
            }
        }


        [TestMethod]
        public void SerializeToXmlTest()
        {
            var acl = new EntityAcl()
            {
                Owner = "test",
            };

            acl.Grant(DefaultIdentity.Owner, DefaultAccess.All);
            acl.Grant("test", "write");
            acl.Grant("test", "read");
            acl.Grant("testgroup", "member", "read");

            var res = acl.ToXml();
        }

        [TestMethod]
        public void DeserializeFromXmlTest()
        {
            var xml = "<acl owner=\"test\"><group name=\"testgroup\" role=\"member\"><grant access=\"read\" /></group><user name=\"@owner\"><grant access=\"all\" /></user><user name=\"test\"><grant access=\"write\" /><grant access=\"read\" /></user></acl>";
            var acl = EntityAcl.FromXml(xml);
        }

        [TestMethod]
        public void DeserializeFromXml2Test()
        {
            string xml;
            EntityAcl acl;

            xml = @"
<acl owner=""test"">
    <group name=""testgroup"" role=""member"">
        <grant access=""read"" />
    </group>
    <user name=""@owner"">
        <grant access=""all"" />
    </user>
    <user name=""test"">
        <deny access=""write"" />
        <grant access=""read"" />
    </user>
</acl>";
            acl = EntityAcl.FromXml(xml);

            Assert.AreEqual("test", acl.Owner);
            Assert.AreEqual(4, acl.Count);
        }
    }
}
