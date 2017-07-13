using System;
using System.Web;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Logging
{
    public class WebLoggingContext : LoggingContext
    {
        #region Singleton access

        public static new WebLoggingContext Current
        {
            get
            {
                return LoggingContext.Current as WebLoggingContext;
            }
        }

        #endregion

        public WebLoggingContext(LoggingContext outerContext)
            : base(outerContext)
        {
            InitializeMembers(new StreamingContext());
        }

        public WebLoggingContext(WebLoggingContext outerContext)
            : base(outerContext)
        {
            CopyMembers(outerContext);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        private void CopyMembers(WebLoggingContext outerContext)
        {
        }

        public override void UpdateEvent(Event e)
        {
            base.UpdateEvent(e);

            Guid userGuid = Guid.Empty;
            Guid sessionGuid = Guid.Empty;
            string client = null;

            var webcontext = System.Web.HttpContext.Current;

            if (webcontext != null)
            {
                var req = webcontext.Request;
                client = req.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (client == null)
                {
                    client = req.UserHostAddress;
                }
            }

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

            var principal = System.Threading.Thread.CurrentPrincipal as Jhu.Graywulf.AccessControl.GraywulfPrincipal;

            if (principal != null)
            {
                userGuid = principal.Identity.UserReference.Guid;
                Guid.TryParse(principal.Identity.SessionId, out sessionGuid);
            }

            e.SessionGuid = sessionGuid;
            e.UserGuid = userGuid;
            e.Principal = System.Threading.Thread.CurrentPrincipal;
            e.Client = client;
        }
    }
}
