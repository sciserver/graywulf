using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Components
{
    [TestClass]
    public class CacheTest
    {
        [TestMethod]
        public void TestCache()
        {
            var cache = new Cache<string, int>();
            cache.CollectionInterval = new TimeSpan(0, 0, 3);

            cache.TryAdd("one", 1, TimeSpan.FromSeconds(1));
            cache.TryAdd("two", 1, TimeSpan.FromMinutes(5));

            Assert.AreEqual(2, cache.Count);

            System.Threading.Thread.Sleep(new TimeSpan(0, 0, 5));

            Assert.AreEqual(1, cache.Count);
        }
    }
}
