using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.Security;
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

        public Context RegistryContext
        {
            get
            {
                if (context == null)
                {
                    context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.ManualCommit);
                    context.ContextGuid = (Guid)Session[Constants.SessionContextGuid];

                    if (Request.IsAuthenticated)
                    {
                        if (Username == null)
                        {
                            registryUser = new User(context);
                            registryUser.Guid = UserGuid;
                            registryUser.Load();

                            Session[Constants.SessionUsername] = RegistryUser.Name;
                        }

                        context.UserGuid = UserGuid;
                        context.UserName = Username;
                    }
                }

                return context;
            }
        }

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
                    myDBDatabaseInstance = RegistryUser.GetUserDatabaseInstance(MyDBDatabaseVersion);
                }

                return myDBDatabaseInstance;
            }
        }

        public Guid UserGuid
        {
            get
            {
                if (Request.IsAuthenticated)
                {
                    return new Guid(User.Identity.Name);
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        public string Username
        {
            get
            {
                if (Request.IsAuthenticated)
                {
                    return (string)Session[Constants.SessionUsername];
                }
                else
                {
                    return null;
                }
            }
        }

        public Jhu.Graywulf.Registry.User RegistryUser
        {
            get
            {
                if (UserGuid != Guid.Empty)
                {
                    if (registryUser == null)
                    {
                        registryUser = new Jhu.Graywulf.Registry.User(RegistryContext);
                        registryUser.Guid = UserGuid;
                        registryUser.Load();
                    }

                    return registryUser;
                }
                else
                {
                    return null;
                }
            }
        }

        #region Initializer functions

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            EnsureUserIdentified();
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

        protected virtual void EnsureUserIdentified()
        {
            // Check newly signed in user
            // If user is authenticated successfully User.Identity.Name is set to
            // the user's GUID (intead of their name). Compare it with session variables
            // to make sure the user we see is actually the one who's authenticated.
            if (Request.IsAuthenticated &&
                (Session[Constants.SessionUsername] == null ||
                 (Guid)Session[Constants.SessionUserGuid] != Guid.Parse(User.Identity.Name)))
            {
                Session[Constants.SessionUsername] = RegistryUser.Name;
                Session[Constants.SessionUserGuid] = RegistryUser.Guid;
                OnUserSignedIn();
            }

            // Check signed off user
            if (!Request.IsAuthenticated && Session[Constants.SessionUsername] != null)
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
