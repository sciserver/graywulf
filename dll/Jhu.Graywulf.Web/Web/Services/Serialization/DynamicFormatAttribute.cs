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
    /// <summary>
    /// When applied to a REST web service operation, selects the return format
    /// based on the accept header sent by the client.
    /// </summary>
    public class DynamicFormatAttribute : Attribute, IOperationBehavior
    {
        private Type[] formatterTypes;

        public Type[] FormatterTypes
        {
            get { return formatterTypes; }
            set { formatterTypes = value; }
        }

        public DynamicFormatAttribute(Type formatterType)
        {
            this.formatterTypes = new Type[] { formatterType };
        }

        public DynamicFormatAttribute(Type[] formatterTypes)
        {
            this.formatterTypes = formatterTypes;
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            // Intentionally do nothing, just register as a dummy behavior
            // Formatter will be registered in RestEndpointBehavior.CreateDispatchFormatter
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            // Intentionally do nothing, just register as a dummy behavior
            // Formatter will be registered in RestEndpointBehavior.CreateDispatchFormatter

            var dynamicFormatter = new DynamicMessageFormatter();
            dynamicFormatter.Initialize(dispatchOperation.Formatter);

            foreach (var formatterType in formatterTypes)
            {
                var formatter  = (RestMessageFormatterBase)Activator.CreateInstance(formatterType);
                var formats = formatter.GetSupportedFormats();

                // Create a separate instance for each supported type
                foreach (var format in formats)
                {
                    var ff = (RestMessageFormatterBase)Activator.CreateInstance(formatterType);
                    ff.Initialize(dispatchOperation.Formatter);
                    dynamicFormatter.Formatters.Add(format.MimeType, ff);


                    if (ff is RawMessageFormatterBase rff)
                    {
                        rff.MimeType = format.MimeType;
                    }
                }
            }

            dispatchOperation.Formatter = dynamicFormatter;
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}