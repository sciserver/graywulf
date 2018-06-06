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
    public class RawFormatAttribute : Attribute, IOperationBehavior
    {
        #region Private member variables

        private Type formatterType;

        #endregion
        #region Properties

        public Type FormatterType
        {
            get { return formatterType; }
            set { formatterType = value; }
        }

        #endregion
        #region Constructors and initializers

        public RawFormatAttribute(Type formatterType)
        {
            InitializeMembers();

            this.formatterType = formatterType;
        }

        private void InitializeMembers()
        {
            this.formatterType = null;
        }

        #endregion

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

        public RawMessageFormatterBase CreateFormatter()
        {
            return (RawMessageFormatterBase)Activator.CreateInstance(formatterType);
        }

        internal RawMessageFormatterBase CreateDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint, IDispatchMessageFormatter fallbackFormatter)
        {
            var formatter = CreateFormatter();
            formatter.Initialize(operationDescription, endpoint, fallbackFormatter);
            ConfigureFormatter(formatter, operationDescription);
            return formatter;
        }

        internal RawMessageFormatterBase CreateClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint, IClientMessageFormatter fallbackFormatter)
        {
            var formatter = CreateFormatter();
            formatter.Initialize(operationDescription, endpoint, fallbackFormatter);
            ConfigureFormatter(formatter, operationDescription);
            return formatter;
        }

        internal void ConfigureFormatter(RawMessageFormatterBase formatter, OperationDescription operationDescription)
        {
            var parameterTypes = GetParameterTypes(operationDescription);
            var retvalType = GetRetvalType(operationDescription);
            var type = formatter.GetFormattedType();

            if (type == retvalType)
            {
                formatter.Direction |= RawMessageFormatterDirection.ReturnValue;
            }

            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (type == parameterTypes[i])
                {
                    formatter.InParameterIndex = i;
                    formatter.Direction |= RawMessageFormatterDirection.ParameterIn;
                }
            }
        }
    }
}
