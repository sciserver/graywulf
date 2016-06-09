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
            var formatter = CreateClientFormatter(clientOperation.Formatter);
            ConfigureFormatter(formatter, operationDescription);
            clientOperation.Formatter = formatter;
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var formatter = CreateDispatchFormatter(dispatchOperation.Formatter);
            ConfigureFormatter(formatter, operationDescription);
            dispatchOperation.Formatter = formatter;
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        protected abstract StreamingRawFormatterBase CreateDispatchFormatter(IDispatchMessageFormatter formatter);

        protected abstract StreamingRawFormatterBase CreateClientFormatter(IClientMessageFormatter formatter);

        private void ConfigureFormatter(StreamingRawFormatterBase formatter, OperationDescription operationDescription)
        {
            var parameterTypes = GetParameterTypes(operationDescription);
            var retvalType = GetRetvalType(operationDescription);

            if (formatter.FormattedType == retvalType)
            {
                formatter.Direction |= StreamingRawFormatterDirection.ReturnValue;
            }

            if (parameterTypes.Length == 1 && formatter.FormattedType == parameterTypes[0])
            {
                formatter.Direction |= StreamingRawFormatterDirection.Parameters;
            }
        }
    }
}
