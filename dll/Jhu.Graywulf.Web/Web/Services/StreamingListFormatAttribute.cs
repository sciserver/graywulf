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
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Formatter = new StreamingListFormatter();
        }

        public void Validate(OperationDescription operationDescription)
        {
            // TODO: can validate return value type here
        }
    }
}
