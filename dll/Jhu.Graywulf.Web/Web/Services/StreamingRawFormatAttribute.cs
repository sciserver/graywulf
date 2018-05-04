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
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        private Type[] GetParameterTypes(OperationDescription operationDescription)
        {
            var inmsg = operationDescription.Messages.First(m => m.Direction == MessageDirection.Input);
            var res = new Type[inmsg.Body.Parts.Count];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = inmsg.Body.Parts[i].Type;
            }

            return res;
        }

        private Type GetRetvalType(OperationDescription operationDescription)
        {
            var outmsg = operationDescription.Messages.First(m => m.Direction == MessageDirection.Output);
            return outmsg.Body.ReturnValue.Type;
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            // Intentionally do nothing, just register as a dummy behavior
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            // Intentionally do nothing, just register as a dummy behavior
        }
        
        public void Validate(OperationDescription operationDescription)
        {
        }

        public StreamingRawFormatterBase CreateFormatter()
        {
            return OnCreateFormatter();
        }

        internal StreamingRawFormatterBase CreateDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint, IDispatchMessageFormatter fallbackFormatter)
        {
            var formatter = OnCreateFormatter();
            formatter.Initialize(operationDescription, endpoint, fallbackFormatter);
            ConfigureFormatter(formatter, operationDescription);
            return formatter;
        }

        internal StreamingRawFormatterBase CreateClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint, IClientMessageFormatter fallbackFormatter)
        {
            var formatter = OnCreateFormatter();
            formatter.Initialize(operationDescription, endpoint, fallbackFormatter);
            ConfigureFormatter(formatter, operationDescription);
            return formatter;
        }

        protected internal abstract StreamingRawFormatterBase OnCreateFormatter();

        internal void ConfigureFormatter(StreamingRawFormatterBase formatter, OperationDescription operationDescription)
        {
            var parameterTypes = GetParameterTypes(operationDescription);
            var retvalType = GetRetvalType(operationDescription);

            if (formatter.FormattedType == retvalType)
            {
                formatter.Direction |= StreamingRawFormatterDirection.ReturnValue;
            }

            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (formatter.FormattedType == parameterTypes[i])
                {
                    formatter.InParameterIndex = i;
                    formatter.Direction |= StreamingRawFormatterDirection.ParameterIn;
                }
            }
        }
    }
}
