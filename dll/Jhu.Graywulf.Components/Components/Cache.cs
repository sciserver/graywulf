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
    public class Cache<TKey, TValue> : IDisposable
    {
        #region Private member variables

        private IEqualityComparer<TKey> comparer;
        private ConcurrentDictionary<TKey, CacheItem<TValue>> cache;
        protected Timer collectionTimer;

        private TimeSpan defaultLifetime;
        private bool autoExtendLifetime;
        private TimeSpan collectionInterval;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the default lifetime of objects in cache.
        /// </summary>
        public TimeSpan DefaultLifetime
        {
            get { return defaultLifetime; }
            set { defaultLifetime = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether object expiration time
        /// is extended automatically when the objects are accessed.
        /// </summary>
        public bool AutoExtendLifetime
        {
            get { return autoExtendLifetime; }
            set { autoExtendLifetime = value; }
        }

        /// <summary>
        /// Gets or sets the garbage collection interval.
        /// </summary>
        public TimeSpan CollectionInterval
        {
            get { return collectionInterval; }
            set
            {
                collectionInterval = value;
                StartTimer();
            }
        }

        public IEnumerable<TKey> Keys
        {
            get { return cache.Keys; }
        }

        public IEnumerable<TValue> Values
        {
            get { return cache.Values.Select(i => i.Value); }
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
            this.autoExtendLifetime = true;
            this.collectionInterval = new TimeSpan(0, 5, 0);

            // Create the timer but don't start it
            this.collectionTimer = new System.Threading.Timer(CollectionTimerCallback);
        }

        private void InitializeCache(IEqualityComparer<TKey> comparer)
        {
            this.comparer = comparer;
            this.cache = new ConcurrentDictionary<TKey, CacheItem<TValue>>(comparer);
        }

        public void Dispose()
        {
            if (collectionTimer != null)
            {
                collectionTimer.Dispose();
                collectionTimer = null;
            }
        }

        #endregion
        #region Cache collection logic

        /// <summary>
        /// Called by the timer to perform the garbage collection.
        /// </summary>
        /// <param name="state"></param>
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

        /// <summary>
        /// Starts or restarts the garbage collection timer.
        /// </summary>
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
            return TryAdd(key, value, expiresAt);
        }

        /// <summary>
        /// Adds an item to the cache with a given expiration time.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public bool TryAdd(TKey key, TValue value, DateTime expiresAt)
        {
            // Check if item is already in the cache. This test will also
            // cause the expiration time to extend.

            CacheItem<TValue> olditem;
            if (cache.TryGetValue(key, out olditem))
            {
                // Check if it's expired, if it has, simply throw away and
                // add new item
                if (olditem.IsExpired)
                {
                    cache.TryRemove(key, out olditem);
                }
                else
                {
                    // Cannot add new item, an unexpired old one still exists
                    return false;
                }
            }

            // Now it's safe to try to add the new item
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
                var now = DateTime.Now;

                if (item.ExpiresAt > now)
                {
                    // Extend lifetime of object on a cache hit
                    if (autoExtendLifetime)
                    {
                        item.ExpiresAt = now.Add(defaultLifetime);
                    }

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
