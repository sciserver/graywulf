using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public class Dropdown : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;

            var dropdown = new Panel()
            {
                CssClass = "dropdown gw-header-dropdown",
            };

            var button = new HtmlGenericControl("button");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "dropdown");
            button.Attributes.Add("class", "btn dropdown-toggle");

            var label = new Label()
            {
                Text = (string)Page.Application[Constants.ApplicationDomainName] + " :: " + (string)Page.Application[Constants.ApplicationShortTitle],
            };

            var glyph = new HtmlGenericControl("span");
            glyph.Attributes.Add("class", "glyphicon glyphicon-menu-down");

            var ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("class", "dropdown-menu");

            foreach (var b in application.DropdownButtons)
            {
                var li = new HtmlGenericControl("li");

                var a = new HyperLink()
                {
                    Text = b.Text,
                    NavigateUrl = b.NavigateUrl,
                };
                
                ul.Controls.Add(li);
                li.Controls.Add(a);
            }

            dropdown.Controls.Add(button);
            button.Controls.Add(label);
            button.Controls.Add(glyph);
            dropdown.Controls.Add(ul);
            this.Controls.Add(dropdown);
        }
    }
}