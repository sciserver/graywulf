using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web
{
    public class PageBase : Page, IContextObject
    {
        private Context registryContext;

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
                if (User.Identity is GraywulfIdentity)
                {
                    var identity = (GraywulfIdentity)User.Identity;
                    identity.User.Context = RegistryContext;
                    return identity.User;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets an initialized  registry context.
        /// </summary>
        public Context RegistryContext
        {
            get
            {
                if (registryContext == null)
                {
                    var application = (ApplicationBase)HttpContext.Current.ApplicationInstance;
                    registryContext = application.CreateRegistryContext();
                }

                return registryContext;
            }
        }

        Context IContextObject.Context
        {
            get { return registryContext; }
            set { throw new InvalidOperationException(); }
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
            if (registryContext != null)
            {
                if (registryContext.DatabaseTransaction != null)
                {
                    registryContext.CommitTransaction();
                }

                registryContext.Dispose();
            }

            base.OnUnload(e);
        }

        protected bool IsAuthenticatedUser(Guid userGuid)
        {
            return userGuid != ((GraywulfIdentity)User.Identity).UserReference.Guid;
        }

        protected Logging.Event LogError(Exception ex)
        {
            var error = Logging.Logger.Instance.LogException(
                AppRelativeVirtualPath,
                Logging.EventSource.WebUI,
                registryContext == null ? Guid.Empty : registryContext.UserGuid,
                registryContext == null ? Guid.Empty : registryContext.ContextGuid,
                ex);

            return error;
        }

        protected override void OnError(EventArgs e)
        {
            var ex = Server.GetLastError();

            var error = LogError(ex);

            // Save exception to session for future use
            Session[Constants.SessionException] = ex;
            Session[Constants.SessionExceptionEventID] = error.EventId;

            if (registryContext != null)
            {
                registryContext.RollbackTransaction();
                registryContext.Dispose();
                registryContext = null;
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
