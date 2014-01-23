using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Jhu.Graywulf.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Web
{
    public abstract class ApplicationBase : System.Web.HttpApplication
    {
        /// <summary>
        /// Gets an initialized registry context.
        /// </summary>
        public Context CreateRegistryContext()
        {
            var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.ManualCommit);

            var session = HttpContext.Current.Session;
            if (session != null)
            {

                if (session[Constants.SessionPrincipal] != null)
                {
                    var sessionPrincipal = (GraywulfPrincipal)session[Constants.SessionPrincipal];
                    var userProperty = ((GraywulfIdentity)sessionPrincipal.Identity).UserProperty;
                    
                    context.UserGuid = userProperty.Guid;
                    context.UserName = userProperty.Name;
                }

                if (session[Constants.SessionClusterGuid] != null)
                {
                    context.ClusterProperty.Guid = (Guid)session[Constants.SessionClusterGuid];
                }

                if (session[Constants.SessionDomainGuid] != null)
                {
                    context.DomainProperty.Guid = (Guid)session[Constants.SessionDomainGuid];
                }

                if (session[Constants.SessionFederationGuid] != null)
                {
                    context.FederationProperty.Guid = (Guid)session[Constants.SessionFederationGuid];
                }
            }

            return context;
        }

        /// <summary>
        /// Gets the authenticated Graywulf user
        /// </summary>
        public User RegistryUser
        {
            get { return ((GraywulfIdentity)User.Identity).User; }
        }

        /// <summary>
        /// Initializes the application events.
        /// </summary>
        public override void Init()
        {
            base.Init();

            this.PostAcquireRequestState += new EventHandler(ApplicationBase_PostAcquireRequestState);
        }

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedVirtualPathProvider());
        }

        void ApplicationBase_PostAcquireRequestState(object sender, EventArgs e)
        {
            IdentifyUser();
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {
        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
        }

        #region User managemenet functions

        /// <summary>
        /// Checks if the authenticated user appears for the first time,
        /// and if so, raises an event. Also checks if the user signed out.
        /// </summary>
        private void IdentifyUser()
        {
            var session = HttpContext.Current.Session;

            if (session != null)
            {
                // Get the saved principal from the session
                var sessionPrincipal = (GraywulfPrincipal)session[Constants.SessionPrincipal];

                // If the current request is authenticated by Graywulf, and this is the
                // first time we see the user, we need to load details from the registry
                if (Request.IsAuthenticated && User is GraywulfPrincipal)
                {
                    if (sessionPrincipal != null)
                    {
                        // Make sure that the known user is the same as the one
                        // just being authenticated
                        if (!sessionPrincipal.Identity.CompareByIdentifier((GraywulfIdentity)User.Identity))
                        {
                            // This is someone we haven't seen
                            OnUserSignedOut();
                            session.Abandon();
                        }
                    }

                    // A new user has just arrived.
                    using (Registry.Context context = CreateRegistryContext())
                    {
                        ((GraywulfIdentity)User.Identity).LoadUser(context.Domain);
                    }

                    session[Constants.SessionPrincipal] = User;
                    OnUserSignedIn((GraywulfIdentity)User.Identity);
                }
                else if (!Request.IsAuthenticated && sessionPrincipal != null)
                {
                    // A user left
                    OnUserSignedOut();
                    session.Abandon();
                }
            }
        }

        /// <summary>
        /// Called when a user signs in
        /// </summary>
        protected abstract void OnUserSignedIn(GraywulfIdentity identity);

        /// <summary>
        /// Called when a user sings out
        /// </summary>
        protected abstract void OnUserSignedOut();

        #endregion
    }
}
