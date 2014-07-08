using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    public class AllObjectsLoadingEventArgs<TKey, TValue> : EventArgs
    {
        private string databaseName;
        private bool cancel;
        private IEnumerable<KeyValuePair<TKey, TValue>> collection;

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Collection
        {
            get { return collection; }
            set { collection = value; }
        }

        public AllObjectsLoadingEventArgs()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.databaseName = null;
            this.cancel = false;
            this.collection = null;
        }
    }
}
