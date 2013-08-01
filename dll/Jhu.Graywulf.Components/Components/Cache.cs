using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class Cache<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
        where TValue : class, ICacheable
    {
        protected IEqualityComparer<TKey> comparer;
        protected TimeSpan collectionInterval;
        protected TimeSpan maxAge;

        protected System.Threading.Timer collectionTimer;

        internal IEqualityComparer<TKey> Comparer
        {
            get { return comparer; }
        }

        public TimeSpan CollectionInterval
        {
            get { return collectionInterval; }
            set { collectionInterval = value; }
        }

        public TimeSpan MaxAge
        {
            get { return maxAge; }
            set { maxAge = value; }
        }

        public Cache()
            :base()
        {
            InitializeMembers();

            this.comparer = EqualityComparer<TKey>.Default;
            StartTimer();
        }

        public Cache(IEqualityComparer<TKey> comparer)
            :base(comparer)
        {
            InitializeMembers();

            this.comparer = comparer;
            StartTimer();
        }

        private void InitializeMembers()
        {
            this.comparer = null;
            this.collectionInterval = new TimeSpan(0, 5, 0);
            this.maxAge = new TimeSpan(1, 0, 0);

            this.collectionTimer = new System.Threading.Timer(CollectionTimerCallback);
        }

        private void CollectionTimerCallback(object state)
        {
            // *** TODO: Implement collection logic here
            lock (this)
            {

            }

            StartTimer();
        }

        private void StartTimer()
        {
            collectionTimer.Change((int)collectionInterval.TotalMilliseconds, System.Threading.Timeout.Infinite);
        }

    }
}
