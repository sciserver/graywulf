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
    public class StreamingListFormatAttribute : Attribute, IOperationBehavior
    {
        public StreamingListFormatAttribute()
        {
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            // Find output message
            var outmsg = operationDescription.Messages.First(m => m.Direction == MessageDirection.Output);
            var retval = outmsg.Body.ReturnValue;
            var rettype = retval.Type;

            clientOperation.Formatter = new StreamingListFormatter(clientOperation.Formatter, rettype);
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Formatter = new StreamingListFormatter(dispatchOperation.Formatter);
        }

        public void Validate(OperationDescription operationDescription)
        {
            // TODO: can validate return value type here
        }
    }
}
