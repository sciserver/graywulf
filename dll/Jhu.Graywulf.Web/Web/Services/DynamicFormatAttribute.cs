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
    /// <summary>
    /// When applied to a REST web service operation, selects the return format
    /// based on the accept header sent by the client.
    /// </summary>
    public class DynamicFormatAttribute : Attribute, IOperationBehavior
    {
        private Type formatterType;

        public DynamicFormatAttribute(Type formatterType)
        {
            this.formatterType = formatterType;
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var formatters = new Dictionary<string, IDispatchMessageFormatter>();
            var formatter = (RestMessageFormatter)Activator.CreateInstance(formatterType);
            var formats = formatter.GetSupportedFormats();

            // Create a separate instance for each supported type
            foreach (var format in formats)
            {
                formatter = (RestMessageFormatter)Activator.CreateInstance(formatterType);
                formatter.MimeType = format.MimeType;
                formatters.Add(format.MimeType, formatter);
            }

            dispatchOperation.Formatter = new DynamicDispatchMessageFormatter(
                dispatchOperation.Formatter,
                formatters);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}