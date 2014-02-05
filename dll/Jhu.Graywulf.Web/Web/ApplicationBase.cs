using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Jhu.Graywulf.Web.Security;
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

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedVirtualPathProvider());
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
        /// Called when a user signs in
        /// </summary>
        internal protected abstract void OnUserSignedIn(GraywulfIdentity identity);

        /// <summary>
        /// Called when a user sings out
        /// </summary>
        internal protected abstract void OnUserSignedOut();

        #endregion
    }
}
