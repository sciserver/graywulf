using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities.Mapping
{
    [TestClass]
    public class DbTableTest
    {
        [DbTable]
        public class DbTableTestEntity : Entity
        {
            private long id;

            [DbColumn(Binding = DbColumnBinding.Key)]
            public long ID
            {
                get { return id; }
                set { id = value; }
            }

            public DbTableTestEntity()
            {
                InitializeMembers();
            }

            private void InitializeMembers()
            {
                this.id = -1;
            }
        }

        [TestMethod]
        public void GetDbTableTest()
        {
            var t = DbTable.GetDbTable(typeof(DbTableTestEntity));

            Assert.AreEqual("DbTableTestEntity", t.Name);
            Assert.IsTrue(t.Type == typeof(DbTableTestEntity));
        }

        class NoDbTableTestEntity : Entity
        {

        }

        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public void NoDbTableAttributeTest()
        {
            var t = DbTable.GetDbTable(typeof(NoDbTableTestEntity));
        }

        [DbTable]
        class DuplicateKeyEntity : Entity
        {
            [DbColumn(Binding = DbColumnBinding.Key)]
            public int Key1 { get; set; }

            [DbColumn(Binding = DbColumnBinding.Key)]
            public int Key2 { get; set; }
        }

        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public void DuplicateKeyTest()
        {
            var t = DbTable.GetDbTable(typeof(DuplicateKeyEntity));
        }

        #region Getter and setter casting tests

        [TestMethod]
        public void GetKeyTest()
        {
            var e = new DbTableTestEntity();

            Assert.AreEqual(-1L, e.GetKey());
        }

        [TestMethod]
        public void SetKeyTest()
        {
            var e = new DbTableTestEntity();

            e.SetKey(2L);

            Assert.AreEqual(2L, e.GetKey());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void SetKeyNoCastTest()
        {
            // TODO: setcolumn internally doesn't do unboxing before
            // convert to column type. A wrong type passed to set key
            // results in an InvalidCastExpression

            var e = new DbTableTestEntity();

            e.SetKey(2);
        }


        #endregion
    }
}
