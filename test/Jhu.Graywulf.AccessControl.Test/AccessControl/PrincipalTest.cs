using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.AccessControl
{
    [TestClass]
    public class PrincipalTest
    {
        private Principal CreatePrincipal()
        {
            var principal = new Principal()
            {
                Identity = new Identity()
                {
                    Name = "a",
                    IsAuthenticated = true,
                }
            };

            principal.AddRole("a", "x");
            principal.AddRole("b", "y");

            return principal;
        }

        [TestMethod]
        public void CopyTest()
        {
            var a = CreatePrincipal();
            var b = new Principal(a);

            Assert.AreEqual(a.Identity.Name, b.Identity.Name);
            Assert.AreEqual(a.Identity.IsAuthenticated, b.Identity.IsAuthenticated);
            Assert.AreEqual(a.RoleCount, b.RoleCount);

            b = (Principal)a.Clone();

            Assert.AreEqual(a.Identity.Name, b.Identity.Name);
            Assert.AreEqual(a.Identity.IsAuthenticated, b.Identity.IsAuthenticated);
            Assert.AreEqual(a.RoleCount, b.RoleCount);
        }

        [TestMethod]
        public void BinarySerializationTest()
        {
            var ms = new MemoryStream();
            var principal = CreatePrincipal();

            var w = new BinaryWriter(ms);
            principal.ToBinary(w);

            ms.Seek(0, SeekOrigin.Begin);

            var r = new BinaryReader(ms);
            var principal2 = Principal.FromBinary(r);

            Assert.AreEqual(principal.Identity.Name, principal2.Identity.Name);
            Assert.AreEqual(principal.Identity.IsAuthenticated, principal2.Identity.IsAuthenticated);
            Assert.AreEqual(principal.RoleCount, principal2.RoleCount);

            Assert.IsTrue(principal2.IsInRole("a", "x"));
            Assert.IsTrue(principal2.IsInRole("b", "y"));
        }

        [TestMethod]
        public void SerializeToXmlTest()
        {
            var principal = CreatePrincipal();

            var res = principal.ToXml();

            Assert.AreEqual(
                @"<id name=""a"" auth=""1""><role name=""a|x"" /><role name=""b|y"" /></id>",
                res);
        }

        [TestMethod]
        public void DeserializeFromXmlTest()
        {
            var xml = @"<id name=""a"" auth=""1""><role name=""a|x"" /><role name=""b|y"" /></id>";
            var principal = Principal.FromXml(xml);

            Assert.AreEqual("a", principal.Identity.Name);
            Assert.AreEqual(2, principal.RoleCount);
        }

        [TestMethod]
        public void DeserializeFromXmlTest2()
        {
            var xml = @"<id name=""a"" auth=""1""></id>";
            var principal = Principal.FromXml(xml);

            Assert.AreEqual("a", principal.Identity.Name);
            Assert.AreEqual(0, principal.RoleCount);
        }

        [TestMethod]
        public void DeserializeFromXmlTest3()
        {
            var xml = @"<id name=""a"" auth=""1"" />";
            var principal = Principal.FromXml(xml);

            Assert.AreEqual("a", principal.Identity.Name);
            Assert.AreEqual(0, principal.RoleCount);
        }
    }
}
