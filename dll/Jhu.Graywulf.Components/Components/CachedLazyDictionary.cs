using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class CachedLazyDictionary<TKey, TValue> : LazyDictionary<TKey, TValue>
        where TValue : class, ICacheable
    {
        protected Cache<TKey, TValue> cacheStore;

        #region Constructors and initializers

        public CachedLazyDictionary()
        {
            InitializeMembers();
        }

        public CachedLazyDictionary(Cache<TKey, TValue> cacheStore)
            : base(cacheStore.Comparer)
        {
            InitializeMembers();

            this.cacheStore = cacheStore;
            
            // Copy everything from cache to local
            foreach (var cc in cacheStore)
            {
                localStore.TryAdd(cc.Key, cc.Value);
            }
        }

        private void InitializeMembers()
        {
            this.cacheStore = null;
        }

        #endregion

        protected override bool OnItemLoading(TKey key, out TValue value)
        {
            if (cacheStore != null && cacheStore.TryGetValue(key, out value))
            {
                return true;
            }
            else
            {
                return base.OnItemLoading(key, out value);
            }
        }

        protected override void OnItemAdded(TKey key, TValue value)
        {
            UpdateCache(key, value);

            base.OnItemAdded(key, value);
        }

        protected override void OnItemUpdated(TKey key, TValue newValue, TValue oldValue)
        {
            UpdateCache(key, newValue);

            base.OnItemUpdated(key, newValue, oldValue);
        }

        private void UpdateCache(TKey key, TValue value)
        {
            TValue cv;
            if (value.IsCacheable && cacheStore != null)
            {
                if (cacheStore.TryGetValue(key, out cv))
                {
                    if (cv != value)
                    {
                        cacheStore.AddOrUpdate(key, value, (k, v) => value);
                    }
                }
                else
                {
                    cacheStore.AddOrUpdate(key, value, (k, v) => value);
                }
            }
        }

    }
}
