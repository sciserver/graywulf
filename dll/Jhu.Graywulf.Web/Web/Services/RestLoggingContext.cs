using System;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Web.Services
{
    public class RestLoggingContext : UserLoggingContext
    {
        #region Singleton access

        public static new RestLoggingContext Current
        {
            get
            {
                return LoggingContext.Current as RestLoggingContext;
            }
        }

        #endregion

        public RestLoggingContext(LoggingContext outerContext)
            : base(outerContext)
        {
            InitializeMembers(new StreamingContext());
        }

        public RestLoggingContext(RestLoggingContext outerContext)
            : base(outerContext)
        {
            CopyMembers(outerContext);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        private void CopyMembers(RestLoggingContext outerContext)
        {
        }

        public override void UpdateEvent(Event e)
        {
            base.UpdateEvent(e);

            string client = null;

            var wcfcontext = System.ServiceModel.OperationContext.Current;

            if (wcfcontext != null)
            {
                if (wcfcontext.IncomingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    var req = (HttpRequestMessageProperty)wcfcontext.IncomingMessageProperties["httpRequest"];
                    client = req.Headers["X-Forwarded-For"];

                    if (client == null)
                    {
                        if (wcfcontext.IncomingMessageProperties.ContainsKey(RemoteEndpointMessageProperty.Name))
                        {
                            var ep = (RemoteEndpointMessageProperty)wcfcontext.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];

                            client = ep.Address;
                        }
                    }
                }
            }

            e.Client = client;
        }
    }
}
