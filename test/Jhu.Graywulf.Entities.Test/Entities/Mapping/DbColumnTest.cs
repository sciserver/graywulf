using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities.Mapping
{
    [TestClass]
    public class DbColumnTest
    {
        [DbTable]
        class InvalidColumnTypeTestEntity : Entity
        {
            [DbColumn(Binding = DbColumnBinding.Key | DbColumnBinding.Identity)]
            public int ID { get; set; }

            [DbColumn]
            public object Name { get; set; }
        }

        [TestMethod]
        public void DbColumnPropertiesTest()
        {
            var t = DbTable.GetDbTable(typeof(EntityWithIdentityKey));

            Assert.AreEqual(typeof(EntityWithIdentityKey).Name, t.Name);
            Assert.AreEqual(DbColumnBinding.Key | DbColumnBinding.Identity, t.Columns["ID"].Binding);
            Assert.AreEqual(0, t.Columns["ID"].DefaultValue);
            Assert.AreEqual("ID", t.Columns["ID"].Name);

            Assert.AreEqual("Name", t.Columns["Name"].Name);
            Assert.AreEqual(DbColumnBinding.Column, t.Columns["Name"].Binding);
            Assert.IsNull(t.Columns["Name"].DefaultValue);
            Assert.IsFalse(t.Columns["Name"].Order.HasValue);
            Assert.AreEqual(SqlDbType.NVarChar, t.Columns["Name"].DbType);
            Assert.IsFalse(t.Columns["Name"].Size.HasValue);

            Assert.AreEqual("SomethingElse", t.Columns["Rename"].Name);

            Assert.AreEqual(4, t.Columns["Four"].Order.Value);

            Assert.AreEqual(SqlDbType.VarChar, t.Columns["AnsiText"].DbType);

            Assert.AreEqual(10, t.Columns["VarCharText"].Size.Value);

            Assert.AreEqual(23, t.Columns.Count);
        }

        [TestMethod]
        public void GetKeyTest()
        {
            var e = new EntityWithIdentityKey();

            Assert.AreEqual(0, e.GetKey());
        }

        [TestMethod]
        public void SetKeyTest()
        {
            var e = new EntityWithIdentityKey();

            e.SetKey(10);

            Assert.AreEqual(10, e.GetKey());
            Assert.IsTrue(e.IsDirty);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public void InvalidColumnTypeTest()
        {
            var t = DbTable.GetDbTable(typeof(InvalidColumnTypeTestEntity));
        }
    }
}
