using System;
using System.Collections.Generic;
using System.Web;
using Jhu.Graywulf.Web.Controls;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public partial class Menu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;

            foreach (var button in application.MenuButtons)
            {
                var b = new ToolbarButton()
                {
                    Text = button.Text.ToLower(),
                    NavigateUrl = button.NavigateUrl,
                    SkinID = "Menu"
                };

                var i = buttonsPlaceholder.Parent.Controls.IndexOf(buttonsPlaceholder);
                buttonsPlaceholder.Parent.Controls.AddAt(i, b);
            }
        }
    }
}