using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.AccessControl
{
    [TestClass]
    public class UserAceTest
    {
        [TestMethod]
        public void CompareToTest()
        {
            var a = new UserAce()
            {
                Name = "a"
            };

            var b = new UserAce()
            {
                Name = "b"
            };

            Assert.AreEqual(0, a.CompareTo(a));
            Assert.AreEqual(-1, a.CompareTo(b));
            Assert.AreEqual(1, b.CompareTo(a));
        }

        [TestMethod]
        public void CompareToGroupAceTest()
        {
            var a = new UserAce()
            {
                Name = "a"
            };

            var b = new GroupAce()
            {
                Name = "b",
                Role = "x",
            };

            Assert.AreEqual(1, a.CompareTo(b));
        }

        [TestMethod]
        public void CopyTest()
        {
            var a = new UserAce()
            {
                Name = "a",
                Access = "a",
                Type = AccessType.Grant
            };

            var b = new UserAce(a);

            Assert.AreEqual(b.Name, a.Name);
            Assert.AreEqual(b.Access, a.Access);
            Assert.AreEqual(b.Type, a.Type);

            b = (UserAce)a.Clone();

            Assert.AreEqual(b.Name, a.Name);
            Assert.AreEqual(b.Access, a.Access);
            Assert.AreEqual(b.Type, a.Type);
        }
    }
}
