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
            }
        }
    }
}
