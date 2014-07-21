using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class AllItemsLoadingEventArgs<TKey, TValue> : EventArgs
    {
        private bool isCancelled;
        private IEnumerable<KeyValuePair<TKey, TValue>> items;

        public bool IsCancelled
        {
            get { return isCancelled; }
            set { isCancelled = value; }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Items
        {
            get { return items; }
            set { items = value; }
        }

        public AllItemsLoadingEventArgs()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.isCancelled = false;
            this.items = null;
        }
    }
}
