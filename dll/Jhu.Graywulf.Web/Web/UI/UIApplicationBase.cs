using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.UI
{
    public class UIApplicationBase : ApplicationBase, IDisposable
    {
        private static List<Type> apps;
        private static List<Type> services;
        private static List<MenuButton> dropdownButtons;
        private static List<MenuButton> menuButtons;
        private static List<MenuButton> footerButtons;
        private EmbeddedVirtualPathProvider virtualPathProvider;

        private LoggingContext applicationLoggingContext;

        public List<Type> Apps
        {
            get { return apps; }
        }

        public List<Type> Services
        {
            get { return services; }
        }

        public List<MenuButton> DropdownButtons
        {
            get { return dropdownButtons; }
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
            dropdownButtons = new List<MenuButton>();
            menuButtons = new List<MenuButton>();
            footerButtons = new List<MenuButton>();
        }

        public UIApplicationBase()
        {
            virtualPathProvider = new EmbeddedVirtualPathProvider();

            applicationLoggingContext = new LoggingContext(true, Components.AmbientContextStoreLocation.Default);
            applicationLoggingContext.Pop();
        }

        public override void Dispose()
        {
            applicationLoggingContext.Push();
            applicationLoggingContext.Dispose();

            base.Dispose();
        }

        public RegistryContext CreateRegistryContext()
        {
            return CreateRegistryContext(Registry.TransactionMode.ManualCommit | TransactionMode.ReadOnly);
        }

        /// <summary>
        /// Gets an initialized registry context.
        /// </summary>
        public RegistryContext CreateRegistryContext(TransactionMode transactionMode)
        {
            var context = ContextManager.Instance.CreateContext(Registry.TransactionMode.ManualCommit | transactionMode);
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

        protected void Application_Start(object sender, EventArgs e)
        {
            applicationLoggingContext.StartLogger(EventSource.WebUI, false);
            applicationLoggingContext.LogOperation(
                    Logging.EventSource.WebUI,
                    String.Format("The web application at {0} has started.", VirtualPathUtility.ToAbsolute("~/")),
                    null,
                    new Dictionary<string, object>() { { "UserAccount", String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName) } });

            using (new LoggingContext(applicationLoggingContext))
            {
                OnApplicationStart();
            }
        }

        protected virtual void OnApplicationStart()
        {
            HostingEnvironment.RegisterVirtualPathProvider(virtualPathProvider);
            OnRegisterScripts();
            OnRegisterControls();
            OnRegisterServices();
            OnRegisterApps();
            OnRegisterButtons();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // This is a workaround that's required by WCF not to throw an exception
            // when first request by a client to the web service returns streaming
            // response
            string sessionId = Session.SessionID;

            using (new LoggingContext(applicationLoggingContext))
            {
                OnSessionStart();
            }
        }

        protected virtual void OnSessionStart()
        {
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {
            using (new LoggingContext(applicationLoggingContext))
            {
                OnSessionEnd();
            }
        }

        protected virtual void OnSessionEnd()
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            using (new LoggingContext(applicationLoggingContext))
            {
                OnApplicationEnd();
            }

            applicationLoggingContext.LogOperation(
                Logging.EventSource.WebUI,
                String.Format("The web application at {0} has stopped.", VirtualPathUtility.ToAbsolute("~/")),
                null,
                null);
            applicationLoggingContext.StopLogger();
        }

        protected virtual void OnApplicationEnd()
        {
        }

        protected virtual void OnRegisterScripts()
        {
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.JQuery());
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.JQueryValidation());
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.Bootstrap());
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.BootstrapLighbox());
            Scripts.ScriptLibrary.RegisterMappings(new Scripts.DockingPanel());
        }

        protected virtual void OnRegisterControls()
        {
        }

        protected virtual void OnRegisterServices()
        {
        }

        protected virtual void OnRegisterApps()
        {
        }

        protected virtual void OnRegisterButtons()
        {
            var home = new MenuButton()
            {
                Text = "Home",
                NavigateUrl = Constants.PageUrlDefault,
            };
            RegisterMenuButton(home, MenuButtonPosition.First);
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

        public void RegisterDropdownButton(MenuButton button)
        {
            dropdownButtons.Add(button);
        }

        public void RegisterMenuButton(MenuButton button)
        {
            RegisterMenuButton(button, MenuButtonPosition.Last, null);
        }

        public void RegisterMenuButton(MenuButton button, MenuButtonPosition position)
        {
            RegisterMenuButton(button, position, null);
        }

        public void RegisterMenuButton(MenuButton button, MenuButtonPosition position, string relativeTo)
        {
            MenuButton relative;
            int relativeIndex = 0;

            if (relativeTo != null)
            {
                relative = menuButtons.FirstOrDefault(b => b.Key == relativeTo);

                if (relative != null)
                {
                    relativeIndex = menuButtons.IndexOf(relative);
                }
            }

            switch (position)
            {
                case MenuButtonPosition.First:
                    menuButtons.Insert(0, button);
                    break;
                case MenuButtonPosition.Last:
                    menuButtons.Add(button);
                    break;
                case MenuButtonPosition.Before:
                    menuButtons.Insert(relativeIndex, button);
                    break;
                case MenuButtonPosition.After:
                    menuButtons.Insert(relativeIndex + 1, button);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void RegisterFooterButton(MenuButton button)
        {
            footerButtons.Add(button);
        }
    }
}