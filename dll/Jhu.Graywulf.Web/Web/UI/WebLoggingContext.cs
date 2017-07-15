using System;
using System.Web;
using System.Runtime.Serialization;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Web.UI
{
    public class WebLoggingContext : UserLoggingContext
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
            
            e.Client = client;

            //var error = Logging.LoggingContext.Current.LogError(Logging.EventSource.WebUI, ex, null, AppRelativeVirtualPath, null);
        }
    }
}
