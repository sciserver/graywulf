using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Api
{
    /// <summary>
    /// When applied to a REST web service operation, selects the return format
    /// based on the accept header sent by the client.
    /// </summary>
    public class DynamicResponseFormatAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Formatter = new DynamicResponseMessageFormatter(
                dispatchOperation.Formatter,
                new Dictionary<string, IDispatchMessageFormatter>()
                {
                        {TextResponseMessageFormatter.MimeType, new TextResponseMessageFormatter() }
                });
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}