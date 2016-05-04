using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class StreamingRawFormatAttribute : Attribute, IOperationBehavior
    {
        protected abstract IDispatchMessageFormatter CreateDispatchFormatter();

        protected abstract IClientMessageFormatter CreateClientFormatter();

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            var outmsg = operationDescription.Messages.First(m => m.Direction == MessageDirection.Output);
            var retval = outmsg.Body.ReturnValue;
            var rettype = retval.Type;

            clientOperation.Formatter = CreateClientFormatter();
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Formatter = CreateDispatchFormatter();
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}
