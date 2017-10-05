using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Web.UI
{
    public class PageBase : Page, IRegistryContextObject
    {
        #region Private member variables

        private string overrideUrl;
        private WebLoggingContext loggingContext;
        private RegistryContext registryContext;
        private FederationContext federationContext;
        
        #endregion
        #region Properties

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

        internal WebLoggingContext LoggingContext
        {
            get { return loggingContext; }
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
                    identity.User.RegistryContext = RegistryContext;
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
        public RegistryContext RegistryContext
        {
            get
            {
                if (registryContext == null)
                {
                    var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;
                    registryContext = application.CreateRegistryContext();
                }

                return registryContext;
            }
        }

        RegistryContext IRegistryContextObject.RegistryContext
        {
            get { return registryContext; }
            set { throw new InvalidOperationException(); }
        }

        public FederationContext FederationContext
        {
            get
            {
                if (federationContext == null)
                {
                    federationContext = new FederationContext(RegistryContext, RegistryUser);
                }

                return federationContext;
            }
        }

        protected bool IsAuthenticatedUser(Guid userGuid)
        {
            return userGuid != ((GraywulfIdentity)User.Identity).UserReference.Guid;
        }

        #endregion
        #region Constructors and initializers

        public PageBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.overrideUrl = null;
            this.registryContext = null;
            this.loggingContext = new WebLoggingContext(Logging.LoggingContext.Current);
        }

        #endregion
        #region Event hook handlers

        protected override void OnPreInit(EventArgs e)
        {
            loggingContext.Push();

            LogDebug();

            base.OnPreInit(e);
            loggingContext.Pop();
        }

        protected override void OnInit(EventArgs e)
        {
            loggingContext.Push();
            base.OnInit(e);
            loggingContext.Pop();
        }

        protected override void OnInitComplete(EventArgs e)
        {
            loggingContext.Push();
            base.OnInitComplete(e);
            loggingContext.Pop();
        }

        protected override void OnPreLoad(EventArgs e)
        {
            loggingContext.Push();
            base.OnPreLoad(e);
            loggingContext.Pop();
        }

        protected override void OnLoad(EventArgs e)
        {
            loggingContext.Push();

            base.OnLoad(e);

            if (!IsPostBack && Request.UrlReferrer != null)
            {
                OriginalReferer = Request.UrlReferrer.ToString();
            }

            loggingContext.Pop();
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            loggingContext.Push();
            base.RaisePostBackEvent(sourceControl, eventArgument);
            loggingContext.Pop();
        }

        public override void Validate()
        {
            loggingContext.Push();
            base.Validate();
            loggingContext.Pop();
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            loggingContext.Push();
            base.OnLoadComplete(e);
            loggingContext.Pop();
        }

        protected override void OnPreRender(EventArgs e)
        {
            loggingContext.Push();

            base.OnPreRender(e);

            if (!String.IsNullOrEmpty(overrideUrl))
            {
                ScriptManager.RegisterClientScriptBlock(this, typeof(PageBase), "Jhu.Graywulf.Web.UI.PageBase.OverrideUrl", GetOverrideUrlScript(), true);
            }

            loggingContext.Pop();
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            loggingContext.Push();
            base.OnPreRenderComplete(e);
            loggingContext.Pop();
        }

        protected override void OnSaveStateComplete(EventArgs e)
        {
            loggingContext.Push();
            base.OnSaveStateComplete(e);
            loggingContext.Pop();
        }

        protected override void OnUnload(EventArgs e)
        {
            loggingContext.Push();

            base.OnUnload(e);

            if (registryContext != null)
            {
                registryContext.CommitTransaction();
                registryContext.Dispose();
                registryContext = null;
            }

            loggingContext.Pop();
            loggingContext.Dispose();
        }

        protected override void OnError(EventArgs e)
        {
            var ex = Server.GetLastError();

#if BREAKDEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
#endif

            var error = LogError(ex);

            // Save exception to session for future use
            Session[Constants.SessionException] = ex;
            Session[Constants.SessionExceptionEventID] = error.ID;

            if (registryContext != null)
            {
                registryContext.RollbackTransaction();
                registryContext.Dispose();
                registryContext = null;
            }

            base.OnError(e);

            Server.ClearError();

            // Redirect is only possible if response is buffered
            // Do not redirect when downloading data via streams
            // File output will be corrupt and no error will be
            // reported to the user.
            if (!Response.HeadersWritten)
            {
                Response.Redirect(Constants.PageUrlError, false);
            }
        }

        #endregion
        #region Utility functions

        public Control FindControlRecursive(string id)
        {
            return Util.PageUtility.FindControlRecursive(this, id);
        }

        #endregion
        #region Browser address bar URL override

        public void OverrideUrl(string overrideUrl)
        {
            this.overrideUrl = overrideUrl;
        }

        private string GetOverrideUrlScript()
        {
            var script =
@"if (typeof (history.replaceState) != 'undefined') {{
    var obj = {{ Page: '{0}', Url: '{1}' }};
    history.replaceState(obj, obj.Page, obj.Url);
}}";

            return String.Format(script, "", overrideUrl);
        }

        #endregion
        #region Logging

        internal void LogDebug()
        {
            var operation = this.GetType().BaseType.FullName;
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Debug,
                Logging.EventSource.WebUI,
                null,
                operation,
                null,
                null);

            Logging.LoggingContext.Current.WriteEvent(e);
        }

        protected Logging.Event LogError(Exception ex)
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Error,
                Logging.EventSource.WebUI,
                null,
                null,
                ex,
                null);

            // Create a bookmark and report to user
            e.BookmarkGuid = Guid.NewGuid();

            Logging.LoggingContext.Current.WriteEvent(e);

            return e;
        }

        #endregion
    }
}
