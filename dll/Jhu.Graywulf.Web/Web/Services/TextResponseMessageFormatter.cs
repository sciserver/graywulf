using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services
{
    public class TextResponseMessageFormatter : GraywulfMessageFormatter, IDispatchMessageFormatter
    {
        public override string[] GetSupportedMimeTypes()
        {
            return new string[] { Constants.MimeTypeText };
        }

        public override void DeserializeRequest(Message message, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            return WebOperationContext.Current.CreateStreamResponse(new TextResponseMessageBodyWriter(result), Constants.MimeTypeText);
        }
    }
}