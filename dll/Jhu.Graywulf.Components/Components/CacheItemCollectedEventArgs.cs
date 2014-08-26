using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class CacheItemCollectedEventArgs<TKey, TValue> : EventArgs
    {
        public TKey Key { get; set; }
        public TValue Item { get; set; }
    }
}
