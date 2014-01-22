using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using Jhu.Graywulf.Security;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web
{
    public class PageBase : Page
    {
        private Context context;

        /// <summary>
        /// Gets the original referer of the page, even after postbacks.
        /// </summary>
        public string OriginalReferer
        {
            get { return (string)(ViewState[Constants.OriginalReferer] ?? String.Empty); }
            private set { ViewState[Constants.OriginalReferer] = value; }
        }

        /// <summary>
        /// Gets the root URL of the current web application
        /// </summary>
        public string BaseUrl
        {
            get { return Util.UrlFormatter.ToBaseUrl(Request.Url.AbsoluteUri, Request.ApplicationPath); }
        }

        /// <summary>
        /// Gets the return url from the query string of the request
        /// </summary>
        public string ReturnUrl
        {
            get { return Request.QueryString[Constants.ReturnUrl] ?? ""; }
        }

        /// <summary>
        /// Gets the authenticated Graywulf user
        /// </summary>
        public User RegistryUser
        {
            get
            {
                var identity = (GraywulfIdentity)User.Identity;
                identity.UserProperty.Context = RegistryContext;
                return identity.User;
            }
        }

        /// <summary>
        /// Gets an initialized  registry context.
        /// </summary>
        public Context RegistryContext
        {
            get
            {
                if (context == null)
                {
                    var application = (ApplicationBase)HttpContext.Current.ApplicationInstance;
                    context = application.CreateRegistryContext();
                }

                return context;
            }
        }

        public Cluster Cluster
        {
            get { return RegistryContext.Cluster; }
        }

        public Domain Domain
        {
            get { return RegistryContext.Domain; }
        }

        public Federation Federation
        {
            get { return RegistryContext.Federation; }
        }

        #region Initializer functions

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && Request.UrlReferrer != null)
            {
                OriginalReferer = Request.UrlReferrer.ToString();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (context != null)
            {
                if (context.DatabaseTransaction != null)
                {
                    context.CommitTransaction();
                }

                context.Dispose();
            }

            base.OnUnload(e);
        }

        protected bool IsAuthenticatedUser(Guid userGuid)
        {
            return userGuid != ((GraywulfIdentity)User.Identity).UserProperty.Guid;
        }

        protected Logging.Event LogError(Exception ex)
        {
            var error = new Logging.Event(AppRelativeVirtualPath, Guid.Empty);
            error.Exception = ex;
            error.ExceptionType = ex.GetType().Name;
            error.Message = ex.Message;
            error.StackTrace = ex.StackTrace;

            if (context != null)
            {
                error.UserGuid = context.UserGuid;
                error.ContextGuid = context.ContextGuid;
            }

            error.EventSeverity = Logging.EventSeverity.Error;
            error.EventSource = Logging.EventSource.WebUI;

            Logging.Logger.Instance.LogEvent(error);

            return error;
        }

        protected override void OnError(EventArgs e)
        {
            var ex = Server.GetLastError();

            var error = LogError(ex);

            // Save exception to session for future use
            Session[Constants.SessionException] = ex;
            Session[Constants.SessionExceptionEventID] = error.EventId;

            if (context != null)
            {
                context.RollbackTransaction();
                context.Dispose();
                context = null;
            }

            Server.ClearError();

            base.OnError(e);

            Response.Redirect(Jhu.Graywulf.Web.Error.GetUrl());
        }

        #endregion

        public Control FindControlRecursive(string id)
        {
            return FindControlRecursive(this, id);
        }

        public Control FindControlRecursive(Control root, string id)
        {
            Control f = root.FindControl(id);
            if (f == null)
            {
                foreach (Control c in root.Controls)
                {
                    Control x = FindControlRecursive(c, id);
                    if (x != null) return x;
                }
                return null;
            }
            else
            {
                return f;
            }
        }
    }
}
