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
    class DynamicDispatchMessageFormatter : IDispatchMessageFormatter
    {
        private IDispatchMessageFormatter fallbackFormatter;
        private Dictionary<string, IDispatchMessageFormatter> formatters;

        public Dictionary<string, IDispatchMessageFormatter> Formatters
        {
            get { return formatters; }
        }

        public DynamicDispatchMessageFormatter(IDispatchMessageFormatter fallbackFormatter, IDictionary<string, IDispatchMessageFormatter> formatters)
        {
            this.fallbackFormatter = fallbackFormatter;
            this.formatters = new Dictionary<string, IDispatchMessageFormatter>(formatters, StringComparer.InvariantCultureIgnoreCase);
        }

        public void DeserializeRequest(Message request, object[] parameters)
        {
            var prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            var contentType = prop.Headers[HttpRequestHeader.ContentType];

            IDispatchMessageFormatter formatter = null;

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
                formatter = fallbackFormatter;
            }

            formatter.DeserializeRequest(request, parameters);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var request = OperationContext.Current.RequestContext.RequestMessage;
            var prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            var acceptHeader = prop.Headers[HttpRequestHeader.Accept] ?? prop.Headers[HttpRequestHeader.ContentType];

            // Parse accept header
            var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();

            // Because we want to match patterns, look-up by mime type is not a way to go.
            // Loop over each item instead.

            IDispatchMessageFormatter formatter = null;

            for (int i = 0; i < accept.Count; i++)
            {
                foreach (var formatMime in formatters.Keys)
                {
                    if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(accept[i].MediaType, formatMime))
                    {
                        formatter = formatters[formatMime];
                        break;
                    }
                }

                if (formatter != null)
                {
                    break;
                }
            }

            if (formatter == null)
            {
                formatter = fallbackFormatter;
            }

            return formatter.SerializeReply(messageVersion, parameters, result);
        }
    }
}