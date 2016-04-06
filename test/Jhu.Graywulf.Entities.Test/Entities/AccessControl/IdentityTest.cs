using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities.AccessControl
{
    [TestClass]
    public class IdentityTest
    {
        [TestMethod]
        public void CopyTest()
        {
            var a = new Identity()
            {
                Name = "a",
                IsAuthenticated = true,
            };

            a.Roles.Add(new IdentityRole() { Group = "a", Role = "x" });
            a.Roles.Add(new IdentityRole() { Group = "b", Role = "y" });

            var b = new Identity(a);

            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.IsAuthenticated, b.IsAuthenticated);
            Assert.AreEqual(a.Roles.Count, b.Roles.Count);

            b = (Identity)a.Clone();

            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.IsAuthenticated, b.IsAuthenticated);
            Assert.AreEqual(a.Roles.Count, b.Roles.Count);
        }
    }
}
