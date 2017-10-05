using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Web.UI.Controls;

namespace Jhu.Graywulf.Web.UI.Apps.Docs
{
    public class App : AppBase
    {
        public const string AssetsPath = "~/Assets/Docs";

        public override void RegisterButtons(UIApplicationBase application)
        {
            application.RegisterMenuButton(new MenuButton()
            {
                Text = "docs",
                NavigateUrl = AssetsPath + "/00_index.aspx"
            });

            application.RegisterFooterButton(new MenuButton()
            {
                Text = "help",
                NavigateUrl = AssetsPath + "/00_index.aspx"
            });
        }
    }
}