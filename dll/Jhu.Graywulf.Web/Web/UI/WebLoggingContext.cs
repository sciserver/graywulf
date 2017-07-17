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

            e.Source |= EventSource.WebUI;

            string request = null;
            string client = null;

            var context = System.Web.HttpContext.Current;

            if (context != null)
            {
                var req = context.Request;

                if (e.Request == null)
                {
                    request = req.HttpMethod + " " + req.Url.AbsolutePath;
                }

                client = req.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (client == null)
                {
                    client = req.UserHostAddress;
                }
            }

            if (request != null) e.Request = request;
            if (client != null) e.Client = client;
        }
    }
}
