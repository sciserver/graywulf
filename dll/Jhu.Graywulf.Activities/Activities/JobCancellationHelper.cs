using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Activities
{
    class JobCancellationHelper
    {
        private Exception exception;
        private Exception unwrappedException;

        private bool isCancelled;
        private bool isTimeout;

        public Exception Exception
        {
            get { return exception; }
        }

        public Exception UnwrappedException
        {
            get { return unwrappedException; }
        }

        public bool IsCancelled
        {
            get { return isCancelled; }
        }

        public bool IsTimeout
        {
            get { return IsTimeout; }
        }

        public JobCancellationHelper(Exception exception)
        {
            this.exception = exception;
            UnwrapException();
            ProcessException();
        }

        private void UnwrapException()
        {
            if (exception is AggregateException)
            {
                unwrappedException = ((AggregateException)exception).InnerException;
            }
            else
            {
                unwrappedException = exception;
            }
        }

        private void ProcessException()
        {
            // TODO: cancel exception probably differs from provider to provider
            // figure out what to do

            if (unwrappedException is TaskCanceledException)
            {
                isCancelled = true;
            }
            else if (unwrappedException is SqlException)
            {
                // When a command is canceled, a SqlException is thrown by the client.
                var ex = (SqlException)unwrappedException;

                if (ex.Class == 11 && ex.Number == -2)
                {
                    isTimeout = true;
                }
                if (ex.Class == 11 && ex.Number == 0 && ex.ErrorCode == -2146232060)
                {
                    isCancelled = true;
                }
            }
            else if (unwrappedException is OperationCanceledException)
            {
                isCancelled = true;
            }
            else if (unwrappedException is OperationAbortedException)
            {
                // *** TODO
                throw new NotImplementedException();
            }
        }

        public Exception DispatchException()
        {
            if (isCancelled)
            {
                return new OperationCanceledException(unwrappedException.Message, unwrappedException);
            }
            else if (isTimeout)
            {
                return new TimeoutException(unwrappedException.Message, unwrappedException);
            }
            else
            {
                return unwrappedException;
            }
        }
    }
}
