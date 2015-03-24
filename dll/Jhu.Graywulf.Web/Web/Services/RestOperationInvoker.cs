using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Services
{
    class RestOperationInvoker : IOperationInvoker
    {
        private string operationName;
        private IOperationInvoker originalInvoker;

        public RestOperationInvoker(string operationName, IOperationInvoker originalInvoker)
        {
            this.operationName = operationName;
            this.originalInvoker = originalInvoker;
        }

        public bool IsSynchronous
        {
            get { return this.originalInvoker.IsSynchronous; }
        }

        /// <summary>
        /// Allocates objects that will be used as parameters for the function call
        /// </summary>
        /// <returns></returns>
        public object[] AllocateInputs()
        {
            return originalInvoker.AllocateInputs();
        }


        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            object res = null;
            var svc = (RestServiceBase)instance;

            svc.OnBeforeInvoke();

            // Catch exceptions so that they can be logged.
            try
            {
                res = this.originalInvoker.Invoke(instance, inputs, out outputs);
            }
            catch (Exception ex)
            {
#if BREAKDEBUG
                System.Diagnostics.Debugger.Break();
#endif
                // TODO: this won't catch exceptions from IEnumerator that occur
                // in MoveNext, so they won't be logged.
                var logevent = svc.OnError(operationName, ex);

                // Wrap up exception into a RestOperationException which will convey it to
                // the error handler implementation
                throw new RestOperationException(ex, logevent.EventId.ToString());
            }

            svc.OnAfterInvoke();

            return res;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return this.originalInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return this.originalInvoker.InvokeEnd(instance, out outputs, result);
        }
    }
}
