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
    public class AuxColumnEntityTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();
        }

        private AuxColumnEntity CreateEntity(Context context)
        {
            var e = new AuxColumnEntity(context)
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
                var e = new AuxColumnEntity(context);
                e.ID = id;
                e.Load();

                Assert.IsTrue(e.IsLoaded);
                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
                Assert.AreEqual("test", e.Name);
                Assert.AreEqual("the value", e.SomeValue);
            }
        }

        [TestMethod]
        public void ModifyEntity()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Save();

                id = e.ID;

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
                var e = new AuxColumnEntity(context);
                e.ID = id;
                e.Delete();
            }
        }
    }
}
