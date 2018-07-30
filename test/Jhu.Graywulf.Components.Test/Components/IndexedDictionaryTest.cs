using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Components
{
    [TestClass]
    public class IndexedDictionaryTest
    {
        private IndexedDictionary<string, string> Create()
        {
            var id = new IndexedDictionary<string, string>();

            id.Add("zero", "zero");
            id.Add("one", "one");
            id.Add("two", "two");
            id.Add("three", "three");
            id.Add("four", "four");

            return id;
        }

        [TestMethod]
        public void ListIndexerTest()
        {
            var id = Create();
            Assert.AreEqual("one", id[1].Value);

            id[2] = new KeyValuePair<string, string>("five", "five");
            Assert.AreEqual("five", id["five"]);
            Assert.AreEqual("five", id[2].Value);
            Assert.AreEqual(2, id.IndexOf("five"));
        }

        [TestMethod]
        public void DictionaryIndexerTest()
        {
            var id = Create();
            Assert.AreEqual("two", id["two"]);

            id["two"] = "five";

            Assert.AreEqual("five", id["two"]);
            Assert.AreEqual("five", id[2].Value);
        }

        [TestMethod]
        public void TestCount()
        {
            var id = Create();
            Assert.AreEqual(5, id.Count);
        }

        [TestMethod]
        public void KeyOrderTest()
        {
            var id = Create();
            var t = id.Keys.ToArray();
            Assert.AreEqual("two", t[2]);
            Assert.AreEqual("four", t[4]);
        }

        [TestMethod]
        public void ValueOrderTest()
        {
            var id = Create();
            var t = id.Values.ToArray();
            Assert.AreEqual("two", t[2]);
            Assert.AreEqual("four", t[4]);
        }

        [TestMethod]
        public void ContainsTest()
        {
            var id = Create();
            Assert.IsTrue(id.Contains("two"));
            Assert.IsFalse(id.Contains("five"));
        }

        [TestMethod]
        public void ContainsKeyTest()
        {
            var id = Create();
            Assert.IsTrue(id.Contains("two"));
            Assert.IsFalse(id.Contains("five"));
        }

        [TestMethod]
        public void IndexOfTest()
        {
            var id = Create();
            Assert.AreEqual(2, id.IndexOf("two"));
            Assert.AreEqual(4, id.IndexOf("four"));
            Assert.AreEqual(-1, id.IndexOf("five"));
        }

        [TestMethod]
        public void TryGetValueTest()
        {
            var id = Create();
            Assert.IsTrue(id.TryGetValue("one", out var val1));
            Assert.IsFalse(id.TryGetValue("five", out var val2));
        }

        [TestMethod]
        public void TestEnumerator()
        {
            var id = Create();
            var q = 0;
            foreach (var v in id)
            {
                Assert.AreEqual(id[q].Value, v);
                q++;
            }
        }

        [TestMethod]
        public void InsertTest()
        {
            var id = Create();
            id.Insert(3, "five", "five");

            Assert.AreEqual("two", id[2].Value);
            Assert.AreEqual("five", id[3].Value);
            Assert.AreEqual("three", id[4].Value);
        }

        [TestMethod]
        public void RemoveTest1()
        {
            var id = Create();
            id.Remove("two");

            Assert.AreEqual("one", id[1].Value);
            Assert.AreEqual("three", id[2].Value);
        }

        [TestMethod]
        public void RemoveTest2()
        {
            var id = Create();
            ((IDictionary<string, string>)id).Remove("two");

            Assert.AreEqual("one", id[1].Value);
            Assert.AreEqual("three", id[2].Value);
        }

        [TestMethod]
        public void RemoveAtTest()
        {
            var id = Create();
            id.RemoveAt(2);

            Assert.AreEqual("one", id[1].Value);
            Assert.AreEqual("three", id[2].Value);
        }
    }
}
