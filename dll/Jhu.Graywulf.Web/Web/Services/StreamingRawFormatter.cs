using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class StreamingRawFormatter<T> : StreamingRawFormatterBase
    {
        internal override Type FormattedType
        {
            get
            {
                return typeof(T);
            }
        }

        protected StreamingRawFormatter()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            
        }

        private WebHeaderCollection GetRequestHeaders()
        {
            var message = OperationContext.Current.RequestContext.RequestMessage;
            return GetRequestHeaders(message);
        }

        private WebHeaderCollection GetRequestHeaders(Message message)
        {
            var prop = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
            return prop.Headers;
        }

        private WebHeaderCollection GetResponseHeaders(Message message)
        {
            var prop = (HttpResponseMessageProperty)message.Properties[HttpResponseMessageProperty.Name];
            return prop.Headers;
        }

        protected abstract StreamingRawAdapter<T> CreateAdapter();

        public override List<RestBodyFormat> GetSupportedFormats()
        {
            return CreateAdapter().GetSupportedFormats();
        }

        #region Server

        public override void DeserializeRequest(Message message, object[] parameters)
        {
            if (!message.IsEmpty &&
                (Direction & StreamingRawFormatterDirection.ParameterIn) != 0)
            {
                DeserializeUrlParameters(message, parameters);

                var body = message.GetReaderAtBodyContents();
                byte[] raw = body.ReadContentAsBase64();

                using (var ms = new MemoryStream(raw))
                {
                    var adapter = CreateAdapter();
                    adapter.Headers = GetRequestHeaders(message);
                    parameters[InParameterIndex] = adapter.ReadFromStream(ms);
                }
            }
            else
            {
                base.DeserializeRequest(message, parameters);
            }
        }

        public override Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            if ((Direction & StreamingRawFormatterDirection.ReturnValue) != 0)
            {
                var adapter = CreateAdapter();
                adapter.Headers = GetRequestHeaders();
                var contentType = adapter.GetRequestedContentType();

                var message = WebOperationContext.Current.CreateStreamResponse(
                    stream =>
                    {
                        adapter.WriteToStream(stream, (T)result);
                    },
                    contentType);

                return message;
            }
            else
            {
                return base.SerializeReply(messageVersion, parameters, result);
            }
        }

        #endregion
        #region Client

        public override Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            if ((Direction & StreamingRawFormatterDirection.ParameterIn) != 0 &&
                parameters != null)
            {
                var adapter = CreateAdapter();
                var format = adapter.GetPreferredFormat();
                var data = (T)parameters[InParameterIndex];
                var body = new StreamingRawBodyWriter<T>(adapter, format.MimeType, data);
                var action = Operation.Messages[0].Action;
                var message = Message.CreateMessage(messageVersion, action, body);

                if (!message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    message.Properties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());
                }

                message.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));

                var prop = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
                prop.Headers[HttpRequestHeader.ContentType] = format.MimeType;

                SerializeUrlParameters(message, parameters);
                
                return message;
            }
            else
            {
                return base.SerializeRequest(messageVersion, parameters);
            }
        }

        public override object DeserializeReply(Message message, object[] parameters)
        {
            if ((Direction & StreamingRawFormatterDirection.ReturnValue) != 0)
            {
                var body = message.GetReaderAtBodyContents();
                byte[] raw = body.ReadContentAsBase64();

                using (var ms = new MemoryStream(raw))
                {
                    var adapter = CreateAdapter();
                    adapter.Headers = GetResponseHeaders(message);
                    return adapter.ReadFromStream(ms);
                }
            }
            else
            {
                return base.DeserializeReply(message, parameters);
            }
        }

        #endregion
    }
}
