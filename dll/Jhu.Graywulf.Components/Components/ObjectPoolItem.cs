using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Implements a wrapper around any object that comes from a pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPoolItem<T> : IDisposable
    {
        private ObjectPool<T> pool;
        private T value;

        public T Value
        {
            get { return value; }
        }

        internal ObjectPoolItem(ObjectPool<T> pool, T value)
        {
            this.pool = pool;
            this.value = value;
        }

        public void Dispose()
        {
            pool.Return(this);
        }
    }
}
