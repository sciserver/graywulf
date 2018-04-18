using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Web.UI
{
    public class PageBase : Page, IRegistryContextObject
    {
        static readonly Regex AppRegex = new Regex("/apps/([^/]+)/", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #region Private member variables

        private bool readOnly;

        private string overrideUrl;
        private RegistryContext registryContext;
        private FederationContext federationContext;
        
        #endregion
        #region Properties

        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

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

        public virtual string SelectedButton
        {
            get
            {
                var m = AppRegex.Match(Page.AppRelativeVirtualPath);
                var key = m.Success ? m.Groups[1].Value : null;

                return key;
            }
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
        public virtual RegistryContext RegistryContext
        {
            get
            {
                if (registryContext == null)
                {
                    var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;
                    registryContext = application.CreateRegistryContext((!readOnly && IsPostBack)? Registry.TransactionMode.ReadWrite : Registry.TransactionMode.ReadOnly);
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

        protected PageBase(bool readOnly)
        {
            InitializeMembers();

            this.readOnly = readOnly;
        }

        private void InitializeMembers()
        {
            this.readOnly = true;
            this.overrideUrl = null;
            this.registryContext = null;
            this.federationContext = null;
        }

        #endregion
        #region Event hook handlers

        protected override void OnPreInit(EventArgs e)
        {
            new WebLoggingContext();

            LogDebug();

            base.OnPreInit(e);
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && Request.UrlReferrer != null)
            {
                OriginalReferer = Request.UrlReferrer.ToString();
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!String.IsNullOrEmpty(overrideUrl))
            {
                ScriptManager.RegisterClientScriptBlock(this, typeof(PageBase), "Jhu.Graywulf.Web.UI.PageBase.OverrideUrl", GetOverrideUrlScript(), true);
            }
        }
        
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            if (registryContext != null)
            {
                registryContext.CommitTransaction();
                registryContext.Dispose();
                registryContext = null;
            }
        }

        public override void Dispose()
        {
            if (registryContext != null)
            {
                registryContext.RollbackTransaction();
                registryContext.Dispose();
                registryContext = null;
            }

            WebLoggingContext.Current.Dispose();

            base.Dispose();
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
                Response.Redirect(Constants.PageUrlError, true);
            }
        }

        #endregion
        #region Utility functions

        public Control FindControlRecursive(string id)
        {
            return Util.PageUtility.FindControlRecursive(this, id);
        }

        #endregion
        #region Session storage

        protected Guid PushSessionItem(object value)
        {
            var guid = Guid.NewGuid();
            var items = (Dictionary<Guid, object>)Session[Constants.SessionItems];

            if (items == null)
            {
                items = new Dictionary<Guid, object>();
            }

            items.Add(guid, value);
            Session[Constants.SessionItems] = items;

            return guid;
        }

        protected object PopSessionItem(Guid guid)
        {
            var items = (Dictionary<Guid, object>)Session[Constants.SessionItems];

            if (items == null)
            {
                return null;
            }
            else
            {
                var item = items[guid];
                items.Remove(guid);
                Session[Constants.SessionItems] = items;
                return item;
            }
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
