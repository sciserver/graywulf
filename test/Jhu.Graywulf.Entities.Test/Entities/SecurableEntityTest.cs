using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    [TestClass]
    public class SecurableEntityTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();
        }

        [DbTable]
        public class SecuredEntity : SecurableEntity
        {
            [DbColumn(Binding = DbColumnBinding.Key)]
            public int ID { get; set; }

            [DbColumn]
            public string Name { get; set; }

            public SecuredEntity()
            {
            }

            public SecuredEntity(Context context)
                : base(context)
            {
            }
        }

        public SecuredEntity CreateEntity(Context context)
        {
            var e = new SecuredEntity(context)
            {
                Name = "test"
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
                Assert.AreEqual(context.Identity.Name, e.Permissions.Owner);
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
                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();

                Assert.IsTrue(e.IsLoaded);
                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
                Assert.AreEqual("test", e.Name);
                Assert.AreEqual(context.Identity.Name, e.Permissions.Owner);
            }
        }
    }
}
