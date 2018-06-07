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

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            // Intentionally do nothing, just register as a dummy behavior
            // Formatter will be registered in RestEndpointBehavior.CreateDispatchFormatter
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            // Intentionally do nothing, just register as a dummy behavior
            // Formatter will be registered in RestEndpointBehavior.CreateDispatchFormatter
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        public RawMessageFormatterBase CreateFormatter()
        {
            return (RawMessageFormatterBase)Activator.CreateInstance(formatterType);
        }
    }
}
