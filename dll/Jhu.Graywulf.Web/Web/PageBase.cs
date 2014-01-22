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
        private Domain domain;
        private Federation federation;
        private User registryUser;
        private DatabaseDefinition myDBDatabaseDefinition;
        private DatabaseVersion myDBDatabaseVersion;
        private DatabaseInstance myDBDatabaseInstance;

        public string OriginalReferer
        {
            get { return (string)(ViewState[Constants.OriginalReferer] ?? String.Empty); }
            set { ViewState[Constants.OriginalReferer] = value; }
        }

        public string BaseUrl
        {
            get
            {
                var url = new Uri(Request.Url.AbsoluteUri);
                return String.Format("{0}://{1}{2}/", url.Scheme, url.Authority, Request.ApplicationPath.TrimEnd('/'));
            }
        }

        public string ReturnUrl
        {
            get { return Request.QueryString[Constants.ReturnUrl] ?? ""; }
        }

        public User RegistryUser
        {
            get { return ((GraywulfIdentity)this.User.Identity).User; }
        }

        public Context RegistryContext
        {
            get
            {
                if (context == null)
                {
                    context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.ManualCommit);

                    if (Request.IsAuthenticated)
                    {
                        var userProperty = ((GraywulfIdentity)User.Identity).UserProperty;

                        context.UserGuid = userProperty.Guid;
                        context.UserName = userProperty.Name;
                    }
                }

                return context;
            }
        }

        // TODO: delete and use property from context
        public Domain Domain
        {
            get
            {
                if (domain == null)
                {
                    var ef = new EntityFactory(RegistryContext);
                    domain = ef.LoadEntity<Domain>(Registry.AppSettings.DomainName);
                }

                return domain;
            }
        }

        // TODO: delete and use property from context
        public Federation Federation
        {
            get
            {
                if (federation == null)
                {
                    var ef = new EntityFactory(RegistryContext);
                    federation = ef.LoadEntity<Federation>(Registry.AppSettings.FederationName);
                }

                return federation;
            }
        }

        public DatabaseDefinition MyDBDatabaseDefinition
        {
            get
            {
                if (myDBDatabaseDefinition == null)
                {
                    myDBDatabaseDefinition = MyDBDatabaseVersion.DatabaseDefinition;
                }

                return myDBDatabaseDefinition;
            }
        }

        public DatabaseVersion MyDBDatabaseVersion
        {
            get
            {
                if (myDBDatabaseVersion == null)
                {
                    myDBDatabaseVersion = Federation.MyDBDatabaseVersion;
                }

                return myDBDatabaseVersion;
            }
        }

        public DatabaseInstance MyDBDatabaseInstance
        {
            get
            {
                if (myDBDatabaseInstance == null)
                {
                    myDBDatabaseInstance = MyDBDatabaseVersion.GetUserDatabaseInstance(((GraywulfIdentity)User.Identity).User);
                }

                return myDBDatabaseInstance;
            }
        }

        #region Initializer functions

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            EnsureUserAuthenticated();
        }

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
        #region User managemenet functions

        protected virtual void EnsureUserAuthenticated()
        {
            var sessionPrincipal = (GraywulfPrincipal)Session[Constants.SessionPrincipal];

            if (Request.IsAuthenticated && User is GraywulfPrincipal && sessionPrincipal == null)
            {
                Session[Constants.SessionPrincipal] = User;
                OnUserSignedIn();
            }
            else if (!Request.IsAuthenticated && sessionPrincipal != null)
            {
                OnUserSignedOut();
                Session.Abandon();
            }
        }

        /// <summary>
        /// Called when a user signs in
        /// </summary>
        protected virtual void OnUserSignedIn()
        {
        }

        /// <summary>
        /// Called when a user sings out
        /// </summary>
        protected virtual void OnUserSignedOut()
        {
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
