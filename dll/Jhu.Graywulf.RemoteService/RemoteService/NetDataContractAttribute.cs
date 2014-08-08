using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.RemoteService
{

    /// <summary>
    /// Implements a contract behaviour that replaces default serialization
    /// logic with NetDataContractSerializer to use specific versions
    /// of assemblies for services.
    /// </summary>
    public class NetDataContractAttribute : Attribute, IContractBehavior
    {
        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            foreach (var operation in contractDescription.Operations)
            {
                ReplaceDataContractSerializerOperationBehavior(operation);
            }
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
            foreach (var operation in contractDescription.Operations)
            {
                ReplaceDataContractSerializerOperationBehavior(operation);
            }
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            
        }

        private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description)
        {
            var dcsob = description.Behaviors.Find<DataContractSerializerOperationBehavior>();

            if (dcsob != null)
            {
                description.Behaviors.Remove(dcsob);
                description.Behaviors.Add(new NetDataContractSerializerOperationBehavior(description));
            }
        }

    }
}
