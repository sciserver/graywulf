using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Components
{
    public class AsyncQueueUnhandledExceptionEventArgs : EventArgs
    {
        private Exception exception;
        private bool abort;

        public Exception Exception
        {
            get { return exception; }
        }

        public bool Abort
        {
            get { return abort; }
            set { abort = value; }
        }

        public AsyncQueueUnhandledExceptionEventArgs(Exception exception)
        {
            this.exception = exception;
            this.abort = false;
        }
    }
}
