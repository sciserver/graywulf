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

        public string MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        protected IDispatchMessageFormatter FallbackDispatchMessageFormatter
        {
            get { return fallbackDispatchMessageFormatter; }
        }

        protected IClientMessageFormatter FallbackClientMessageFormatter
        {
            get { return fallbackClientMessageFormatter; }
        }

        protected RestMessageFormatter()
        {
            InitializeMembers();
        }

        protected RestMessageFormatter(IDispatchMessageFormatter fallbackFormatter)
        {
            InitializeMembers();

            this.fallbackDispatchMessageFormatter = fallbackFormatter;
        }

        protected RestMessageFormatter(IClientMessageFormatter fallbackFormatter)
        {
            InitializeMembers();

            this.fallbackClientMessageFormatter = fallbackFormatter;
        }

        private void InitializeMembers()
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
    }
}
