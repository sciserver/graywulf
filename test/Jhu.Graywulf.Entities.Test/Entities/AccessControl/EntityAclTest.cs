using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities.AccessControl
{
    [TestClass]
    public class EntityAclTest
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

            var id = new Identity()
            {
                IsAuthenticated = true,
                Name = "test"
            };

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

            var id = new Identity()
            {
                IsAuthenticated = true,
                Name = "test"
            };

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

            var id = new Identity()
            {
                IsAuthenticated = true,
                Name = "test"
            };

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

            var id = new Identity()
            {
                IsAuthenticated = false,
            };

            var access = acl.EvaluateAccess(id);

            Assert.AreEqual(AccessType.Grant, access["read"]);
            Assert.AreEqual(AccessType.Deny, access["write"]);
        }

        [TestMethod]
        public void EvaluateGroupPermissionTest()
        {
            var acl = new EntityAcl();

            acl.Grant("testgroup", "member", "read");
            acl.Deny("testgroup", "member", "write");

            var id = new Identity()
            {
                IsAuthenticated = true,
                Name = "test",
            };

            id.Roles.Add("testgroup", "member");

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

            var id = new Identity()
            {
                IsAuthenticated = true,
                Name = "test",
            };

            id.Roles.Add("testgroup", "member");

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
            string xml;
            EntityAcl acl;

            xml = "<acl owner=\"test\"><group name=\"testgroup\" role=\"member\"><grant access=\"read\" /></group><user name=\"@owner\"><grant access=\"all\" /></user><user name=\"test\"><grant access=\"write\" /><grant access=\"read\" /></user></acl>";
            acl = EntityAcl.FromXml(xml);
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
        }
    }
}
