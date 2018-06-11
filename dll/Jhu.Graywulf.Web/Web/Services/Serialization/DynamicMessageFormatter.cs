using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    /// <summary>
    /// Implements dynamic message format selection based on HTTP request
    /// headers.
    /// </summary>
    class DynamicMessageFormatter : RestMessageFormatterBase
    {
        private Dictionary<string, RestMessageFormatterBase> formatters;

        public Dictionary<string, RestMessageFormatterBase> Formatters
        {
            get { return formatters; }
        }

        #region Constructors and initializers

        public DynamicMessageFormatter()
        {
            InitializeMembers();
        }

        public DynamicMessageFormatter(IDispatchMessageFormatter fallbackFormatter, IDictionary<string, RestMessageFormatterBase> formatters)
        {
            InitializeMembers();

            base.Initialize(fallbackFormatter);
            this.formatters = new Dictionary<string, RestMessageFormatterBase>(formatters, StringComparer.InvariantCultureIgnoreCase);
        }

        private void InitializeMembers()
        {
            this.formatters = new Dictionary<string, RestMessageFormatterBase>(StringComparer.InvariantCultureIgnoreCase);
        }

        public override void Initialize(IDispatchMessageFormatter fallbackFormatter)
        {
            base.Initialize(fallbackFormatter);

            foreach (var f in formatters.Values)
            {
                f.Initialize(fallbackFormatter);
            }
        }

        public override void Initialize(IClientMessageFormatter fallbackFormatter)
        {
            base.Initialize(fallbackFormatter);

            foreach (var f in formatters.Values)
            {
                f.Initialize(fallbackFormatter);
            }
        }

        #endregion

        public override List<RestBodyFormat> GetSupportedFormats()
        {
            var res = new List<RestBodyFormat>();

            foreach (var formatter in formatters.Values)
            {
                res.AddRange(formatter.GetSupportedFormats());
            }

            return res;
        }
        
        #region Server

        public override void DeserializeRequest(Message request, object[] parameters)
        {
            IDispatchMessageFormatter formatter = null;
            var headers = GetRequestHeaders(request);
            var contentType = GetPostedContentType(headers);

            foreach (var formatMime in formatters.Keys)
            {
                if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(contentType, formatMime))
                {
                    formatter = formatters[formatMime];
                    break;
                }
            }

            if (formatter == null)
            {
                formatter = FallbackDispatchMessageFormatter;
            }

            formatter.DeserializeRequest(request, parameters);
        }

        public override Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            IDispatchMessageFormatter formatter = null;
            var headers = GetRequestHeaders();
            var accept = GetRequestedContentType(headers, formatters.Keys);

            if (formatters.ContainsKey(accept))
            {
                formatter = formatters[accept];
            }
            else
            {
                formatter = FallbackDispatchMessageFormatter;
            }

            var message = formatter.SerializeReply(messageVersion, parameters, result);
            SetMessageHeaders(message);

            return message;
        }

        #endregion
        #region Client

        public override Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            IClientMessageFormatter formatter = null;
            var format = GetPreferredFormat();

            if (formatters.ContainsKey(format.MimeType))
            {
                formatter = formatters[format.MimeType];
            }
            else
            {
                formatter = FallbackClientMessageFormatter;
            }

            var message = formatter.SerializeRequest(messageVersion, parameters);
            SetMessageHeaders(message);
            return message;
        }

        public override object DeserializeReply(Message message, object[] parameters)
        {
            IClientMessageFormatter formatter = null;
            var headers = GetResponseHeaders(message);
            var contentType = GetPostedContentType(headers);

            if (contentType != null && formatters.ContainsKey(contentType))
            {
                formatter = formatters[contentType];
            }
            else
            {
                formatter = FallbackClientMessageFormatter;
            }

            return formatter.DeserializeReply(message, parameters);
        }

        #endregion
    }
}