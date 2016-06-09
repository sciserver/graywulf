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
    public abstract class RestMessageFormatter : IDispatchMessageFormatter, IClientMessageFormatter 
    {
        private IDispatchMessageFormatter fallbackDispatchMessageFormatter;
        private IClientMessageFormatter fallbackClientMessageFormatter;
        private string mimeType;

        protected IDispatchMessageFormatter FallbackDispatchMessageFormatter
        {
            get
            {
                return fallbackDispatchMessageFormatter;
            }
        }

        protected IClientMessageFormatter FallbackClientMessageFormatter
        {
            get
            {
                return fallbackClientMessageFormatter;
            }
        }

        public string MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        protected RestMessageFormatter()
        {
        }

        protected RestMessageFormatter(IDispatchMessageFormatter dispatchMessageFormatter)
        {
            InititializeMembers();

            this.fallbackDispatchMessageFormatter = dispatchMessageFormatter;
        }

        protected RestMessageFormatter(IClientMessageFormatter clientMessageFormatter)
        {
            InititializeMembers();

            this.fallbackClientMessageFormatter = clientMessageFormatter;
        }

        private void InititializeMembers()
        {
            this.fallbackDispatchMessageFormatter = null;
            this.fallbackClientMessageFormatter = null;
            this.mimeType = null;
        }

        public abstract string[] GetSupportedMimeTypes();

        public virtual void DeserializeRequest(Message message, object[] parameters)
        {
            fallbackDispatchMessageFormatter.DeserializeRequest(message, parameters);
        }

        public virtual Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            return fallbackDispatchMessageFormatter.SerializeReply(messageVersion, parameters, result);
        }

        public virtual Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            return fallbackClientMessageFormatter.SerializeRequest(messageVersion, parameters);
        }

        public virtual object DeserializeReply(Message message, object[] parameters)
        {
            return fallbackClientMessageFormatter.DeserializeReply(message, parameters);
        }
    }
}
