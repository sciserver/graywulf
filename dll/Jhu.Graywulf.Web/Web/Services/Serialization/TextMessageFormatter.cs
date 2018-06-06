using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    public class TextMessageFormatter : RestMessageFormatter
    {
        public override List<RestBodyFormat> GetSupportedFormats()
        {
            return new List<RestBodyFormat>()
            {
                RestBodyFormats.Text
            };
        }

        public override void DeserializeRequest(Message message, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            return WebOperationContext.Current.CreateStreamResponse(new TextResponseMessageBodyWriter(result), Constants.MimeTypeText);
        }

        public override Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override object DeserializeReply(Message message, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}