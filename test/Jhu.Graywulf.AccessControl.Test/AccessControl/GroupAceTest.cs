using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.AccessControl
{
    [TestClass]
    public class GroupAceTest
    {
        [TestMethod]
        public void CompareToTest1()
        {
            var a = new GroupAce()
            {
                Name = "a",
                Role = "x",
            };

            var b = new GroupAce()
            {
                Name = "b",
                Role = "x",
            };

            Assert.AreEqual(0, a.CompareTo(a));
            Assert.AreEqual(-1, a.CompareTo(b));
            Assert.AreEqual(1, b.CompareTo(a));
        }

        [TestMethod]
        public void CompareToTest2()
        {
            var a = new GroupAce()
            {
                Name = "a",
                Role = "a"
            };

            var b = new GroupAce()
            {
                Name = "a",
                Role = "b"
            };

            Assert.AreEqual(0, a.CompareTo(a));
            Assert.AreEqual(0, b.CompareTo(b));
            Assert.AreEqual(-1, a.CompareTo(b));
            Assert.AreEqual(1, b.CompareTo(a));
        }

        [TestMethod]
        public void CompareToUserAceTest()
        {
            var a = new GroupAce()
            {
                Name = "a",
                Role = "x",
            };

            var b = new UserAce()
            {
                Name = "b"
            };

            Assert.AreEqual(-1, a.CompareTo(b));
        }

        [TestMethod]
        public void CopyTest()
        {
            var a = new GroupAce()
            {
                Name = "n",
                Access = "a",
                Role = "r",
                Type = AccessType.Grant
            };

            var b = new GroupAce(a);

            Assert.AreEqual(b.Name, a.Name);
            Assert.AreEqual(b.Access, a.Access);
            Assert.AreEqual(b.Role, a.Role);
            Assert.AreEqual(b.Type, a.Type);

            b = (GroupAce)a.Clone();

            Assert.AreEqual(b.Name, a.Name);
            Assert.AreEqual(b.Access, a.Access);
            Assert.AreEqual(b.Role, a.Role);
            Assert.AreEqual(b.Type, a.Type);
        }
    }
}
