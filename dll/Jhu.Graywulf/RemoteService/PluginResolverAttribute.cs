using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;

namespace Jhu.Graywulf.RemoteService
{
    public class PluginResolverAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
            var b = description.Behaviors.Find<DataContractSerializerOperationBehavior>();
            b.DataContractResolver = new PluginDataContractResolver();
        }

        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
        {
            var b = description.Behaviors.Find<DataContractSerializerOperationBehavior>();
            b.DataContractResolver = new PluginDataContractResolver();
        }

        public void Validate(OperationDescription description)
        {
        }
    }
}
