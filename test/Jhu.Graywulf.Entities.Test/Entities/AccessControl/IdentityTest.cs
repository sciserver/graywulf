using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities.AccessControl
{
    [TestClass]
    public class IdentityTest
    {
        private Identity CreateIdentity()
        {
            var id = new Identity()
            {
                Name = "a",
                IsAuthenticated = true,
            };

            id.Roles.Add(new IdentityRole() { Group = "a", Role = "x" });
            id.Roles.Add(new IdentityRole() { Group = "b", Role = "y" });

            return id;
        }

        [TestMethod]
        public void CopyTest()
        {
            var a = CreateIdentity();
            var b = new Identity(a);

            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.IsAuthenticated, b.IsAuthenticated);
            Assert.AreEqual(a.Roles.Count, b.Roles.Count);

            b = (Identity)a.Clone();

            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.IsAuthenticated, b.IsAuthenticated);
            Assert.AreEqual(a.Roles.Count, b.Roles.Count);
        }

        [TestMethod]
        public void SerializeToXmlTest()
        {
            var id = CreateIdentity();

            var res = id.ToXml();

            Assert.AreEqual(
                @"<id name=""a"" auth=""1""><group name=""a"" role=""x"" /><group name=""b"" role=""y"" /></id>",
                res);
        }

        [TestMethod]
        public void DeserializeFromXmlTest()
        {
            var xml = @"<id name=""a"" auth=""1""><group name=""a"" role=""x"" /><group name=""b"" role=""y"" /></id>";
            var id = Identity.FromXml(xml);

            Assert.AreEqual("a", id.Name);
            Assert.AreEqual(2, id.Roles.Count);
        }

        [TestMethod]
        public void DeserializeFromXmlTest2()
        {
            var xml = @"<id name=""a"" auth=""1""></id>";
            var id = Identity.FromXml(xml);

            Assert.AreEqual("a", id.Name);
            Assert.AreEqual(0, id.Roles.Count);
        }

        [TestMethod]
        public void DeserializeFromXmlTest3()
        {
            var xml = @"<id name=""a"" auth=""1"" />";
            var id = Identity.FromXml(xml);

            Assert.AreEqual("a", id.Name);
            Assert.AreEqual(0, id.Roles.Count);
        }
    }
}
