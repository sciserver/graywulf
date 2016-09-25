using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Web.UI.Controls;

namespace Jhu.Graywulf.Web.UI.Apps.Common
{
    public class App : AppBase
    {
        public override void RegisterButtons(UIApplicationBase application)
        {
            application.RegisterFooterButton(new MenuButton()
            {
                Text = "feedback",
                NavigateUrl = "~/Apps/Common/Feedback.aspx"
            });
        }
    }
}