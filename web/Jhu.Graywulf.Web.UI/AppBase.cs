using System.Collections.Generic;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Web.UI.Controls;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class AppBase
    {
        private Context registryContext;

        protected virtual string Name
        {
            get
            {
                var nsname = this.GetType().Namespace;
                var i = nsname.LastIndexOf('.');

                if (i >= 0)
                {
                    nsname = nsname.Substring(i + 1);
                }

                return nsname;
            }
        }

        public Context RegistryContext
        {
            get { return registryContext; }
            set { registryContext = value; }
        }

        public virtual void Initialize(UIApplicationBase application)
        {
        }

        public virtual void RegisterButtons(UIApplicationBase application)
        {
            var button = new MenuButton();
            button.Text = Name;
            button.NavigateUrl = "~/Apps/" + Name + "/Default.aspx";

            application.RegisterMenuButton(button);
        }

        public virtual void RegisterVirtualPaths(UIApplicationBase application, EmbeddedVirtualPathProvider vpp)
        {
        }

        public virtual void RegisterChecks(List<CheckRoutineBase> checks)
        {
        }

        public virtual void OnUserArrived(UIApplicationBase application, GraywulfPrincipal principal)
        {
        }

        public virtual void OnUserLeft(UIApplicationBase application, GraywulfPrincipal principal)
        {

        }
    }
}