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
    public class EnumEntityTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();
        }

        private EnumEntity CreateEntity(Context context)
        {
            var e = new EnumEntity(context)
            {
                Name = "test",
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
                var e = new EnumEntity(context);
                e.ID = id;
                e.Load();

                Assert.IsTrue(e.IsLoaded);
                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
                Assert.AreEqual("test", e.Name);
            }
        }

        [TestMethod]
        public void ModifyEntity()
        {
            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Save();

                int id = e.ID;

                e.Name = "modified";
                e.Save();

                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
            }
        }

        [TestMethod]
        public void DeleteEntity()
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
                var e = new EnumEntity(context);
                e.ID = id;
                e.Delete();
            }
        }
    }
}
