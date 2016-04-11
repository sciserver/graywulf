using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities
{
    
    [TestClass]
    public class EntitySearchTest : EntityWithIdentityKeyTest
    {
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
                var e = CreateEntity(context);
                e.Save();
            }

            using (var context = CreateContext())
            {
                var s = new EntityWithIdentityKeySearch(context);

                s.Name = "tes%";

                var cnt = s.Count();

                Assert.AreEqual(1, cnt);
            }
        }
    }
}
