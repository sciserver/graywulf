using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.IO;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    public abstract class RestMessageFormatterBase : IDispatchMessageFormatter, IClientMessageFormatter
    {
        #region Private member variables

        private IDispatchMessageFormatter fallbackDispatchMessageFormatter;
        private IClientMessageFormatter fallbackClientMessageFormatter;

        #endregion
        #region Properties

        protected IDispatchMessageFormatter FallbackDispatchMessageFormatter
        {
            get { return fallbackDispatchMessageFormatter; }
        }

        protected IClientMessageFormatter FallbackClientMessageFormatter
        {
            get { return fallbackClientMessageFormatter; }
        }

        #endregion
        #region Constructors and initializers

        protected RestMessageFormatterBase()
        {
            InitializeMembers();
        }
        
        private void InitializeMembers()
        {
            this.fallbackDispatchMessageFormatter = null;
            this.fallbackClientMessageFormatter = null;
        }

        public virtual void Initialize(IDispatchMessageFormatter fallbackFormatter)
        {
            this.fallbackDispatchMessageFormatter = fallbackFormatter;
        }

        public virtual void Initialize(IClientMessageFormatter fallbackFormatter)
        {
            this.fallbackClientMessageFormatter = fallbackFormatter;
        }

        #endregion

        public abstract List<RestBodyFormat> GetSupportedFormats();

        internal RestBodyFormat GetPreferredFormat()
        {
            var formats = GetSupportedFormats();
            return formats[0];
        }

        #region HTTP header handling

        protected void SetMessageHeaders(Message message)
        {
            WebBodyFormatMessageProperty body;
            HttpRequestMessageProperty http;

            if (!message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name))
            {
                body = new WebBodyFormatMessageProperty(WebContentFormat.Raw);
                message.Properties.Add(WebBodyFormatMessageProperty.Name, body);
            }
            else
            {
                body = (WebBodyFormatMessageProperty)message.Properties[WebBodyFormatMessageProperty.Name];
            }

            if (message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                http = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
            }
            else
            {
                http = new HttpRequestMessageProperty();
                message.Properties.Add(HttpRequestMessageProperty.Name, http);
            }

            OnSetMessageHeaders(http);
        }

        protected virtual void OnSetMessageHeaders(HttpRequestMessageProperty http)
        {
            var accept = "";
            var formats = GetSupportedFormats();

            foreach (var f in formats)
            {
                accept += ", " + f.MimeType;
            }

            if (accept == "")
            {
                accept = "*/*";
            }
            else
            {
                accept = accept.Substring(2);
            }

            http.Headers[HttpRequestHeader.Accept] = accept;
        }

        protected WebHeaderCollection GetRequestHeaders()
        {
            var message = OperationContext.Current.RequestContext.RequestMessage;
            return GetRequestHeaders(message);
        }

        protected WebHeaderCollection GetRequestHeaders(Message message)
        {
            var prop = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
            return prop.Headers;
        }

        protected WebHeaderCollection GetResponseHeaders(Message message)
        {
            var prop = (HttpResponseMessageProperty)message.Properties[HttpResponseMessageProperty.Name];
            return prop.Headers;
        }

        internal string GetPostedContentType(WebHeaderCollection headers)
        {
            var contentType = headers[HttpRequestHeader.ContentType];
            return contentType;
        }

        internal string GetRequestedContentType(WebHeaderCollection headers)
        {
            var acceptHeader = headers[HttpRequestHeader.Accept] ??
                               headers[HttpRequestHeader.ContentType];

            // Parse accept header
            var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();
            var formats = GetSupportedFormats();

            for (int i = 0; i < accept.Count; i++)
            {
                foreach (var format in formats)
                {
                    if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(accept[i].MediaType, format.MimeType))
                    {
                        return format.MimeType;
                    }
                }
            }

            return Constants.MimeTypeText;
        }

        #endregion
        #region WCF interface implementations

        public virtual void DeserializeRequest(Message message, object[] parameters)
        {
            fallbackDispatchMessageFormatter.DeserializeRequest(message, parameters);
        }

        public virtual Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var message = fallbackDispatchMessageFormatter.SerializeReply(messageVersion, parameters, result);
            return message;
        }

        public virtual Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            var message = fallbackClientMessageFormatter.SerializeRequest(messageVersion, parameters);
            return message;
        }

        public virtual object DeserializeReply(Message message, object[] parameters)
        {
            var retval = fallbackClientMessageFormatter.DeserializeReply(message, parameters);
            return retval;
        }

        #endregion
    }
}
