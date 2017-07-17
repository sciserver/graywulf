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

            string request = null;
            string operation = null;
            string client = null;

            var context = System.ServiceModel.OperationContext.Current;

            if (context != null)
            {
                if (e.Operation == null && context.IncomingMessageProperties.ContainsKey("HttpOperationName"))
                {
                    operation = context.Host.Description.ServiceType.FullName + "." +
                        (string)context.IncomingMessageProperties["HttpOperationName"];
                }

                if (e.Request == null && context.IncomingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    var req = (HttpRequestMessageProperty)context.IncomingMessageProperties["httpRequest"];

                    request =
                        req.Method.ToUpper() + " " +
                        context.IncomingMessageProperties.Via.AbsolutePath +
                        context.IncomingMessageProperties["HttpOperationName"];
                }

                if (e.Client == null && context.IncomingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    var req = (HttpRequestMessageProperty)context.IncomingMessageProperties["httpRequest"];
                    client = req.Headers["X-Forwarded-For"];

                    if (client == null)
                    {
                        if (context.IncomingMessageProperties.ContainsKey(RemoteEndpointMessageProperty.Name))
                        {
                            var ep = (RemoteEndpointMessageProperty)context.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];

                            client = ep.Address;
                        }
                    }
                }
            }

            if (request != null) e.Request = request;
            if (operation != null) e.Operation = operation;
            if (client != null) e.Client = client;
        }
    }
}
