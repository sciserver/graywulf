using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Components
{
    public class AsyncQueueUnhandledExceptionEventArgs<T> : EventArgs
    {
        private AsyncQueueUnhandledExceptionLocation location;
        private Exception exception;
        private T item;
        private bool abort;

        public AsyncQueueUnhandledExceptionLocation Location
        {
            get { return location; }
        }

        public Exception Exception
        {
            get { return exception; }
        }

        public T Item
        {
            get { return item; }
        }

        public bool Abort
        {
            get { return abort; }
            set { abort = value; }
        }

        public AsyncQueueUnhandledExceptionEventArgs(AsyncQueueUnhandledExceptionLocation location, Exception exception, T item)
        {
            this.location = location;
            this.exception = exception;
            this.item = item;
            this.abort = false;
        }
    }
}
