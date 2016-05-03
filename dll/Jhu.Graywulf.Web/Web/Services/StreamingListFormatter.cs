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
            var writer = new StreamingListXmlMessageBodyWriter(result);
            var message = WebOperationContext.Current.CreateStreamResponse(writer, Constants.MimeTypeXml);
            return message;
        }
    }
}
