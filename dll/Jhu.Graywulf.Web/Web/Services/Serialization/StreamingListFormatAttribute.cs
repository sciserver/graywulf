using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Services.Serialization
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

            var formatter = new StreamingListFormatter();
            formatter.Initialize(clientOperation.Formatter, rettype);
            clientOperation.Formatter = formatter;
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var formatter = new StreamingListFormatter();
            formatter.Initialize(dispatchOperation.Formatter);
            dispatchOperation.Formatter = formatter;
        }

        public void Validate(OperationDescription operationDescription)
        {
            // TODO: can validate return value type here
        }
    }
}
