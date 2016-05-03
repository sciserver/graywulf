using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class ApplicationBase : System.Web.HttpApplication
    {
        private EmbeddedVirtualPathProvider virtualPathProvider;

        protected EmbeddedVirtualPathProvider VirtualPathProvider
        {
            get { return virtualPathProvider; }
        }

        /// <summary>
        /// Gets an initialized registry context.
        /// </summary>
        public Context CreateRegistryContext()
        {
            var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.ManualCommit);

            var session = HttpContext.Current.Session;
            if (session != null)
            {
                // TODO: user info in context might be correct already and no
                // need to load from session. Test this, however, with the admin interface
                // before deleting this code
                if (session[Constants.SessionPrincipal] != null)
                {
                    var sessionPrincipal = (GraywulfPrincipal)session[Constants.SessionPrincipal];
                    var userProperty = ((GraywulfIdentity)sessionPrincipal.Identity).UserReference;

                    context.UserGuid = userProperty.Guid;
                    context.UserName = userProperty.Name;
                }

                // TODO: These are only used by the admin interface, consider removing them:
                if (session[Constants.SessionClusterGuid] != null)
                {
                    context.ClusterReference.Guid = (Guid)session[Constants.SessionClusterGuid];
                }

                if (session[Constants.SessionDomainGuid] != null)
                {
                    context.DomainReference.Guid = (Guid)session[Constants.SessionDomainGuid];
                }

                if (session[Constants.SessionFederationGuid] != null)
                {
                    context.FederationReference.Guid = (Guid)session[Constants.SessionFederationGuid];
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
            // Initialize virtual paths
            virtualPathProvider = new EmbeddedVirtualPathProvider();

            // Register controls
            virtualPathProvider.RegisterVirtualPath("~/Captcha.aspx", typeof(Jhu.Graywulf.Web.UI.Captcha), ".aspx");
            virtualPathProvider.RegisterVirtualPath("~/Feedback.aspx", typeof(Jhu.Graywulf.Web.UI.Feedback), ".aspx");
            virtualPathProvider.RegisterVirtualPath("~/Error.aspx", typeof(Jhu.Graywulf.Web.UI.Error), ".aspx");

            HostingEnvironment.RegisterVirtualPathProvider(virtualPathProvider);



            // Start logger
            Logger.Instance.Writers.Add(new SqlLogWriter());
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {
            // This is a workaround that's required by WCF not to throw an exception
            // when first request by a client to the web service returns streaming
            // response
            string sessionId = Session.SessionID; 
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
        internal protected virtual void OnUserArrived(GraywulfPrincipal principal)
        { }

        /// <summary>
        /// Called when a user sings out
        /// </summary>
        internal protected virtual void OnUserLeft(GraywulfPrincipal principal)
        { }

        #endregion
    }
}
