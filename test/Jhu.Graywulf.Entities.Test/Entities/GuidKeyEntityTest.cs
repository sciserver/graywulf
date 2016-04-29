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
    public class GuidKeyEntityTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();
        }

        private GuidKeyEntity CreateEntity(Context context)
        {
            var e = new GuidKeyEntity(context)
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
            Guid guid;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);

                e.Save();

                guid = e.Guid;
            }

            using (var context = CreateContext())
            {
                var e = new GuidKeyEntity(context);
                e.Guid = guid;
                e.Load();

                Assert.IsTrue(e.IsLoaded);
                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(guid, e.Guid);
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

                Guid guid = e.Guid;

                e.Name = "modified";
                e.Save();

                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(guid, e.Guid);
            }
        }

        [TestMethod]
        public void DeleteEntity()
        {
            Guid guid;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);

                e.Save();

                guid = e.Guid;
            }

            using (var context = CreateContext())
            {
                var e = new GuidKeyEntity(context);
                e.Guid = guid;
                e.Delete();
            }
        }
    }
}
