using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Implements a simple cache to store items that are collected
    /// regularly.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Cache<TKey, TValue>
    {
        #region Private member variables

        private IEqualityComparer<TKey> comparer;
        private ConcurrentDictionary<TKey, CacheItem<TValue>> cache;
        protected Timer collectionTimer;

        private TimeSpan defaultLifetime;
        private TimeSpan collectionInterval;

        #endregion
        #region Properties

        public TimeSpan DefaultLifetime
        {
            get { return defaultLifetime; }
            set { defaultLifetime = value; }
        }

        public TimeSpan CollectionInterval
        {
            get { return collectionInterval; }
            set
            {
                collectionInterval = value;
                StartTimer();
            }
        }

        public int Count
        {
            get { return cache.Count; }
        }

        #endregion
        #region Constructors and initializers

        public Cache()
            : base()
        {
            InitializeMembers();
            InitializeCache(EqualityComparer<TKey>.Default);

            StartTimer();
        }

        public Cache(IEqualityComparer<TKey> comparer)
        {
            InitializeMembers();
            InitializeCache(comparer);

            StartTimer();
        }

        private void InitializeMembers()
        {
            this.defaultLifetime = new TimeSpan(0, 5, 0);
            this.collectionInterval = new TimeSpan(0, 5, 0);

            // Create the timer but don't start it
            this.collectionTimer = new System.Threading.Timer(CollectionTimerCallback);
        }

        private void InitializeCache(IEqualityComparer<TKey> comparer)
        {
            this.comparer = comparer;
            this.cache = new ConcurrentDictionary<TKey, CacheItem<TValue>>(comparer);
        }

        #endregion
        #region Cache collection logic

        private void CollectionTimerCallback(object state)
        {
            lock (this)
            {
                var delete = new HashSet<TKey>(this.comparer);
                var now = DateTime.Now;

                foreach (var item in cache)
                {
                    if (item.Value.ExpiresAt <= now)
                    {
                        delete.Add(item.Key);
                    }
                }

                foreach (var key in delete)
                {
                    CacheItem<TValue> item;
                    cache.TryRemove(key, out item);
                }
            }

            StartTimer();
        }

        private void StartTimer()
        {
            // Reschedule the timer, but turn of periodic firing.
            collectionTimer.Change((int)collectionInterval.TotalMilliseconds, System.Threading.Timeout.Infinite);
        }

        #endregion

        /// <summary>
        /// Adds an item to the cache with default lifetime.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAdd(TKey key, TValue value)
        {
            return TryAdd(key, value, defaultLifetime);
        }

        /// <summary>
        /// Adds an item to the cache with a given lifetime.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public bool TryAdd(TKey key, TValue value, TimeSpan lifetime)
        {
            var expiresAt = DateTime.Now.Add(lifetime);
            var item = new CacheItem<TValue>(value, expiresAt);
            return cache.TryAdd(key, item);
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryRemove(TKey key, out TValue value)
        {
            CacheItem<TValue> item;
            var res = cache.TryRemove(key, out item);
            value = item.Value;
            return res;
        }

        /// <summary>
        /// Tries to get and item from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            CacheItem<TValue> item;

            // Only non-expired items are returned
            if (cache.TryGetValue(key, out item))
            {
                if (item.ExpiresAt > DateTime.Now)
                {
                    value = item.Value;
                    return true;
                }
                else
                {
                    // Remove expired item
                    cache.TryRemove(key, out item);
                }
            }

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Clears all items from the cache
        /// </summary>
        public void Clear()
        {
            cache.Clear();
        }
    }
}
