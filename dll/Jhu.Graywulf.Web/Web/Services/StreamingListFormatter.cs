using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Services
{
    class StreamingListFormatter : RestMessageFormatter, IDispatchMessageFormatter, IClientMessageFormatter
    {

        private Type returnType;

        public StreamingListFormatter(IDispatchMessageFormatter fallbackFormatter)
            :base(fallbackFormatter)
        {
            InitializeMembers();
        }

        public StreamingListFormatter(IClientMessageFormatter fallbackFormatter, Type returnType)
            :base(fallbackFormatter)
        {
            InitializeMembers();
            
            this.returnType = returnType;
        }

        private void InitializeMembers()
        {
            this.returnType = null;
        }

        public override string[] GetSupportedMimeTypes()
        {
            return new string[] { 
                Constants.MimeTypeXml,
                Constants.MimeTypeJson,
            };
        }

        public override void DeserializeRequest(Message message, object[] parameters)
        {
            FallbackDispatchMessageFormatter.DeserializeRequest(message, parameters);
        }

        public override Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var request = OperationContext.Current.RequestContext.RequestMessage;
            var prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            var acceptHeader = prop.Headers[HttpRequestHeader.Accept] ?? prop.Headers[HttpRequestHeader.ContentType];
            var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();

            StreamBodyWriter writer = null;
            string mimetype = Constants.MimeTypeXml;

            foreach (var a in accept)
            {
                if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(a.MediaType, Constants.MimeTypeXml))
                {
                    writer = new StreamingListXmlMessageBodyWriter(result);
                    mimetype = Constants.MimeTypeXml;
                    break;
                }
                else if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(a.MediaType, Constants.MimeTypeJson))
                {
                    writer = new StreamingListJsonMessageBodyWriter(result);
                    mimetype = Constants.MimeTypeJson;
                    break;
                }
            }

            if (writer == null)
            {
                // Default to XML
                writer = new StreamingListXmlMessageBodyWriter(result);
                mimetype = Constants.MimeTypeXml;
            }

            var message = WebOperationContext.Current.CreateStreamResponse(writer, mimetype);
            return message;
        }

        public override Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            return FallbackClientMessageFormatter.SerializeRequest(messageVersion, parameters);
        }

        public override object DeserializeReply(Message message, object[] parameters)
        {
            var reader = new StreamingListXmlMessageBodyReader(returnType);
            return reader.ReadBodyContents(message.GetReaderAtBodyContents());
        }

        internal static void ReflectClass(Type type, out string name, out string ns)
        {
            name = ns = null;

            // Look for any attributes that control serialization of name
            var attrs = type.GetCustomAttributes(true);

            for (int i = 0; i < attrs.Length; i++)
            {
                var attr = attrs[i];

                if (attr is XmlRootAttribute)
                {
                    var a = (XmlRootAttribute)attr;
                    name = a.ElementName;
                    ns = a.Namespace;
                }
                else if (attr is DataContractAttribute)
                {
                    var a = (DataContractAttribute)attr;
                    name = a.Name;
                    ns = a.Namespace;
                }
            }

            name = name ?? type.Name;
            ns = ns ?? "http://schemas.datacontract.org/2004/07/" + type.Namespace;
        }

        internal static void ReflectClass(Type type, out string name, out string ns, out Dictionary<string, PropertyInfo> properties)
        {
            ReflectClass(type, out name, out ns);
            properties = ReflectProperties(type);
        }

        internal static Dictionary<string, PropertyInfo> ReflectProperties(Type type)
        {
            // Iterate through properties
            var res = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                string propName;
                StreamingListFormatter.ReflectProperty(prop, out propName);

                res.Add(propName, prop);
            }

            return res;
        }

        internal static void ReflectProperty(PropertyInfo prop, out string name)
        {
            name = null;

            // Look for any attributes that control serialization of name
            var attrs = prop.GetCustomAttributes(true);

            for (int i = 0; i < attrs.Length; i++)
            {
                var attr = attrs[i];

                if (attr is XmlElementAttribute)
                {
                    var a = (XmlElementAttribute)attr;
                    name = a.ElementName;
                }
                else if (attr is DataMemberAttribute)
                {
                    var a = (DataMemberAttribute)attr;
                    name = a.Name;
                }
            }
        }
    }
}
