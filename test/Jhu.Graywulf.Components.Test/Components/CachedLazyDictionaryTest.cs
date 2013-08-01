using Jhu.Graywulf.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Jhu.Graywulf.Components.Test
{

    [TestClass]
    public class CachedLazyDictionaryTest : CachedLazyDictionaryTestBase<CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper>, GenericParameterHelper, CacheableParameterHelper>
    {
        protected override CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper> CreateTarget()
        {
            // Create class with cache
            Cache<GenericParameterHelper, CacheableParameterHelper> cache = new Cache<GenericParameterHelper, CacheableParameterHelper>();
            return new CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper>(cache);
        }

        protected override CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper> CreateTarget(Cache<GenericParameterHelper, CacheableParameterHelper> cache)
        {
            return new CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper>(cache);
        }
    }

    [TestClass]
    public class CachedLazyDictionaryNoCacheTest : CachedLazyDictionaryTestBase<CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper>, GenericParameterHelper, CacheableParameterHelper>
    {
        protected override CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper> CreateTarget()
        {
            // Create class with no cache
            return new CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper>();
        }

        protected override CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper> CreateTarget(Cache<GenericParameterHelper, CacheableParameterHelper> cache)
        {
            return new CachedLazyDictionary<GenericParameterHelper, CacheableParameterHelper>(cache);
        }
    }

    /// <summary>
    ///This is a test class for CachedLazyDictionaryTest and is intended
    ///to contain all CachedLazyDictionaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public abstract class CachedLazyDictionaryTestBase<TDictionary, TKey, TValue> : LazyDictionaryTestBase<TDictionary, TKey, TValue>
        where TDictionary : CachedLazyDictionary<TKey, TValue>, new()
        where TKey : new()
        where TValue : class, ICacheable, new()
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        protected abstract TDictionary CreateTarget(Cache<TKey, TValue> cache);

        /// <summary>
        ///A test for OnItemLoading
        ///</summary>
        public void OnItemLoadingTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();
            TValue v4 = new TValue();
            TValue v;

            // Init cache
            Cache<TKey, TValue> cache = new Cache<TKey, TValue>();
            TDictionary target = CreateTarget(cache);

            // See if auto-loaded items show up in cache
            target.ItemLoading += (o, e) =>
                {
                    CacheableParameterHelper cc = (CacheableParameterHelper)(object)v1;
                    cc.IsCacheable = true;
                    e.Value = v1;
                    e.IsFound = true;
                };

            v = target[k1];
            Assert.AreEqual(v, v1);
            Assert.AreEqual(cache[k1], v1);

            // Make sure non-cacheable items don't show up in the cache
            cache = new Cache<TKey, TValue>();
            target = CreateTarget(cache);
            
            target.ItemLoading += (o, e) =>
            {
                CacheableParameterHelper cc = (CacheableParameterHelper)(object)v1;
                cc.IsCacheable = false;
                e.Value = v1;
                e.IsFound = true;
            };

            v = target[k1];
            Assert.AreEqual(v, v1);
            Assert.IsFalse(cache.ContainsKey(k1));
        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void OnItemLoadingTest()
        {
            OnItemLoadingTestHelper();
        }

        /// <summary>
        ///A test for OnItemAdded
        ///</summary>
        public void OnItemAddedTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();
            TValue v4 = new TValue();
            TValue v;

            // Init cache
            Cache<TKey, TValue> cache = new Cache<TKey, TValue>();
            TDictionary target = CreateTarget(cache);

            // Add item directly to the cache and see if is returned
            cache[k1] = v1;
            Assert.AreEqual(target[k1], v1);

            // Not even in the cache
            try
            {
                v = target[k2];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }

            // Put something into the cache
            var cc = (CacheableParameterHelper)(object)v2;
            cc.IsCacheable = true;
            target[k2] = v2;

            Assert.AreEqual(cache[k2], v2);
            Assert.AreEqual(target[k2], v2);

            // Upgrade value in the cache
            cc = (CacheableParameterHelper)(object)v3;
            cc.IsCacheable = true;
            target[k2] = v3;

            Assert.AreEqual(cache[k2], v3);

            // Insert something that is not cacheable
            target[k2] = v4;

            Assert.AreEqual(cache[k2], v3);
            Assert.AreNotEqual(cache[k2], v4);
        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void OnItemAddedTest()
        {
            OnItemLoadingTestHelper();
        }

        /// <summary>
        ///A test for OnItemUpdated
        ///</summary>
        public void OnItemUpdatedTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();
            TValue v4 = new TValue();

            Cache<TKey, TValue> cache = new Cache<TKey, TValue>();
            TDictionary target = CreateTarget(cache);

            var cc = (CacheableParameterHelper)(object)v1;
            cc.IsCacheable = true;

            cc = (CacheableParameterHelper)(object)v2;
            cc.IsCacheable = true;

            target[k1] = v1;
            Assert.AreEqual(cache[k1], v1);

            target[k1] = v2;
            Assert.AreEqual(cache[k1], v2);

            target[k1] = v3;
            Assert.AreNotEqual(cache[k1], v3);
        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void OnItemUpdatedTest()
        {
            OnItemUpdatedTestHelper();
        }
    }
}
