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
            get { return true; }
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
            new RestLoggingContext(Logging.LoggingContext.Current).Push();
            RestLoggingContext.Current.DefaultEventSource = Logging.EventSource.WebService;

            var svc = (RestServiceBase)instance;
            svc.OnBeforeInvoke();
            LogOperation();

            try
            {
                var res = this.originalInvoker.Invoke(instance, inputs, out outputs);
                svc.OnAfterInvoke();
                return res;
            }
            catch (Exception ex)
            {
#if BREAKDEBUG
                if (System.Diagnostics.Debugger != null)
                {
                    System.Diagnostics.Debugger.Break();
                }
#endif

                var logevent = LogError(ex);

                // TODO: this won't catch exceptions from IEnumerator that occur
                // in MoveNext, so they won't be logged.
                svc.OnError(ex);

                // Wrap up exception into a RestOperationException which will convey it to
                // the error handler implementation
                throw new RestOperationException(ex, logevent?.ID.ToString());      // *** TODO: use bookmark
            }
            finally
            {
                RestLoggingContext.Current.Pop();
            }
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        #region Logging

        internal Logging.Event LogOperation()
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Operation,
                Logging.EventSource.WebService,
                null,
                null,
                null,
                null);

            UpdateEvent(e);
            Logging.LoggingContext.Current.WriteEvent(e);

            return e;
        }

        internal Logging.Event LogError(Exception ex)
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Error,
                Logging.EventSource.WebService,
                null,
                null,
                ex,
                null);

            UpdateEvent(e);
            Logging.LoggingContext.Current.WriteEvent(e);

            return e;
        }

        private void UpdateEvent(Logging.Event e)
        {
            string message = null;
            string operation = null;

            var context = System.ServiceModel.OperationContext.Current;

            if (context != null)
            {
                if (context.IncomingMessageProperties.ContainsKey("HttpOperationName"))
                {
                    operation = context.Host.Description.ServiceType.FullName + "." +
                        (string)context.IncomingMessageProperties["HttpOperationName"];
                }

                if (context.IncomingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    var req = (HttpRequestMessageProperty)context.IncomingMessageProperties["httpRequest"];

                    message =
                        req.Method.ToUpper() + " " +
                        context.IncomingMessageProperties.Via.AbsolutePath +
                        context.IncomingMessageProperties["HttpOperationName"];
                }
            }

            e.Message = message;
            e.Operation = operation;
        }

        #endregion
    }
}
