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
            var formats = new Dictionary<string, IDispatchMessageFormatter>();
            var formatter = (GraywulfMessageFormatter)Activator.CreateInstance(formatterType);
            var mimetypes = formatter.GetSupportedMimeTypes();

            // Create a separate instance for each supported type
            foreach (var mimetype in mimetypes)
            {
                formatter = (GraywulfMessageFormatter)Activator.CreateInstance(formatterType);
                formatter.MimeType = mimetype;
                formats.Add(mimetype, formatter);
            }

            dispatchOperation.Formatter = new DynamicDispatchMessageFormatter(
                dispatchOperation.Formatter,
                formats);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}