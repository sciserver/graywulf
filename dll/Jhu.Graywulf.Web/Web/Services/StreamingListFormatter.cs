using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services
{
    class StreamingListFormatter : GraywulfMessageFormatter, IDispatchMessageFormatter
    {
        public override string[] GetSupportedMimeTypes()
        {
            return new string[] { 
                Constants.MimeTypeXml,
                Constants.MimeTypeJson,
            };
        }

        public override void DeserializeRequest(Message message, object[] parameters)
        {

        }

        public override Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var t = result.GetType();

            if (!t.IsSubclassOf

            /*
            // Look for an IEnumerable<T> interface on type
            Type t = null;
            var interfaces = result.GetType().GetInterfaces();

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].IsGenericType &&
                    interfaces[i].GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    t = interfaces[i].GetGenericArguments()[0];
                    break;
                }
            }

            if (t == null)
            {
                throw new InvalidOperationException("IEnumarable<T> expected.");
            }

            var writer = new StreamingListXmlMessageBodyWriter((IEnumerable)result, t);
            var message = WebOperationContext.Current.CreateStreamResponse(writer, Constants.MimeTypeXml);
            return message;
             * */
        }
    }
}
