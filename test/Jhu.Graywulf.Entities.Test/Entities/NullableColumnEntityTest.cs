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
    public class NullableColumnEntityTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();
        }

        protected NullableColumnEntity CreateEntity(Context context)
        {
            var e = new NullableColumnEntity(context)
            {
                ID = 0,
                Name = "test",
                Int32 = 9,
                Double = 10,
                ByteArray = new byte[13],
                DateTime = DateTime.Now,
                Guid = new Guid(),
            };

            return e;
        }

        protected NullableColumnEntity CreateNullEntity(Context context)
        {
            var e = new NullableColumnEntity(context)
            {
                ID = 0,
                Name = null,
                Int32 = null,
                Double = null,
                ByteArray = null,
                DateTime = null,
                Guid = null,
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
                var e = new NullableColumnEntity(context);
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
                var e = new NullableColumnEntity(context);
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
                var e = new NullableColumnEntity(context);
                e.ID = id;
                e.Delete();
            }
        }
    }
}
