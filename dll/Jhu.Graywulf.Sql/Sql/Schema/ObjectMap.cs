using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.Schema
{
    // TODO: move this to schema

    public class ObjectMap<TKey, TValue>
        where TKey : DatabaseObject
        where TValue : DatabaseObject
    {
        private ConcurrentDictionary<string, TKey> keys;
        private ConcurrentDictionary<string, TValue> values;

        public IEnumerable<TKey> Keys
        {
            get { return keys.Values; }
        }

        public IEnumerable<TValue> Values
        {
            get { return values.Values; }
        }

        public ObjectMap()
        {
            InitializeMembers();
        }

        public ObjectMap(ObjectMap<TKey, TValue> old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.keys = new ConcurrentDictionary<string, TKey>(SchemaManager.Comparer);
            this.values = new ConcurrentDictionary<string, TValue>(SchemaManager.Comparer);
        }

        private void CopyMembers(ObjectMap<TKey, TValue> old)
        {
            this.keys = new ConcurrentDictionary<string, TKey>(old.keys, SchemaManager.Comparer);
            this.values = new ConcurrentDictionary<string, TValue>(old.values, SchemaManager.Comparer);
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public bool ContainsKey(string key)
        {
            return values.ContainsKey(key);
        }

        public bool ContainsKey(TKey key)
        {
            return values.ContainsKey(key.UniqueKey);
        }

        public bool ContainsValue(string value)
        {
            return keys.ContainsKey(value);
        }

        public bool ContainsValue(TValue value)
        {
            return keys.ContainsKey(value.UniqueKey);
        }

        public TValue GetValue(string key)
        {
            return values[key];
        }

        public TValue GetValue(TKey key)
        {
            return values[key.UniqueKey];
        }

        public TKey GetKey(string value)
        {
            return keys[value];
        }

        public TKey GetKey(TValue value)
        {
            return keys[value.UniqueKey];
        }

        public void Add(TKey key, TValue value)
        {
            keys.TryAdd(value.UniqueKey, key);
            values.TryAdd(key.UniqueKey, value);
        }
    }
}
