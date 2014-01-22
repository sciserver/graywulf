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
        /// Gets an initialized  registry context.
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
            get { return ((GraywulfIdentity)this.User.Identity).User; }
        }

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
            // Flush principal from the cache
            /*var principal = (GraywulfPrincipal)Session[Constants.SessionPrincipal];
            if (principal != null)
            {
                Security.GraywulfAuthenticationModule.FlushGraywulfPrincipal(Application, principal);
            }*/
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

        private void IdentifyUser()
        {
            var session = HttpContext.Current.Session;

            if (session != null)
            {
                var sessionPrincipal = (GraywulfPrincipal)session[Constants.SessionPrincipal];

                if (Request.IsAuthenticated && User is GraywulfPrincipal && sessionPrincipal == null)
                {
                    using (Registry.Context context = CreateRegistryContext())
                    {
                        ((GraywulfIdentity)User.Identity).LoadUser(context.Domain);
                    }

                    session[Constants.SessionPrincipal] = User;
                    OnUserSignedIn((GraywulfIdentity)User.Identity);
                }
                else if (!Request.IsAuthenticated && sessionPrincipal != null)
                {
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
