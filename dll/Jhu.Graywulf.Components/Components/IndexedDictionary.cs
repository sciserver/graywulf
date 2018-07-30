using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Components
{
    public class IndexedDictionary<TKey, TValue> : IList<TValue>, IDictionary<TKey, TValue>, IEnumerable<TValue>
    {
        public class IndexedDictionaryEnumerator : IEnumerator<TValue>
        {
            private IndexedDictionary<TKey, TValue> dict;
            private int index;

            public IndexedDictionaryEnumerator(IndexedDictionary<TKey, TValue> dict)
            {
                this.dict = dict;
                this.index = -1;
            }

            public TValue Current
            {
                get { return dict.list[index].Value; }
            }

            object IEnumerator.Current
            {
                get { return dict.list[index].Value; }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return ++index < dict.list.Count;
            }

            public void Reset()
            {
                index = -1;
            }
        }

        private List<KeyValuePair<TKey, TValue>> list;
        private Dictionary<TKey, KeyValuePair<int, TValue>> dict;

        public TValue this[int index]
        {
            get { return list[index].Value; }
            set { throw new InvalidOperationException(); }
        }

        public TValue this[TKey key]
        {
            get { return dict[key].Value; }
            set { throw new InvalidOperationException(); }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICollection<TKey> Keys
        {
            get { return list.Select(i => i.Key).ToArray(); }
        }

        public ICollection<TValue> Values
        {
            get { return list.Select(i => i.Value).ToArray(); }
        }

        public IndexedDictionary()
        {
            this.list = new List<KeyValuePair<TKey, TValue>>();
            this.dict = new Dictionary<TKey, KeyValuePair<int, TValue>>();
        }

        public IndexedDictionary(int capacity)
        {
            this.list = new List<KeyValuePair<TKey, TValue>>(capacity);
            this.dict = new Dictionary<TKey, KeyValuePair<int, TValue>>(capacity);
        }

        public IndexedDictionary(IEqualityComparer<TKey> comparer)
        {
            this.list = new List<KeyValuePair<TKey, TValue>>();
            this.dict = new Dictionary<TKey, KeyValuePair<int, TValue>>(comparer);
        }

        public IndexedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.list = new List<KeyValuePair<TKey, TValue>>(capacity);
            this.dict = new Dictionary<TKey, KeyValuePair<int, TValue>>(capacity, comparer);
        }

        public IndexedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.list = new List<KeyValuePair<TKey, TValue>>(dictionary.Count);
            this.dict = new Dictionary<TKey, KeyValuePair<int, TValue>>(dictionary.Count);

            CopyValues(dictionary);
        }

        public IndexedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.list = new List<KeyValuePair<TKey, TValue>>(dictionary.Count);
            this.dict = new Dictionary<TKey, KeyValuePair<int, TValue>>(dictionary.Count, comparer);

            CopyValues(dictionary);
        }

        public IndexedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            this.list = new List<KeyValuePair<TKey, TValue>>();
            this.dict = new Dictionary<TKey, KeyValuePair<int, TValue>>();

            CopyValues(values);
        }

        public IndexedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> values, IEqualityComparer<TKey> comparer)
        {
            this.list = new List<KeyValuePair<TKey, TValue>>();
            this.dict = new Dictionary<TKey, KeyValuePair<int, TValue>>(comparer);

            CopyValues(values);
        }

        private void CopyValues(IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            foreach (var item in values)
            {
                dict.Add(item.Key, new KeyValuePair<int, TValue>(list.Count, item.Value));
                list.Add(new KeyValuePair<TKey, TValue>(item.Key, item.Value));
            }
        }

        public void Add(TValue item)
        {
            throw new InvalidOperationException();
        }

        public void Add(TKey key, TValue value)
        {
            dict.Add(key, new KeyValuePair<int, TValue>(list.Count, value));
            list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            dict.Add(item.Key, new KeyValuePair<int, TValue>(list.Count, item.Value));
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
            dict.Clear();
        }

        public bool Contains(TValue item)
        {
            return list.Select(i => i.Value).Contains(item);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return list.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            list.Select(i => i.Value).ToArray().CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(TValue item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (EqualityComparer<TValue>.Default.Equals(list[i].Value, item))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, TValue item)
        {
            throw new InvalidOperationException();
        }

        public void Insert(int index, TKey key, TValue value)
        {
            dict.Add(key, new KeyValuePair<int, TValue>(index, value));
            list.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
            Renumber();
        }

        public bool Remove(TValue item)
        {
            if (Contains(item))
            {
                var index = IndexOf(item);
                dict.Remove(list[index].Key);
                list.RemoveAt(index);
                Renumber();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(TKey key)
        {
            if (dict.ContainsKey(key))
            {
                list.RemoveAt(dict[key].Key);
                dict.Remove(key);
                Renumber();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (dict.ContainsKey(item.Key))
            {
                list.RemoveAt(dict[item.Key].Key);
                dict.Remove(item.Key);
                Renumber();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveAt(int index)
        {
            var key = list[index].Key;
            list.RemoveAt(index);
            dict.Remove(key);
            Renumber();
        }

        private void Renumber()
        {
            for (int i = 0; i < list.Count; i++)
            {
                dict[list[i].Key] = new KeyValuePair<int, TValue>(i, list[i].Value);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (dict.TryGetValue(key, out var kv))
            {
                value = kv.Value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new IndexedDictionaryEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new IndexedDictionaryEnumerator(this);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
