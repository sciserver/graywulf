using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Logging
{
    public class WcfLoggingBehavior : Attribute, IServiceBehavior, IOperationBehavior
    {
        #region IServiceBehavior implementation

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // Automatically add WcfLoggingBehavior to all operations
            var logging = new WcfLoggingBehavior();

            foreach (var ep in serviceDescription.Endpoints)
            {
                foreach (var op in ep.Contract.Operations)
                {
                    op.Behaviors.Add(logging);
                }
            }
        }

        #endregion
        #region IOperationBehavior implementation

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {

            dispatchOperation.Invoker = new WcfLoggingOperationInvoker(operationDescription.Name, dispatchOperation.Invoker);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        #endregion
    }
}
