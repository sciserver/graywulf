using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities
{
    [TestClass]
    public class SecurableEntitySearchTest : TestClassBase
    {
        protected static SecuredEntity CreateEntity(Context context)
        {
            var e = new SecuredEntity(context)
            {
                ID = 0,
                Name = "test",
            };

            return e;
        }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Save();
            }
        }

        [TestMethod]
        public void CountTest()
        {
            using (var context = CreateContext())
            {
                var s = new SecuredEntitySearch(context);

                s.Name = "tes%";

                var cnt = s.Count();

                Assert.AreEqual(1, cnt);
            }
        }

        [TestMethod]
        public void CountDeniedTest()
        {
            using (var context = CreateContext())
            {
                context.Identity = CreateAnonIdentity();

                var s = new SecuredEntitySearch(context);

                s.Name = "tes%";

                var cnt = s.Count();

                Assert.AreEqual(0, cnt);
            }
        }
    }
}
