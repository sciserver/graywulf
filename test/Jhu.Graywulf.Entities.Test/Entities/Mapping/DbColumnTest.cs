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
        class ColumnsTestEntity : Entity
        {
            [DbColumn(Binding = DbColumnBinding.Key | DbColumnBinding.Identity, DefaultValue = -1)]
            public int ID { get; set; }

            [DbColumn]
            public string Name { get; set; }

            [DbColumn(Name = "SomethingElse")]
            public string Rename { get; set; }

            [DbColumn(Order = 8)]
            public string Eight { get; set; }

            [DbColumn(Type = SqlDbType.VarChar)]
            public string AnsiText { get; set; }

            [DbColumn(Size = 10)]
            public string VarCharText { get; set; }

            [DbColumn]
            public SByte SByte { get; set; }

            [DbColumn]
            public Int16 Int16 { get; set; }

            [DbColumn]
            public Int32 Int32 { get; set; }

            [DbColumn]
            public Int64 Int64 { get; set; }

            [DbColumn]
            public Byte Byte { get; set; }

            [DbColumn]
            public UInt16 UInt16 { get; set; }

            [DbColumn]
            public UInt32 UInt32 { get; set; }

            [DbColumn]
            public UInt64 UInt64 { get; set; }

            [DbColumn]
            public Single Single { get; set; }

            [DbColumn]
            public Double Double { get; set; }

            [DbColumn]
            public Decimal Decimal { get; set; }

            [DbColumn]
            public String String { get; set; }

            [DbColumn]
            public byte[] ByteArray { get; set; }

            [DbColumn]
            public DateTime DateTime { get; set; }

            [DbColumn]
            public Guid Guid { get; set; }

            [DbColumn]
            public XmlElement XmlElement { get; set; }
        }

        [DbTable]
        class InvalidColumnTypeTestEntity : Entity
        {
            [DbColumn(Binding = DbColumnBinding.Key | DbColumnBinding.Identity, DefaultValue = -1)]
            public int ID { get; set; }

            [DbColumn]
            public object Name { get; set; }
        }

        [TestMethod]
        public void DbColumnPropertiesTest()
        {
            var t = DbTable.GetDbTable(typeof(ColumnsTestEntity));

            Assert.AreEqual("ColumnsTestEntity", t.Name);
            Assert.AreEqual(DbColumnBinding.Key | DbColumnBinding.Identity,  t.Columns["ID"].Binding);
            Assert.AreEqual(-1, t.Columns["ID"].DefaultValue);
            Assert.AreEqual("ID", t.Columns["ID"].Name);

            Assert.AreEqual("Name", t.Columns["Name"].Name);
            Assert.AreEqual(DbColumnBinding.Column, t.Columns["Name"].Binding);
            Assert.IsNull(t.Columns["Name"].DefaultValue);
            Assert.IsFalse(t.Columns["Name"].Order.HasValue);
            Assert.AreEqual(SqlDbType.NVarChar, t.Columns["Name"].Type);
            Assert.IsFalse(t.Columns["Name"].Size.HasValue);

            Assert.AreEqual("SomethingElse", t.Columns["Rename"].Name);

            Assert.AreEqual(8, t.Columns["Eight"].Order.Value);

            Assert.AreEqual(SqlDbType.VarChar, t.Columns["AnsiText"].Type);

            Assert.AreEqual(10, t.Columns["VarCharText"].Size.Value);
        }

        [TestMethod]
        public void GetKeyTest()
        {
            var e = new ColumnsTestEntity();

            Assert.AreEqual(0, e.GetKey());
        }

        [TestMethod]
        public void SetKeyTest()
        {
            var e = new ColumnsTestEntity();

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
