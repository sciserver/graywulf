using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class CacheItem<T>
    {
        private T value;
        private DateTime expiresAt;

        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public DateTime ExpiresAt
        {
            get { return expiresAt; }
            set { expiresAt = value; }
        }

        public CacheItem(T value, DateTime expiresAt)
        {
            this.value = value;
            this.expiresAt = expiresAt;
        }
    }
}
