using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.ServiceModel
{
    class ServiceLoggingOperationInvoker : IOperationInvoker
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

        public ServiceLoggingOperationInvoker(string operationName, IOperationInvoker originalInvoker)
        {
            this.operationName = operationName;
            this.originalInvoker = originalInvoker;
        }

        public object[] AllocateInputs()
        {
            return originalInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            new ServiceLoggingContext(LoggingContext.Current).Push();

            LogDebug();

            try
            {
                return this.originalInvoker.Invoke(instance, inputs, out outputs);
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

        private void LogDebug()
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Debug,
                EventSource.RemoteService,
                null,
                null,
                null,
                null);

            ServiceLoggingContext.Current.RecordEvent(e);
        }

        private void LogError(Exception ex)
        {
            var e = LoggingContext.Current.CreateEvent(
                EventSeverity.Error,
                EventSource.RemoteService,
                null,
                null,
                ex,
                null);

            ServiceLoggingContext.Current.RecordEvent(e);
        }
    }
}
