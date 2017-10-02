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

        public string SelectedButton
        {
            get { return (string)ViewState["SelectedButton"]; }
            set { ViewState["SelectedButton"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;

            // If the key is not set, figure out active button from the URL
            if (String.IsNullOrWhiteSpace(SelectedButton))
            {
                var m = AppRegex.Match(Page.AppRelativeVirtualPath);
                var app = m.Success ? m.Groups[1].Value : null;

                SelectedButton = app;
            }

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

                if (!String.IsNullOrWhiteSpace(SelectedButton) && StringComparer.InvariantCultureIgnoreCase.Compare(button.Key, SelectedButton) == 0)
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