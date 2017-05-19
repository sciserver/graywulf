using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public partial class Menu : System.Web.UI.UserControl
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;
            var key = ((PageBase)Page).SelectedButton;

            var menu = new Panel()
            {
                CssClass = "menu"
            };

            foreach (var button in application.MenuButtons)
            {
                var b = new HyperLink()
                {
                    Text = button.Text.ToLower(),
                    NavigateUrl = VirtualPathUtility.MakeRelative(Page.AppRelativeVirtualPath, button.NavigateUrl),
                };

                if (key != null && StringComparer.InvariantCultureIgnoreCase.Compare(button.Key, key) == 0)
                {
                    b.CssClass = "selected";
                }

                menu.Controls.Add(b);
            }

            var span = new HtmlGenericControl("div");
            span.Attributes.Add("class", "span");

            menu.Controls.Add(span);
            this.Controls.Add(menu);
        }
    }
}