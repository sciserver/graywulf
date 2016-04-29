using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities
{
    [TestClass]
    public class SecurableEntitySearchTest : TestClassBase
    {
        protected static SecuredEntity CreateEntity(Context context, string name)
        {
            var e = new SecuredEntity(context)
            {
                ID = 0,
                Name = name,
            };

            e.Save();

            return e;
        }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();
        }

        [TestMethod]
        public void CountTest()
        {
            using (var context = CreateContext())
            {
                CreateEntity(context, "CountTest");

                var s = new SecuredEntitySearch(context);

                s.Name = "CountTest";

                var cnt = s.Count();

                Assert.AreEqual(1, cnt);
            }
        }

        [TestMethod]
        public void CountDeniedTest()
        {
            using (var context = CreateContext())
            {
                CreateEntity(context, "CountDeniedTest");

                context.Principal = CreateAnonPrincipal();

                var s = new SecuredEntitySearch(context);

                s.Name = "CountDeniedTest";

                var cnt = s.Count();

                Assert.AreEqual(0, cnt);
            }
        }

        [TestMethod]
        public void FindTest()
        {
            using (var context = CreateContext())
            {
                CreateEntity(context, "FindTest");

                var s = new SecuredEntitySearch(context);

                s.Name = "FindTest";

                var cnt = s.Find().Count();

                Assert.AreEqual(1, cnt);
            }
        }

        [TestMethod]
        public void FindDeniedTest()
        {
            using (var context = CreateContext())
            {
                CreateEntity(context, "FindDeniedTest");

                context.Principal = CreateAnonPrincipal();

                var s = new SecuredEntitySearch(context);

                s.Name = "FindDeniedTest";

                var cnt = s.Find().Count();

                Assert.AreEqual(0, cnt);
            }
        }
    }
}
