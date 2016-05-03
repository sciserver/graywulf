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
    public abstract class GraywulfMessageFormatter : IDispatchMessageFormatter, IClientMessageFormatter 
    {
        private string mimeType;

        public string MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        protected GraywulfMessageFormatter()
        {
            InititializeMembers();
        }

        private void InititializeMembers()
        {
            this.mimeType = null;
        }

        public abstract string[] GetSupportedMimeTypes();

        public abstract void DeserializeRequest(Message message, object[] parameters);

        public abstract Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result);

        public abstract Message SerializeRequest(MessageVersion messageVersion, object[] parameters);

        public abstract object DeserializeReply(Message message, object[] parameters);
    }
}
