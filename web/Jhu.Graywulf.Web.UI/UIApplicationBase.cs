using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Services;
using Jhu.Graywulf.Web.UI.Controls;

namespace Jhu.Graywulf.Web.UI
{
    public class UIApplicationBase : ApplicationBase
    {
        private static List<Type> apps;
        private static List<Type> services;
        private static List<MenuButton> menuButtons;
        private static List<MenuButton> footerButtons;
        private EmbeddedVirtualPathProvider virtualPathProvider;
        
        public List<Type> Apps
        {
            get { return apps; }
        }

        public List<Type> Services
        {
            get { return services; }
        }

        public List<MenuButton> MenuButtons
        {
            get { return menuButtons; }
        }

        public List<MenuButton> FooterButtons
        {
            get { return footerButtons; }
        }

        protected EmbeddedVirtualPathProvider VirtualPathProvider
        {
            get { return virtualPathProvider; }
        }

        static UIApplicationBase()
        {
            apps = new List<Type>();
            services = new List<Type>();
            menuButtons = new List<MenuButton>();
            footerButtons = new List<MenuButton>();
        }

        public UIApplicationBase()
        {
            virtualPathProvider = new EmbeddedVirtualPathProvider();
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
                    var user = ((GraywulfIdentity)sessionPrincipal.Identity).UserReference.Value;

                    context.UserReference.Value = user;
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
            // Start logger
            Logger.Instance.Writers.Add(new SqlLogWriter());

            HostingEnvironment.RegisterVirtualPathProvider(virtualPathProvider);
            RegisterScripts();
            RegisterControls();
            RegisterButtons();
            RegisterServices();
            RegisterApps();
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

        protected virtual void RegisterScripts()
        {
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.JQuery());
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.JQueryValidation());
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.Bootstrap());
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.BootstrapLighbox());
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.DockingPanel());
        }

        protected virtual void RegisterControls()
        {
        }

        protected virtual void RegisterServices()
        {
        }

        protected virtual void RegisterApps()
        {
        }

        protected virtual void RegisterButtons()
        {
            var button = new MenuButton()
            {
                Text = "Home",
                NavigateUrl = Default.GetUrl()
            };
            RegisterMenuButton(button);
        }

        protected void RegisterService(Type type)
        {
            services.Add(type);
        }

        protected void RegisterApp(Type type)
        {
            apps.Add(type);

            var app = (AppBase)Activator.CreateInstance(type);
            app.RegisterVirtualPaths(this, this.virtualPathProvider);
            app.RegisterButtons(this);
        }

        public void RegisterMenuButton(MenuButton button)
        {
            menuButtons.Add(button);
        }

        public void RegisterFooterButton(MenuButton button)
        {
            footerButtons.Add(button);
        }
    }
}