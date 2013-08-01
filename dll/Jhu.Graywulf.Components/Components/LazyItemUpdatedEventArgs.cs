using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class LazyItemUpdatedEventArgs<TKey, TValue> : EventArgs
    {
        private TKey key;
        private TValue oldValue;
        private TValue newValue;

        public TKey Key
        {
            get { return key; }
            set { key = value; }
        }

        public TValue OldValue
        {
            get { return this.oldValue; }
            set { this.oldValue = value; }
        }

        public TValue NewValue
        {
            get { return this.newValue; }
            set { this.newValue = value; }
        }

        public LazyItemUpdatedEventArgs()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.key = default(TKey);
            this.oldValue = default(TValue);
            this.newValue = default(TValue);
        }
    }
}
