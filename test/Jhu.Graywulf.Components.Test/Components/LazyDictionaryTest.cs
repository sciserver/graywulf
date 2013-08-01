using Jhu.Graywulf.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Jhu.Graywulf.Components.Test
{

    [TestClass]
    public class LazyDictionaryTest : LazyDictionaryTestBase<LazyDictionary<GenericParameterHelper, GenericParameterHelper>, GenericParameterHelper, GenericParameterHelper>
    {
    }

    /// <summary>
    ///This is a test class for TDictionaryTest and is intended
    ///to contain all TDictionaryTest Unit Tests
    ///</summary>
    [TestClass]
    public abstract class LazyDictionaryTestBase<TDictionary, TKey, TValue>
        where TDictionary : LazyDictionary<TKey, TValue>, new()
        where TKey : new()
        where TValue : new()
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

        protected virtual TDictionary CreateTarget()
        {
            return new TDictionary();
        }

        /// <summary>
        ///A test for TDictionary`2 Constructor
        ///</summary>
        public void TDictionaryConstructorTestHelper()
        {
            IEqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;
            LazyDictionary<TKey, TValue> target = new LazyDictionary<TKey, TValue>(comparer);

            Assert.IsTrue(target.IsEmpty);
        }

        [TestMethod()]
        public void TDictionaryConstructorTest()
        {
            TDictionaryConstructorTestHelper();
        }

        /// <summary>
        ///A test for TDictionary`2 Constructor
        ///</summary>
        public void TDictionaryConstructorTest1Helper()
        {
            TDictionary target = CreateTarget();

            Assert.IsTrue(target.IsEmpty);
        }

        [TestMethod()]
        public void TDictionaryConstructorTest1()
        {
            TDictionaryConstructorTest1Helper();
        }

        /// <summary>
        ///A test for AddOrUpdate
        ///</summary>
        public void AddOrUpdateTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();

            // Create and empty collection

            TDictionary target = CreateTarget();
            Assert.IsTrue(target.IsEmpty);

            // Try to add and update with null key
            try
            {
                target.AddOrUpdate(default(TKey), v1, (key, value) => v1);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }

            // Try to update with null factory
            try
            {
                target.AddOrUpdate(k1, v1, null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }

            // Make sure factory is not called if key is not found
            target.AddOrUpdate(k1, v1, (key, value) =>
                {
                    Assert.Fail();
                    return default(TValue);
                });

            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);
            Assert.AreEqual(target[k1], v1);

            // Add another with different key
            // Make sure factory is not called if key is not found
            target.AddOrUpdate(k2, v2, (key, value) =>
            {
                Assert.Fail();
                return default(TValue);
            });

            Assert.IsTrue(target.Count == 2);
            Assert.AreEqual(target[k2], v2);


            // Now try to update the first one with key already existing
            target.AddOrUpdate(k1, v3, (key, value) => v3);

            Assert.IsTrue(target.Count == 2);
            Assert.AreNotEqual(target[k1], v1);
            Assert.AreEqual(target[k1], v3);
            Assert.AreEqual(target[k2], v2);
        }

        [TestMethod()]
        public void AddOrUpdateTest()
        {
            AddOrUpdateTestHelper();
        }

        /// <summary>
        ///A test for AddOrUpdate
        ///</summary>
        public void AddOrUpdateTest1Helper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();

            // Create and empty collection

            TDictionary target = CreateTarget();
            Assert.IsTrue(target.IsEmpty);

            // Try to add and update with null key
            try
            {
                target.AddOrUpdate(default(TKey), (key) => v1, (key, value) => v1);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }

            // Try to update with null factory
            try
            {
                target.AddOrUpdate(k1, null, (key, value) => v1);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }

            // Try to update with null factory
            try
            {
                target.AddOrUpdate(k1, (key) => v1, null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }

            // Make sure factory is not called if key is not found
            target.AddOrUpdate(k1, (key) => v1, (TKey key, TValue value) =>
            {
                Assert.Fail();
                return default(TValue);
            });

            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);
            Assert.AreEqual(target[k1], v1);

            // Add another with different key
            // Make sure factory is not called if key is not found
            target.AddOrUpdate(k2, (key) => v2, (TKey key, TValue value) =>
            {
                Assert.Fail();
                return default(TValue);
            });

            Assert.IsTrue(target.Count == 2);
            Assert.AreEqual(target[k2], v2);


            // Now try to update the first one with key already existing
            target.AddOrUpdate(k1, (key) => v3, (TKey key, TValue value) => v3);

            Assert.IsTrue(target.Count == 2);
            Assert.AreNotEqual(target[k1], v1);
            Assert.AreEqual(target[k1], v3);
            Assert.AreEqual(target[k2], v2);
        }

        [TestMethod()]
        public void AddOrUpdateTest1()
        {
            AddOrUpdateTest1Helper();
        }

        /// <summary>
        ///A test for Clear
        ///</summary>
        public void ClearTestHelper()
        {
            TKey k1 = new TKey();
            TValue v1 = new TValue();

            TDictionary target = CreateTarget();
            target[k1] = v1;

            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);
            Assert.IsTrue(target.ContainsKey(k1));

            target.Clear();

            Assert.IsTrue(target.IsEmpty);
            Assert.IsTrue(target.Count == 0);
            Assert.IsFalse(target.ContainsKey(k1));

            try
            {
                TValue v = target[k1];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }

        }

        [TestMethod()]
        public void ClearTest()
        {
            ClearTestHelper();
        }

        /// <summary>
        ///A test for ContainsKey
        ///</summary>
        public void ContainsKeyTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();

            TDictionary target = CreateTarget();

            try
            {
                target.ContainsKey(default(TKey));
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }

            Assert.IsFalse(target.ContainsKey(k1));

            target[k1] = v1;

            Assert.IsTrue(target.ContainsKey(k1));

            target.TryRemove(k1, out v1);

            Assert.IsFalse(target.ContainsKey(k1));

            // Try autoload
            target.ItemLoading += (sender, e) =>
                {
                    Assert.AreEqual(e.Key, k2);
                    e.IsFound = true;
                    e.Value = v2;
                };

            Assert.IsTrue(target.ContainsKey(k2));
            Assert.AreEqual(v2, target[k2]);
        }

        [TestMethod()]
        public void ContainsKeyTest()
        {
            ContainsKeyTestHelper();
        }

        /// <summary>
        ///A test for GetEnumerator
        ///</summary>
        public void GetEnumeratorTestHelper()
        {
            TDictionary target = CreateTarget();
            Assert.IsNotNull(target.GetEnumerator());
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            GetEnumeratorTestHelper();
        }

        /// <summary>
        ///A test for GetOrAdd
        ///</summary>
        public void GetOrAddTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v;

            TDictionary target = CreateTarget();

            // Try simple add
            v = target.GetOrAdd(k1, v1);
            Assert.AreEqual(v, v1);
            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);
            Assert.IsTrue(target.ContainsKey(k1));

            // Try add when already exists
            v = target.GetOrAdd(k1, v2);
            Assert.AreNotEqual(v, v2);
            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);
            Assert.IsTrue(target.ContainsKey(k1));

            // Try lazy loading
            target.ItemLoading += (sender, e) =>
            {
                Assert.AreEqual(e.Key, k2);
                e.IsFound = true;
                e.Value = v2;
            };

            v = target.GetOrAdd(k2, v1);

            Assert.IsTrue(target.ContainsKey(k2));
            Assert.AreEqual(v2, target[k2]);
            Assert.AreEqual(v, v2);
            Assert.AreNotEqual(v, v1);

            target.Clear();

            // Try failing loading
            target = CreateTarget();
            target.ItemLoading += (sender, e) =>
            {
                Assert.AreEqual(e.Key, k1);
                e.IsFound = false;
                e.Value = v2;
            };

            v = target.GetOrAdd(k1, v1);

            Assert.IsTrue(target.ContainsKey(k1));
            Assert.AreEqual(v1, target[k1]);
            Assert.AreEqual(v, v1);
        }

        [TestMethod()]
        public void GetOrAddTest()
        {
            GetOrAddTestHelper();
        }

        /// <summary>
        ///A test for GetOrAdd
        ///</summary>
        public void GetOrAddTest1Helper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v;

            TDictionary target = CreateTarget();

            // Try simple add
            v = target.GetOrAdd(k1, _ => v1);
            Assert.AreEqual(v, v1);
            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);
            Assert.IsTrue(target.ContainsKey(k1));

            // Try add when already in exists
            v = target.GetOrAdd(k1, _ =>
                {
                    // Make sure it's not called when key is found
                    Assert.Fail();
                    return v2;
                });
            Assert.AreNotEqual(v, v2);
            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);
            Assert.IsTrue(target.ContainsKey(k1));

            // Lazy loading tested in GetOrAddTestHelper
        }

        [TestMethod()]
        public void GetOrAddTest1()
        {
            GetOrAddTest1Helper();
        }

        /*
        /// <summary>
        ///A test for LoadAll
        ///</summary>
        public void LoadAllTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TKey k3 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();
            var kvp1 = new KeyValuePair<TKey, TValue>(k1, v1);
            var kvp2 = new KeyValuePair<TKey, TValue>(k2, v2);

            TDictionary target = CreateTarget();
            Assert.IsFalse(target.IsAllLoaded);

            // Try event handler
            try
            {
                target.LoadAll();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
            Assert.IsFalse(target.IsAllLoaded);

            // Try good loading
            target = CreateTarget();
            target.AllItemsLoading += (sender, e) =>
                {
                    e.Collection = new KeyValuePair<TKey, TValue>[] { kvp1, kvp2 };
                };

            target.LoadAll();

            Assert.IsTrue(target.IsAllLoaded);
            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 2);

            // Try cancelled loading
            target = CreateTarget();
            target.AllItemsLoading += (sender, e) =>
            {
                e.Cancel = true;
            };

            try
            {
                target.LoadAll();
                Assert.Fail();
            }
            catch (OperationCanceledException)
            {
            }

            Assert.IsFalse(target.IsAllLoaded);

            // Try null enumarable returned
            target = CreateTarget();
            target.AllItemsLoading += (sender, e) =>
            {
            };

            try
            {
                target.LoadAll();
                Assert.Fail();
            }
            catch (NullReferenceException)
            {
            }

            Assert.IsFalse(target.IsAllLoaded);

            // Try loading over existing data
            target = CreateTarget();
            target.AllItemsLoading += (sender, e) =>
            {
                e.Collection = new KeyValuePair<TKey, TValue>[] { kvp1, kvp2 };
            };

            target[k3] = v3;
            target.LoadAll();

            Assert.IsTrue(target.IsAllLoaded);
            Assert.IsTrue(target.Count == 3);
            Assert.AreEqual(target[k1], v1);
            Assert.AreEqual(target[k2], v2);
            Assert.AreEqual(target[k3], v3);

            // Try loading over existing data with value replace
            target = CreateTarget();
            target.AllItemsLoading += (sender, e) =>
            {
                e.Collection = new KeyValuePair<TKey, TValue>[] { kvp1, kvp2 };
            };

            target[k1] = v3;
            Assert.AreEqual(target[k1], v3);

            target.LoadAll();

            Assert.IsTrue(target.IsAllLoaded);
            Assert.IsTrue(target.Count == 2);
            Assert.AreEqual(target[k1], v1);
            Assert.AreEqual(target[k2], v2);
        }

        [TestMethod()]
        public void LoadAllTest()
        {
            LoadAllTestHelper();
        }*/

        /// <summary>
        ///A test for ToArray
        ///</summary>
        public void ToArrayTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TKey k3 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();

            TDictionary target = CreateTarget();

            target[k1] = v1;
            target[k2] = v2;
            target[k3] = v3;

            Assert.IsTrue(target.Count == 3);
            Assert.IsTrue(target.ToArray().Length == 3);

        }

        [TestMethod()]
        public void ToArrayTest()
        {
            ToArrayTestHelper();
        }

        /// <summary>
        ///A test for TryAdd
        ///</summary>
        public void TryAddTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();

            TDictionary target = CreateTarget();

            Assert.IsTrue(target.TryAdd(k1, v1));
            Assert.IsTrue(target.TryAdd(k2, v2));
            Assert.IsFalse(target.TryAdd(k1, v3));
            Assert.AreEqual(target[k1], v1);
        }

        [TestMethod()]
        public void TryAddTest()
        {
            TryAddTestHelper();
        }

        /// <summary>
        ///A test for TryGetValue
        ///</summary>
        public void TryGetValueTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v;

            TDictionary target = CreateTarget();

            // Try getting from empty collection
            Assert.IsFalse(target.TryGetValue(k1, out v));

            target[k1] = v1;

            Assert.IsTrue(target.TryGetValue(k1, out v));
            Assert.AreEqual(v, v1);
            Assert.IsFalse(target.TryGetValue(k2, out v));
            Assert.AreEqual(v, default(TValue));
        }

        [TestMethod()]
        public void TryGetValueTest()
        {
            TryGetValueTestHelper();
        }

        /// <summary>
        ///A test for TryRemove
        ///</summary>
        public void TryRemoveTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v;

            TDictionary target = CreateTarget();

            // Try remove empty
            Assert.IsFalse(target.TryRemove(k1, out v));
            Assert.AreEqual(v, default(TValue));
            Assert.IsTrue(target.IsEmpty);
            Assert.IsTrue(target.Count == 0);

            // Add multiple and try to remove
            target[k1] = v1;
            target[k2] = v2;
            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 2);
            Assert.IsTrue(target.TryRemove(k1, out v));
            Assert.AreEqual(v, v1);
            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);

            Assert.IsFalse(target.TryRemove(k1, out v));
            Assert.IsFalse(target.IsEmpty);
            Assert.IsTrue(target.Count == 1);

            Assert.IsTrue(target.TryRemove(k2, out v));
            Assert.AreEqual(v, v2);
            Assert.IsTrue(target.IsEmpty);
            Assert.IsTrue(target.Count == 0);
        }

        [TestMethod()]
        public void TryRemoveTest()
        {
            TryRemoveTestHelper();
        }

        /// <summary>
        ///A test for TryUpdate
        ///</summary>
        public void TryUpdateTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v3 = new TValue();

            TDictionary target = CreateTarget();

            // Try update non-existent
            Assert.IsFalse(target.TryUpdate(k1, v1, v1));
            Assert.IsTrue(target.IsEmpty);
            Assert.IsTrue(target.Count == 0);

            // Try update existing but wrong value
            target[k1] = v1;
            Assert.IsFalse(target.TryUpdate(k1, v3, v2));
            Assert.AreEqual(target[k1], v1);

            // Try update good one
            Assert.IsTrue(target.TryUpdate(k1, v3, v1));
            Assert.AreEqual(target[k1], v3);
        }

        [TestMethod()]
        public void TryUpdateTest()
        {
            TryUpdateTestHelper();
        }

        /// <summary>
        ///A test for Count
        ///</summary>
        public void CountTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v;

            TDictionary target = CreateTarget();

            Assert.IsTrue(target.Count == 0);
            target[k1] = v1;
            Assert.IsTrue(target.Count == 1);
            target[k2] = v2;
            Assert.IsTrue(target.Count == 2);
            Assert.IsTrue(target.TryRemove(k1, out v));
            Assert.IsTrue(target.Count == 1);
            Assert.IsTrue(target.TryRemove(k2, out v));
            Assert.IsTrue(target.Count == 0);
        }

        [TestMethod()]
        public void CountTest()
        {
            CountTestHelper();
        }

        /*
        /// <summary>
        ///A test for IsAllLoaded
        ///</summary>
        public void IsAllLoadedTestHelper()
        {
            TDictionary target = CreateTarget();

            Assert.IsFalse(target.IsAllLoaded);

            // Try load without event set
            try
            {
                target.LoadAll();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }

            Assert.IsFalse(target.IsAllLoaded);

            // Try load with cancel
            target = CreateTarget();
            target.AllItemsLoading += (sender, e) =>
                {
                    e.Cancel = true;
                };

            try
            {
                target.LoadAll();
                Assert.Fail();
            }
            catch (OperationCanceledException)
            {
            }

            Assert.IsFalse(target.IsAllLoaded);

            // Try load with null collection
            target = CreateTarget();
            target.AllItemsLoading += (sender, e) =>
            {
                e.Collection = null;
            };

            try
            {
                target.LoadAll();
                Assert.Fail();
            }
            catch (NullReferenceException)
            {
            }

            Assert.IsFalse(target.IsAllLoaded);

            // Try correct loading with no data
            target = CreateTarget();
            target.AllItemsLoading += (sender, e) =>
            {
                e.Collection = new KeyValuePair<TKey, TValue>[] { };
            };

            target.LoadAll();
            Assert.IsTrue(target.IsAllLoaded);
        }

        [TestMethod()]
        public void IsAllLoadedTest()
        {
            IsAllLoadedTestHelper();
        }*/

        /// <summary>
        ///A test for IsEmpty
        ///</summary>
        public void IsEmptyTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v;

            TDictionary target = CreateTarget();
            Assert.IsTrue(target.IsEmpty);

            target[k1] = v1;
            Assert.IsFalse(target.IsEmpty);

            target.TryRemove(k1, out v);
            Assert.IsTrue(target.IsEmpty);
        }

        [TestMethod()]
        public void IsEmptyTest()
        {
            IsEmptyTestHelper();
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        public void ItemTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();

            TDictionary target = CreateTarget();

            target[k1] = v1;
            target[k2] = v2;
            Assert.AreEqual(target[k1], v1);
            Assert.AreEqual(target[k2], v2);
            target[k1] = v2;
            target[k2] = v1;
            Assert.AreEqual(target[k1], v2);
            Assert.AreEqual(target[k2], v1);
        }

        [TestMethod()]
        public void ItemTest()
        {
            ItemTestHelper();
        }

        /// <summary>
        ///A test for Keys
        ///</summary>
        public void KeysTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();

            TDictionary target = CreateTarget();

            Assert.IsTrue(target.Keys.Count == 0);

            target[k1] = v1;
            target[k2] = v2;

            Assert.IsTrue(target.Keys.Count == 2);
            Assert.IsTrue(target.Keys.Contains(k1));
            Assert.IsTrue(target.Keys.Contains(k2));

            target.Clear();

            Assert.IsTrue(target.Keys.Count == 0);
        }

        [TestMethod()]
        public void KeysTest()
        {
            KeysTestHelper();
        }

        /// <summary>
        ///A test for Values
        ///</summary>
        public void ValuesTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();

            TDictionary target = CreateTarget();

            Assert.IsTrue(target.Values.Count == 0);

            target[k1] = v1;
            target[k2] = v2;

            Assert.IsTrue(target.Values.Count == 2);
            Assert.IsTrue(target.Values.Contains(v1));
            Assert.IsTrue(target.Values.Contains(v2));

            target.Clear();

            Assert.IsTrue(target.Values.Count == 0);
        }

        [TestMethod()]
        public void ValuesTest()
        {
            ValuesTestHelper();
        }

        /// <summary>
        ///A test for System.Collections.IDictionary.IsReadOnly
        ///</summary>
        public void IsReadOnlyTestHelper()
        {
            IDictionary target = CreateTarget();
            Assert.IsFalse(target.IsReadOnly);
        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void IsReadOnlyTest()
        {
            IsReadOnlyTestHelper();
        }

        /// <summary>
        ///A test for System.Collections.IDictionary.IsFixedSize
        ///</summary>
        public void IsFixedSizeTestHelper()
        {
            IDictionary target = CreateTarget();

            Assert.IsFalse(target.IsFixedSize);
        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void IsFixedSizeTest()
        {
            IsFixedSizeTestHelper();
        }

        /// <summary>
        ///A test for System.Collections.ICollection.SyncRoot
        ///</summary>
        public void SyncRootTestHelper()
        {
            ICollection target = CreateTarget();

            try
            {
                object o = target.SyncRoot;
                Assert.Fail();
            }
            catch (NotSupportedException)
            {
            }
        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void SyncRootTest()
        {
            SyncRootTestHelper();
        }

        /// <summary>
        ///A test for System.Collections.ICollection.IsSynchronized
        ///</summary>
        public void IsSynchronizedTestHelper()
        {
            ICollection target = CreateTarget();
            Assert.IsFalse(target.IsSynchronized);
        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void IsSynchronizedTest()
        {
            IsSynchronizedTestHelper();
        }

        public void ItemAddedItemUpdatedTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();

            TDictionary target;

            // Make sure OnItemAdded event is rised
            bool onaddedrised;
            bool onupdatedrised;
            bool onloadingrised;

            target = CreateTarget();
            target.ItemAdded += (o, e) =>
            {
                onaddedrised = true;
                Assert.AreEqual(e.Value, v1);
            };

            target.ItemUpdated += (o, e) =>
            {
                onupdatedrised = true;
                Assert.AreEqual(e.OldValue, v1);
                Assert.AreEqual(e.NewValue, v2);
            };

            target.ItemLoading += (o, e) =>
            {
                onloadingrised = true;
                Assert.AreEqual(e.Key, k1);
            };

            //          -- try add with get or add
            onaddedrised = false;
            onupdatedrised = false;
            onloadingrised = false;

            target.GetOrAdd(k1, v1);
            Assert.AreEqual(target[k1], v1);
            Assert.IsTrue(onaddedrised);
            Assert.IsFalse(onupdatedrised);
            Assert.IsTrue(onloadingrised);

            //          -- try get with get or add
            onaddedrised = false;
            onupdatedrised = false;
            onloadingrised = false;

            target.GetOrAdd(k1, v2);
            Assert.IsFalse(onaddedrised);
            Assert.IsFalse(onupdatedrised);
            Assert.IsFalse(onloadingrised);

            // Clear and make sure loading doesnt happen
            target.Clear();
            target.ItemLoading += (k, e) => Assert.Fail();

            //          -- try add
            onaddedrised = false;
            onupdatedrised = false;

            target[k1] = v1;
            Assert.IsTrue(onaddedrised);
            Assert.IsFalse(onupdatedrised);

            //          -- try update
            onaddedrised = false;
            onupdatedrised = false;

            target[k1] = v2;
            Assert.IsFalse(onaddedrised);
            Assert.IsTrue(onupdatedrised);

            //          -- try add with addorupdate
            onaddedrised = false;
            onupdatedrised = false;

            target.AddOrUpdate(k2, v1, (k,v) => v1);
            Assert.IsTrue(onaddedrised);
            Assert.IsFalse(onupdatedrised);

            //          -- try update with addorupdate
            onaddedrised = false;
            onupdatedrised = false;

            target.AddOrUpdate(k2, v1, (k, v) => v2);
            Assert.IsFalse(onaddedrised);
            Assert.IsTrue(onupdatedrised);

        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void ItemAddedItemUpdatedItemRemovedTest()
        {
            ItemAddedItemUpdatedTestHelper();
        }

        public void ItemLoadingItemRemovedTestHelper()
        {
            TKey k1 = new TKey();
            TKey k2 = new TKey();
            TValue v1 = new TValue();
            TValue v2 = new TValue();
            TValue v;

            TDictionary target = CreateTarget();

            bool onloadingrised;
            bool onremovedrised;

            onloadingrised = false;
            onremovedrised = false;

            target.ItemLoading += (o, e) =>
                {
                    Assert.AreEqual(e.Key, k1);
                    e.Value = v1;
                    e.IsFound = true;

                    onloadingrised = true;
                };

            v = target[k1];
            Assert.IsTrue(onloadingrised);

            onloadingrised = false;
            onremovedrised = false;

            v = target[k1];
            Assert.IsFalse(onloadingrised);

            target.ItemRemoved += (o, e) =>
                {
                    Assert.AreEqual(e.Key, k1);
                    Assert.AreEqual(e.Value, v1);
                    onremovedrised = true;
                };

            target.TryRemove(k1, out v);
            Assert.AreEqual(v, v1);
            Assert.IsTrue(onremovedrised);
        }

        [TestMethod()]
        [DeploymentItem("Jhu.Graywulf.Components.dll")]
        public void ItemLoadingItemRemovedTest()
        {
            ItemLoadingItemRemovedTestHelper();
        }
    }
}
