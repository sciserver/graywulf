using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    [TestClass]
    public class IdentityKeyEntityTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();
        }

        protected IdentityKeyEntity CreateEntity(Context context)
        {
            var xml = new XmlDocument();
            var xn = xml.CreateElement("test");
            xml.AppendChild(xn);

            var e = new IdentityKeyEntity(context)
            {
                ID = 0,
                Name = "test",
                Rename = "test",
                Four = "four",
                AnsiText = "testtest",
                VarCharText = "moretest",
                SByte = 1,
                Int16 = 2,
                Int32 = 3,
                Int64 = 4,
                Byte = 5,
                UInt16 = 6,
                UInt32 = 7,
                UInt64 = 8,
                Single = 9,
                Double = 10,
                Decimal = 11,
                String = "twelve",
                ByteArray = new byte[13],
                DateTime = DateTime.Now,
                Guid = new Guid(),
                XmlElement = xml.DocumentElement,
            };

            return e;
        }

        [TestMethod]
        public void CreateTest()
        {
            using (var context = CreateContext())
            {
                var e = CreateEntity(context);

                e.Save();

                Assert.IsFalse(e.IsDirty);
                Assert.IsTrue(e.ID > 0);
            }
        }

        [TestMethod]
        public void LoadTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);

                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                var e = new IdentityKeyEntity(context);
                e.ID = id;
                e.Load();

                Assert.IsTrue(e.IsLoaded);
                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
                Assert.AreEqual("test", e.Name);
            }
        }

        [TestMethod]
        public void ModifyTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                var e = new IdentityKeyEntity(context);
                e.ID = id;
                e.Load();

                e.Name = "modified";
                e.Save();

                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
            }
        }

        [TestMethod]
        public void DeleteTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);

                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                var e = new IdentityKeyEntity(context);
                e.ID = id;
                e.Delete();
            }
        }
    }
}
