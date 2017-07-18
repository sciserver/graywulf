using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Components
{
    public class AsyncQueueItemProcessingEventArgs<T> : EventArgs
    {
        private T item;

        public T Item
        {
            get { return item; }
            set { item = value; }
        }

        public AsyncQueueItemProcessingEventArgs(T item)
        {
            this.item = item;
        }
    }
}
