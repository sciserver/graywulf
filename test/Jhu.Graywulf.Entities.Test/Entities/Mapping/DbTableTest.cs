using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities.Mapping
{
    [TestClass]
    public class DbTableTest
    {
        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public void NoDbTableAttributeTest()
        {
            var t = DbTable.GetDbTable(typeof(TestEntity));
        }

        [TestMethod]
        public void GetDbTableTest()
        {
            var t = DbTable.GetDbTable(typeof(TestEntity));

            Assert.AreEqual("TestEntity", t.Name);
        }

        [TestMethod]
        public void GetKeyTest()
        {
            var e = new TestEntity();

            Assert.AreEqual(-1L, e.GetKey());
        }
    }
}
