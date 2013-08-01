using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class LazyItemAddedEventArgs<TKey, TValue> : EventArgs
    {
        private TKey key;
        private TValue value;

        public TKey Key
        {
            get { return key; }
            set { key = value; }
        }

        public TValue Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public LazyItemAddedEventArgs()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.key = default(TKey);
            this.value = default(TValue);
        }
    }
}
