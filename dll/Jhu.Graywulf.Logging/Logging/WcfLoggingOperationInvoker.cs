using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Logging
{
    class WcfLoggingOperationInvoker : IOperationInvoker
    {
        private EventSource eventSource;
        private string operationName;
        private IOperationInvoker originalInvoker;

        public EventSource EventSource
        {
            get { return eventSource; }
            set { eventSource = value; }
        }

        public bool IsSynchronous
        {
            get { return true; }
        }

        public WcfLoggingOperationInvoker(string operationName, IOperationInvoker originalInvoker)
        {
            this.eventSource = EventSource.RemoteService;   // TODO
            this.operationName = operationName;
            this.originalInvoker = originalInvoker;
        }

        public object[] AllocateInputs()
        {
            return originalInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            new WcfLoggingContext(LoggingContext.Current).Push();

            try
            {
                var res = this.originalInvoker.Invoke(instance, inputs, out outputs);
                LogOperation();
                return res;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw ex;
            }
            finally
            {
                LoggingContext.Current.Pop();
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

        private void LogOperation()
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Operation,
                eventSource,
                null,
                null,
                null,
                null);

            WcfLoggingContext.Current.RecordEvent(e);
        }

        private void LogError(Exception ex)
        {
            var e = LoggingContext.Current.CreateEvent(
                EventSeverity.Error,
                eventSource,
                null,
                null,
                ex,
                null);

            WcfLoggingContext.Current.RecordEvent(e);
        }
    }
}
