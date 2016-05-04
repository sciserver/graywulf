using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class StreamingRawFormatter<T> : RestMessageFormatter, IClientMessageFormatter, IDispatchMessageFormatter
    {
        protected abstract StreamingRawAdapter<T> CreateAdapter();

        public override string[] GetSupportedMimeTypes()
        {
            return CreateAdapter().GetSupportedMimeTypes();
        }

        #region Server

        public override void DeserializeRequest(Message message, object[] parameters)
        {
            if (!message.IsEmpty)
            {
                var body = message.GetReaderAtBodyContents();
                byte[] raw = body.ReadContentAsBase64();

                using (var ms = new MemoryStream(raw))
                {
                    var adapter = CreateAdapter();
                    parameters[parameters.Length - 1] = adapter.ReadFromStream(ms);
                }
            }
        }

        public override Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var adapter = CreateAdapter();
            var message = WebOperationContext.Current.CreateStreamResponse(
                stream => 
                {
                    adapter.WriteToStream(stream, (T)result); 
                }, 
                adapter.GetRequestedContentType());

            return message;
        }

        #endregion
        #region Client

        public override object DeserializeReply(Message message, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
