using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public partial class Menu : System.Web.UI.UserControl
    {
        static readonly Regex AppRegex = new Regex("/apps/([^/]+)/", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected void Page_Load(object sender, EventArgs e)
        {
            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;
            var m = AppRegex.Match(Page.AppRelativeVirtualPath);
            var app = m.Success ? m.Groups[1].Value : null;

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

                if (app != null && StringComparer.InvariantCultureIgnoreCase.Compare(button.Key, app) == 0)
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