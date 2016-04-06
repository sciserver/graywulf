using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities.AccessControl
{
    [TestClass]
    public class IdentityRoleTest
    {
        [TestMethod]
        public void CopyTest()
        {
            var a = new IdentityRole()
            {
                Group = "a",
                Role = "r"
            };

            var b = new IdentityRole(a);

            Assert.AreEqual(a.Group, b.Group);
            Assert.AreEqual(a.Role, b.Role);

            b = (IdentityRole)a.Clone();

            Assert.AreEqual(a.Group, b.Group);
            Assert.AreEqual(a.Role, b.Role);
        }
    }
}
