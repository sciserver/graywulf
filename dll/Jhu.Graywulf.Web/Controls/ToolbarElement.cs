using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    [ParseChildren]
    public class ToolbarElement : WebControl, INamingContainer
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var css = !String.IsNullOrEmpty(CssClass) ? CssClass : ((Toolbar)Parent).CssClass;

            // render style attribute
            ControlStyle.AddAttributesToRender(writer);
            AddAttributesToRender(writer);
            writer.AddAttribute("class", css);

            writer.RenderBeginTag("td");

            foreach (var c in Controls.Cast<Control>())
            {
                c.RenderControl(writer);
            }

            writer.RenderEndTag();
        }
    }
}
