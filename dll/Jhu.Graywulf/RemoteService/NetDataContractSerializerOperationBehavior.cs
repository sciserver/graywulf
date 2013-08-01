using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ServiceModel.Description;

namespace Jhu.Graywulf.RemoteService
{
    public class NetDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        public NetDataContractSerializerOperationBehavior(OperationDescription description)
            : base(description)
        {
        }

        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            return new NetDataContractSerializer();
        }

        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            return new NetDataContractSerializer();
        }

    }

}
