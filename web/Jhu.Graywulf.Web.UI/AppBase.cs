using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Web.UI.Controls;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class AppBase
    {
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

        public virtual void Initialize(UIApplicationBase application)
        {
        }

        public virtual void RegisterMenuButtons(UIApplicationBase application)
        {
            var button = new MenuButton();
            button.Text = Name;
            button.NavigateUrl = "~/Apps/" + Name + "/Default.aspx";

            application.RegisterMenuButton(button);
        }

        public virtual void RegisterVirtualPaths(UIApplicationBase application, EmbeddedVirtualPathProvider vpp)
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