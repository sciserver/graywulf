using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// Implements an operation behaviour that limits access to the decorated
    /// operations by requiring role membership to execute them
    /// </summary>
    public class LimitedAccessOperationAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        {
            // Wrap original invoker into custom invoker
            dispatchOperation.Invoker =
                new LimitedAccessOperationInvoker(
                    operationDescription.Name,
                    dispatchOperation.Invoker);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}
