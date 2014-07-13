using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Implements a simple object pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : IDisposable
    {
        #region Private member variables

        private int maxItems;
        private ConcurrentStack<T> pool;

        /// <summary>
        /// Delegate to create new items
        /// </summary>
        private Func<T> objectCreating;

        #endregion

        /// <summary>
        /// Gets or sets the maximum number of pooled items.
        /// </summary>
        public int MaxItems
        {
            get { return maxItems; }
            set { maxItems = value; }
        }

        public ObjectPool(Func<T> objectCreating)
        {
            InitializeMembers();

            this.objectCreating = objectCreating;
        }

        private void InitializeMembers()
        {
            this.maxItems = 32;
            this.pool = new ConcurrentStack<T>();
            this.objectCreating = null;
        }

        public void Dispose()
        {
            foreach (var item in pool)
            {
                if (item is IDisposable)
                {
                    ((IDisposable)item).Dispose();
                }
            }
        }

        public ObjectPoolItem<T> Take()
        {
            T item;

            if (!pool.TryPop(out item))
            {
                item = objectCreating();
            }

            return new ObjectPoolItem<T>(this, item);
        }

        internal void Return(ObjectPoolItem<T> item)
        {
            if (pool.Count < maxItems)
            {
                pool.Push(item.Value);
            }
            else if (item.Value is IDisposable)
            {
                ((IDisposable)item.Value).Dispose();
            }
        }
    }
}
