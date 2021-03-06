﻿using System;
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
            using (new RestLoggingContext())
            {
                RestLoggingContext.Current.DefaultEventSource = Logging.EventSource.WebService;
                LogDebug();

                var svc = (RestServiceBase)instance;
                var context = new RestOperationContext();
                OperationContext.Current.Extensions.Add(context);

                try
                {
                    svc.OnBeforeInvoke(context);
                    var res = this.originalInvoker.Invoke(instance, inputs, out outputs);
                    svc.OnAfterInvoke(context);
                    return res;
                }
                catch (Exception ex)
                {
#if BREAKDEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
#endif

                    // WCF automatically wraps SecurityException into a fault and
                    // doesn't call IErrorHandler.ProvideFault, so handle this situation
                    // here
                    if (ex is FaultException fex)
                    {
                        RestErrorHandler.SetHttpResponseStatus(fex);
                    }

                    var e = LogError(ex);

                    // TODO: this won't catch exceptions from IEnumerator that occur
                    // in MoveNext, so they won't be logged.
                    svc.OnError(context, ex);

                    // Wrap up exception into a RestOperationException which will convey it to
                    // the error handler implementation
                    throw new RestOperationException(ex, e.BookmarkGuid.ToString());
                }
                finally
                {
                    OperationContext.Current.Extensions.Remove(context);
                    context.Dispose();
                }
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

        internal Logging.Event LogDebug()
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Debug,
                Logging.EventSource.WebService,
                null,
                null,
                null,
                null);

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

            Logging.LoggingContext.Current.WriteEvent(e);

            return e;
        }
        
        #endregion
    }
}
