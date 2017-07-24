using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public partial class Footer : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;

            Controls.Add(new Literal()
            {
                Text = Application[Jhu.Graywulf.Web.UI.Constants.ApplicationCopyright] + " - powered by Graywulf"
            });

            foreach (var button in application.FooterButtons)
            {
                Controls.Add(new Literal() { Text = " | " });

                var b = new HtmlAnchor()
                {
                    InnerText = button.Text.ToLower(),
                    HRef = VirtualPathUtility.MakeRelative(Page.AppRelativeVirtualPath, button.NavigateUrl),
                };

                Controls.Add(b);
            }

            Controls.Add(new Literal() { Text = " | " });
            Controls.Add(new Literal() { Text = " version: " });
            Controls.Add(new Literal() { Text = (string)Application[Web.UI.Constants.ApplicationVersion] });
        }
    }
}