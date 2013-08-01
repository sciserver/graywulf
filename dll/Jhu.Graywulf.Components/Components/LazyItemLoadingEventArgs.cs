using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class LazyItemLoadingEventArgs<TKey, TValue> : EventArgs
    {
        private bool isCancelled;
        private bool isFound;
        private TKey key;
        private TValue value;

        public bool IsCancelled
        {
            get { return isCancelled; }
            set { isCancelled = value; }
        }

        public bool IsFound
        {
            get { return isFound; }
            set { isFound = value; }
        }

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

        public LazyItemLoadingEventArgs()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.isCancelled = false;
            this.isFound = false;
            this.key = default(TKey);
            this.value = default(TValue);
        }
    }
}
